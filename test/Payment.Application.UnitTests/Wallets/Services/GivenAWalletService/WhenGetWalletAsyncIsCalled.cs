using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using NSubstitute;
using OneOf;
using Optional;
using Payment.Application.Wallets.Features.ContributeWallet;
using Payment.Application.Wallets.Features.CreateWallet;
using Payment.Application.Wallets.Features.GetWallet;
using Payment.Application.Wallets.Services;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;
using Xunit;

namespace Payment.Application.UnitTests.Wallets.Services.GivenAWalletService;

/// <summary>
/// Could filter by `dotnet test --filter "Category=WalletService.GetWalletAsync"` in running tests in command line
/// </summary>
[Trait("Category", "WalletService.GetWalletAsync")]
public class WhenGetWalletAsyncIsCalled
{
  private readonly IWalletService _sut;
  private readonly Fixture _fixture;
  private readonly IQueryHandler<GetWalletQuery, OneOf<GetWalletResponse, ValidationErrorResponse, ErrorResponse>> _getWalletQueryHandler;

  public WhenGetWalletAsyncIsCalled()
  {
    _fixture = new Fixture();
    _fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
    _getWalletQueryHandler = Substitute.For<IQueryHandler<GetWalletQuery, OneOf<GetWalletResponse, ValidationErrorResponse, ErrorResponse>>>();

    _sut = new WalletService(
      Substitute.For<ICommandHandler<CreateWalletCommand, OneOf<CreateWalletResponse, ValidationErrorResponse, ErrorResponse>>>(),
      _getWalletQueryHandler,
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
  public async Task Then_should_throw_exception_given_null_query()
  {
    // act & assert
    await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetWalletAsync(null!));
  }

  [Fact]
  public async Task Then_should_handle_query()
  {
    // arrange
    var query = _fixture.Create<GetWalletQuery>();

    _getWalletQueryHandler.HandleAsync(Arg.Any<GetWalletQuery>(), Arg.Any<CancellationToken>())
      .Returns(new OneOf<GetWalletResponse, ValidationErrorResponse, ErrorResponse>());

    // act
    await _sut.GetWalletAsync(query, CancellationToken.None);

    // assert
    await _getWalletQueryHandler.Received(1).HandleAsync(query);
  }
}
