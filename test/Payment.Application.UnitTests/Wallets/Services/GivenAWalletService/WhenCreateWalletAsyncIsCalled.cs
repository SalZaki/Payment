using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using NSubstitute;
using Payment.Application.Wallets.Features.ContributeWallet;
using Payment.Application.Wallets.Features.CreateWallet;
using Payment.Application.Wallets.Features.GetWallet;
using Payment.Application.Wallets.Services;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using OneOf;
using Optional;
using Payment.Common.Abstraction.Queries;
using Xunit;

namespace Payment.Application.UnitTests.Wallets.Services.GivenAWalletService;

/// <summary>
/// Could filter by `dotnet test --filter "Category=WalletService.CreateWalletAsync"` in running tests in command line
/// </summary>
[Trait("Category", "WalletService.CreateWalletAsync")]
public class WhenCreateWalletAsyncIsCalled
{
  private readonly IWalletService _sut;
  private readonly Fixture _fixture;
  private readonly ICommandHandler<CreateWalletCommand, OneOf<CreateWalletResponse, ValidationErrorResponse, ErrorResponse>> _createWalletCommandHandler;

  public WhenCreateWalletAsyncIsCalled()
  {
    _fixture = new Fixture();
    _fixture.Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });
    _createWalletCommandHandler = Substitute.For<ICommandHandler<CreateWalletCommand, OneOf<CreateWalletResponse, ValidationErrorResponse, ErrorResponse>>>();

    _sut = new WalletService(
      _createWalletCommandHandler,
      Substitute.For<IQueryHandler<GetWalletQuery, OneOf<GetWalletResponse, ValidationErrorResponse, ErrorResponse>>>(),
      Substitute.For<ICommandHandler<ContributeWalletCommand, Option<ValidationErrorResponse, ErrorResponse>>>());
  }

  [Fact]
  public void Then_should_have_constructor_guard_clauses()
  {
    // arrange
    var assertion = new GuardClauseAssertion(_fixture);

    // act
    assertion.Verify(typeof(WalletService).GetConstructors());
  }

  [Fact]
  public async Task Then_should_throw_exception_given_null_command()
  {
    // act & assert
    await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.CreateWalletAsync(null!, CancellationToken.None));
  }

  [Fact]
  public async Task Then_should_handle_command()
  {
    // arrange
    var command = _fixture.Create<CreateWalletCommand>();

    _createWalletCommandHandler.HandleAsync(Arg.Any<CreateWalletCommand>(), Arg.Any<CancellationToken>())
      .Returns(new OneOf<CreateWalletResponse, ValidationErrorResponse, ErrorResponse>());

    // act
    await _sut.CreateWalletAsync(command, CancellationToken.None);

    // assert
    await _createWalletCommandHandler.Received(1).HandleAsync(command);
  }
}
