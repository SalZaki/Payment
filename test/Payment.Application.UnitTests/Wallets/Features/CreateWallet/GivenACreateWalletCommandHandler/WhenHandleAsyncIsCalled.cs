using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using OneOf;
using Payment.Application.UnitTests.Helpers;
using Payment.Application.Wallets.Features.CreateWallet;
using Payment.Application.Wallets.Repositories;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.Application.UnitTests.Wallets.Features.CreateWallet.GivenACreateWalletCommandHandler;

/// <summary>
/// Could filter by `dotnet test --filter "Category=CreateWalletCommandHandler"` in running tests in command line
/// </summary>
[Trait("Category", "CreateWalletCommandHandler")]
public sealed class WhenHandleAsyncIsCalled
{
  private readonly Fixture _fixture;
  private readonly IWalletRepository _walletRepositoryMock;
  private readonly IValidator<CreateWalletCommand> _createWalletCommandValidatorMock;
  private readonly ICommandHandler<CreateWalletCommand, OneOf<CreateWalletResponse, ValidationErrorResponse, ErrorResponse>> _sut;

  public WhenHandleAsyncIsCalled()
  {
    _fixture = new Fixture();
    _fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
    _walletRepositoryMock = Substitute.For<IWalletRepository>();
    _createWalletCommandValidatorMock = Substitute.For<IValidator<CreateWalletCommand>>();

    _sut = new CreateWalletCommandHandler(_walletRepositoryMock, _createWalletCommandValidatorMock);
  }

  [Fact]
  public void Then_should_have_constructor_guard_clauses()
  {
    // arrange
    var assertion = new GuardClauseAssertion(_fixture);

    // act
    assertion.Verify(typeof(CreateWalletCommand).GetConstructors());
  }

  [Fact]
  public async Task Then_should_throw_exception_given_null_command()
  {
    // act & assert
    await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.HandleAsync(null!, CancellationToken.None));
  }

  [Fact]
  public async Task Then_should_handle_validation_errors_given_invalid_command()
  {
    // arrange
    var command = _fixture.Create<CreateWalletCommand>();

    var errorMessage = "some-error-message";

    var validationResult = new ValidationResult
    {
      Errors = { new ValidationFailure("some-property", errorMessage) }
    };

    _createWalletCommandValidatorMock
      .ValidateAsync(Arg.Any<CreateWalletCommand>())
      .Returns(validationResult);

    // act
    var actualResponse = await _sut.HandleAsync(command, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.IsT1.Should().BeTrue();
    actualResponse.Value.Should().BeOfType<ValidationErrorResponse>();
    actualResponse.AsT1.Errors.Should().HaveCount(1);
    actualResponse.AsT1.Errors.Any(x => x.Contains(errorMessage)).Should().BeTrue();
    await _createWalletCommandValidatorMock.Received(1).ValidateAsync(command);
  }

  [Theory]
  [InlineData(typeof(Exception), "general exception")]
  [InlineData(typeof(ArgumentException), "argument exception")]
  [InlineData(typeof(InvalidOperationException), "invalid operation exception")]
  public async Task Then_should_handle_exceptions_given_unexpected_errors(Type type, string errorMessage)
  {
    // arrange
    var userId = UserId.Create(_fixture.Create<Guid>());

    var command = new CreateWalletCommand
    {
      UserId = userId.ToString()
    };

    var expectedError = $"Failed to create wallet due to {errorMessage}.";

    var exception = ExceptionHelper.CreateExceptionBy(type, errorMessage) ?? new Exception(errorMessage);

    _createWalletCommandValidatorMock
      .ValidateAsync(Arg.Any<CreateWalletCommand>())
      .Returns(new ValidationResult());

    _walletRepositoryMock
      .AddAsync(Arg.Any<Wallet>())
      .Throws(exception);

    // act
    var actualResponse = await _sut.HandleAsync(command, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.Value.Should().BeOfType<ErrorResponse>();
    actualResponse.IsT2.Should().BeTrue();
    actualResponse.AsT2.Error.Should().Be(expectedError);
  }

  [Fact]
  public async Task Then_should_return_create_wallet_response_given_valid_command()
  {
    // arrange
    var userId = UserId.Create(_fixture.Create<Guid>());
    var walletId = WalletId.Create(_fixture.Create<Guid>());
    var amount = _fixture.Create<decimal>();
    var walletAmount = Money.Create(amount, Currency.GBP, Units.Major);
    var wallet = Wallet.Create(walletId, userId, walletAmount);

    var command = new CreateWalletCommand
    {
      UserId = userId.ToString(),
      Amount = amount,
      Currency = Currency.GBP.Code
    };

    _createWalletCommandValidatorMock
      .ValidateAsync(Arg.Any<CreateWalletCommand>(), CancellationToken.None)
      .Returns(new ValidationResult());

    _walletRepositoryMock
      .AddAsync(Arg.Any<Wallet>(), CancellationToken.None)
      .Returns(wallet);

    // act
    var actualResponse = await _sut.HandleAsync(command, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.Value.Should().BeOfType<CreateWalletResponse>();
    actualResponse.IsT0.Should().BeTrue();
    actualResponse.AsT0.WalletId.Should().NotBeEmpty();
  }
}
