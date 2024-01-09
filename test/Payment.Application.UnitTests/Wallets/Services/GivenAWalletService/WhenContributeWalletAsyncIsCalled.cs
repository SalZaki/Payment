using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using NSubstitute;
using Optional;
using Payment.Application.Wallets.Features.ContributeWallet;
using Payment.Application.Wallets.Features.CreateWallet;
using Payment.Application.Wallets.Features.GetWallet;
using Payment.Application.Wallets.Services;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;
using OneOf;
using Xunit;

namespace Payment.Application.UnitTests.Wallets.Services.GivenAWalletService;

/// <summary>
/// Could filter by `dotnet test --filter "Category=WalletService.ContributeWalletAsync"` in running tests in command line
/// </summary>
[Trait("Category", "WalletService.ContributeWalletAsync")]
public class WhenContributeWalletAsyncIsCalled
{
  private readonly IWalletService _sut;
  private readonly Fixture _fixture;
  private readonly ICommandHandler<ContributeWalletCommand, Option<ValidationErrorResponse, ErrorResponse>> _contributeWalletCommandHandlerMock;

  public WhenContributeWalletAsyncIsCalled()
  {
    _fixture = new Fixture();
    _fixture.Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });
    _contributeWalletCommandHandlerMock = Substitute.For<ICommandHandler<ContributeWalletCommand, Option<ValidationErrorResponse, ErrorResponse>>>();

    _sut = new WalletService(
      Substitute.For<ICommandHandler<CreateWalletCommand, OneOf<CreateWalletResponse, ValidationErrorResponse, ErrorResponse>>>(),
      Substitute.For<IQueryHandler<GetWalletQuery, OneOf<GetWalletResponse, ValidationErrorResponse, ErrorResponse>>>(),
      _contributeWalletCommandHandlerMock);
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
    await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ContributeWalletAsync(null!, CancellationToken.None));
  }

  [Fact]
  public async Task Then_should_handle_command()
  {
    // arrange
    var command = _fixture.Create<ContributeWalletCommand>();

    _contributeWalletCommandHandlerMock.HandleAsync(Arg.Any<ContributeWalletCommand>(), Arg.Any<CancellationToken>())
      .Returns(new Option<ValidationErrorResponse, ErrorResponse>());

    // act
    await _sut.ContributeWalletAsync(command, CancellationToken.None);

    // assert
    await _contributeWalletCommandHandlerMock.Received(1).HandleAsync(command);
  }
}
