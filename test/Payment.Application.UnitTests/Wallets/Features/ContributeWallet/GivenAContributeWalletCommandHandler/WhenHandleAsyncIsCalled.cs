using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Optional;
using Payment.Application.Wallets.Features.ContributeWallet;
using Payment.Application.Wallets.Repositories;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using Optional.Unsafe;
using Payment.Application.UnitTests.Helpers;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.Application.UnitTests.Wallets.Features.ContributeWallet.GivenAContributeWalletCommandHandler;

/// <summary>
/// Could filter by `dotnet test --filter "Category=ContributeWalletCommandHandler"` in running tests in command line
/// </summary>
[Trait("Category", "ContributeWalletCommandHandler")]
public sealed class WhenHandleAsyncIsCalled
{
  private readonly Fixture _fixture;
  private readonly IWalletRepository _walletRepositoryMock;
  private readonly IValidator<ContributeWalletCommand> _contributeWalletCommandValidatorMock;
  private readonly ICommandHandler<ContributeWalletCommand, Option<ValidationErrorResponse, ErrorResponse>> _sut;

  public WhenHandleAsyncIsCalled()
  {
    _fixture = new Fixture();
    _fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
    _walletRepositoryMock = Substitute.For<IWalletRepository>();
    _contributeWalletCommandValidatorMock = Substitute.For<IValidator<ContributeWalletCommand>>();

    _sut = new ContributeWalletCommandHandler(_walletRepositoryMock, _contributeWalletCommandValidatorMock);
  }

  [Fact]
  public void Then_should_have_constructor_guard_clauses()
  {
    // arrange
    var assertion = new GuardClauseAssertion(_fixture);

    // act
    assertion.Verify(typeof(ContributeWalletCommand).GetConstructors());
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
    var command = _fixture.Create<ContributeWalletCommand>();

    var expectedErrorMessage = "some-error-message";

    var validationResult = new ValidationResult
    {
      Errors = {new ValidationFailure("some-property", expectedErrorMessage)}
    };

    _contributeWalletCommandValidatorMock
      .ValidateAsync(Arg.Any<ContributeWalletCommand>())
      .Returns(validationResult);

    // act
    var actualResponse = await _sut.HandleAsync(command, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.HasValue.Should().BeTrue();
    var actualErrorResponse = actualResponse.ValueOrDefault();
    actualErrorResponse.Should().BeOfType<ValidationErrorResponse>();
    actualErrorResponse.Errors.Should().HaveCount(1);
    actualErrorResponse.Errors.Any(x => x.Contains(expectedErrorMessage)).Should().BeTrue();
    await _contributeWalletCommandValidatorMock.Received(1).ValidateAsync(command);
  }

  [Theory]
  [InlineData(typeof(Exception), "general exception")]
  [InlineData(typeof(ArgumentException), "argument exception")]
  [InlineData(typeof(InvalidOperationException), "invalid operation exception")]
  public async Task Then_should_handle_exceptions_given_unexpected_errors(Type type, string errorMessage)
  {
    // arrange
    var walletId = WalletId.Create(_fixture.Create<Guid>());
    var userId = UserId.Create(_fixture.Create<Guid>());
    var contributorId = UserId.Create(_fixture.Create<Guid>());
    var amount = _fixture.Create<decimal>();
    var currency = Currency.GBP.Code;
    var walletAmount = Money.Create(amount, Currency.GBP, Units.Major);
    var wallet = Wallet.Create(walletId, userId, walletAmount);

    var command = new ContributeWalletCommand
    {
      WalletId = walletId.ToString(),
      Shares = new[]
      {
        new ContributeShareCommand
        {
          Amount = amount,
          Currency = currency,
          ContributorId = contributorId.ToString()
        }
      },
      CreatedOrModifiedBy = "TestSystem",
      CreatedOrModifiedOn = DateTime.UtcNow
    };

    var expectedError = $"Failed to contribute wallet with id {command.WalletId} due to {errorMessage}.";

    var exception = ExceptionHelper.CreateExceptionBy(type, errorMessage) ?? new Exception(errorMessage);

    _contributeWalletCommandValidatorMock
      .ValidateAsync(Arg.Any<ContributeWalletCommand>())
      .Returns(new ValidationResult());

    _walletRepositoryMock
      .FindByIdAsync(Arg.Any<WalletId>(), Arg.Any<CancellationToken>())
      .Returns(wallet);

    _walletRepositoryMock
      .UpdateAsync(Arg.Any<Wallet>(), Arg.Any<CancellationToken>())
      .Throws(exception);

    // act
    var actualResponse = await _sut.HandleAsync(command, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.HasValue.Should().BeFalse();
    actualResponse.MatchNone(x => x.Error.Should().Be(expectedError));
  }

  [Fact]
  public async Task Then_should_contribute_given_valid_command()
  {
    // arrange
    var walletId = WalletId.Create(_fixture.Create<Guid>());
    var userId = UserId.Create(_fixture.Create<Guid>());
    var contributorId = UserId.Create(_fixture.Create<Guid>());
    var amount = _fixture.Create<decimal>();
    var currency = Currency.GBP.Code;
    var walletAmount = Money.Create(amount, Currency.GBP, Units.Major);
    var wallet = Wallet.Create(walletId, userId, walletAmount);

    var command = new ContributeWalletCommand
    {
      WalletId = walletId.ToString(),
      Shares = new[]
      {
        new ContributeShareCommand
        {
          Amount = amount,
          Currency = currency,
          ContributorId = contributorId.ToString()
        }
      },
      CreatedOrModifiedBy = "TestSystem",
      CreatedOrModifiedOn = DateTime.UtcNow
    };

    _contributeWalletCommandValidatorMock
      .ValidateAsync(Arg.Any<ContributeWalletCommand>())
      .Returns(new ValidationResult());

    _walletRepositoryMock
      .FindByIdAsync(Arg.Any<WalletId>(), Arg.Any<CancellationToken>())
      .Returns(wallet);

    _walletRepositoryMock
      .UpdateAsync(Arg.Any<Wallet>(), Arg.Any<CancellationToken>())
      .Returns(wallet);

    // act
    var actualResponse = await _sut.HandleAsync(command, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.HasValue.Should().BeFalse();
  }
}
