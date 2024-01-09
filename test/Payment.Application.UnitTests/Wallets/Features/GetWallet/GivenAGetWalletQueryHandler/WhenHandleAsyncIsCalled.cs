using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Payment.Application.Wallets.Features.GetWallet;
using Payment.Application.Wallets.Repositories;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;
using Payment.Common.Mappers;
using Payment.Domain.Entities;
using OneOf;
using Payment.Application.UnitTests.Helpers;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.Application.UnitTests.Wallets.Features.GetWallet.GivenAGetWalletQueryHandler;

/// <summary>
/// Could filter by `dotnet test --filter "Category=GetWalletQueryHandler"` in running tests in command line
/// </summary>
[Trait("Category", "GetWalletQueryHandler")]
public sealed class WhenHandleAsyncIsCalled
{
  private readonly Fixture _fixture;
  private readonly IWalletRepository _walletRepositoryMock;
  private readonly IValidator<GetWalletQuery> _getWalletQueryValidatorMock;
  private readonly IMapper<Wallet, GetWalletResponse> _walletToGetWalletResponseMapperMock;
  private readonly IQueryHandler<GetWalletQuery, OneOf<GetWalletResponse, ValidationErrorResponse, ErrorResponse>> _sut;

  public WhenHandleAsyncIsCalled()
  {
    _fixture = new Fixture();
    _fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
    _walletRepositoryMock = Substitute.For<IWalletRepository>();
    _walletToGetWalletResponseMapperMock = Substitute.For<IMapper<Wallet, GetWalletResponse>>();
    _getWalletQueryValidatorMock = Substitute.For<IValidator<GetWalletQuery>>();
    _sut = new GetWalletQueryHandler(_walletRepositoryMock, _getWalletQueryValidatorMock, _walletToGetWalletResponseMapperMock);
  }

  [Fact]
  public void Then_should_have_constructor_guard_clauses()
  {
    // arrange
    var assertion = new GuardClauseAssertion(_fixture);

    // act
    assertion.Verify(typeof(GetWalletQueryHandler).GetConstructors());
  }

  [Fact]
  public async Task Then_should_throw_exception_given_null_query()
  {
    // act & assert
    await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.HandleAsync(null!, CancellationToken.None));
  }

  [Fact]
  public async Task Then_should_handle_validation_errors_given_invalid_query()
  {
    // arrange
    var query = _fixture.Create<GetWalletQuery>();

    var validationResult = new ValidationResult
    {
      Errors = {new ValidationFailure("WalletId", "TestError")}
    };

    _getWalletQueryValidatorMock
      .ValidateAsync(Arg.Any<GetWalletQuery>())
      .Returns(validationResult);

    // act
    var response = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    response.Should().NotBeNull();
    response.IsT1.Should().BeTrue();
    response.Value.Should().BeOfType<ValidationErrorResponse>();
    response.AsT1.Errors.Should().HaveCount(1);
    response.AsT1.Errors.First().Should().Be("`WalletId`: TestError");
    await _getWalletQueryValidatorMock.Received(1).ValidateAsync(query);
  }

  [Theory]
  [InlineData(typeof(Exception), "general exception")]
  [InlineData(typeof(ArgumentException), "argument exception")]
  [InlineData(typeof(InvalidOperationException), "invalid operation exception")]
  public async Task Then_should_handle_mapper_exceptions_given_unexpected_errors(Type type, string errorMessage)
  {
    // arrange
    var walletId = WalletId.Create(_fixture.Create<Guid>());

    var query = new GetWalletQuery
    {
      WalletId = walletId.ToString()
    };

    var expectedError = $"Failed to get wallet with id {query.WalletId} due to {errorMessage}.";

    var exception = ExceptionHelper.CreateExceptionBy(type, errorMessage) ?? new Exception(errorMessage);

    _getWalletQueryValidatorMock
      .ValidateAsync(Arg.Any<GetWalletQuery>())
      .Returns(new ValidationResult());

    _walletToGetWalletResponseMapperMock
      .Map(Arg.Any<Wallet>())
      .Throws(exception);

    // act
    var response = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    response.Should().NotBeNull();
    response.Value.Should().BeOfType<ErrorResponse>();
    response.IsT2.Should().BeTrue();
    response.AsT2.Error.Should().Be(expectedError);
    _walletToGetWalletResponseMapperMock.Received(1).Map(Arg.Any<Wallet>());
  }

  [Fact]
  public async Task Then_should_return_error_response_given_wallet_not_found()
  {
    // arrange
    var walletId = WalletId.Create(_fixture.Create<Guid>());

    var query = new GetWalletQuery
    {
      WalletId = walletId.ToString()
    };

    var expectedResponse = new ErrorResponse { Error = $"Failed to get wallet with id {query.WalletId}." };

    _getWalletQueryValidatorMock
      .ValidateAsync(Arg.Any<GetWalletQuery>(), CancellationToken.None)
      .Returns(new ValidationResult());

    _walletRepositoryMock
      .FindByIdAsync(Arg.Any<WalletId>(), CancellationToken.None)
      .Returns(Wallet.NotFound);

    // act
    var actualResponse = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.Value.Should().BeOfType<ErrorResponse>();
    actualResponse.IsT2.Should().BeTrue();
    actualResponse.AsT2.Should().Be(expectedResponse);
  }

  [Fact]
  public async Task Then_should_return_get_wallet_response_given_valid_query()
  {
    // arrange
    var userId = UserId.Create(_fixture.Create<Guid>());
    var contributorId = UserId.Create(_fixture.Create<Guid>());
    var walletId = WalletId.Create(_fixture.Create<Guid>());
    var amount = _fixture.Create<decimal>();
    var walletAmount = Money.Create(amount, Currency.GBP, Units.Major);
    var wallet = Wallet.Create(walletId, userId, walletAmount);
    var shareId = ShareId.Create(_fixture.Create<Guid>());
    var shareAmount = Money.Create(20, Currency.USD, Units.Major);

    wallet.Contribute(shareAmount, contributorId);

    var expectedResponse = new GetWalletResponse
    {
      WalletId = walletId.ToString(),
      UserId = userId.ToString(),
      Amount = amount,
      Currency = Currency.GBP.Code,
      Shares = new[]
      {
        new GetWalletShareResponse
        {
          ShareId = shareId.ToString(),
          Amount = shareAmount.InMajorUnits,
          Currency = Currency.USD.Code,
          ContributorId = contributorId.ToString(),
          WalletId = walletId.ToString()
        }
      }
    };

    var query = new GetWalletQuery
    {
      WalletId = walletId.ToString()
    };

    _getWalletQueryValidatorMock
      .ValidateAsync(Arg.Any<GetWalletQuery>(), CancellationToken.None)
      .Returns(new ValidationResult());

    _walletRepositoryMock
      .FindByIdAsync(Arg.Any<WalletId>(), CancellationToken.None)
      .Returns(wallet);

    _walletToGetWalletResponseMapperMock
      .Map(Arg.Any<Wallet>())
      .Returns(expectedResponse);

    // act
    var actualResponse = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.Value.Should().BeOfType<GetWalletResponse>();
    actualResponse.IsT0.Should().BeTrue();
    actualResponse.AsT0.Should().Be(expectedResponse);
  }
}
