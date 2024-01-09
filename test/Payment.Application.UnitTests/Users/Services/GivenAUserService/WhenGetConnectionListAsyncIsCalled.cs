using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using NSubstitute;
using Optional;
using Payment.Application.Users.Features.AddFriendship;
using Payment.Application.Users.Features.CreateUser;
using Payment.Application.Users.Features.DeleteUser;
using Payment.Application.Users.Features.GetCommonFriends;
using Payment.Application.Users.Features.GetConnectionList;
using Payment.Application.Users.Features.GetUser;
using Payment.Application.Users.Services;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;
using OneOf;
using Xunit;

namespace Payment.Application.UnitTests.Users.Services.GivenAUserService;

/// <summary>
/// Could filter by `dotnet test --filter "Category=WalletService.GetConnectionListAsync"` in running tests in command line
/// </summary>
[Trait("Category", "WalletService.GetConnectionListAsync")]
public class WhenGetConnectionListAsyncIsCalled
{
  private readonly IUserService _sut;
  private readonly Fixture _fixture;
  private readonly IQueryHandler<GetConnectionListQuery, OneOf<IReadOnlyCollection<GetConnectionListResponse>, ValidationErrorResponse, ErrorResponse>> _getConnectionListQueryHandlerMock;

  public WhenGetConnectionListAsyncIsCalled()
  {
    _fixture = new Fixture();
    _fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
    _getConnectionListQueryHandlerMock = Substitute.For<IQueryHandler<GetConnectionListQuery, OneOf<IReadOnlyCollection<GetConnectionListResponse>, ValidationErrorResponse, ErrorResponse>>>();

    _sut = new UserService(
      Substitute.For<IQueryHandler<GetUserQuery, OneOf<GetUserResponse, ValidationErrorResponse, ErrorResponse>>>(),
      Substitute.For<ICommandHandler<CreateUserCommand, OneOf<CreateUserResponse, ValidationErrorResponse, ErrorResponse>>>(),
      Substitute.For<ICommandHandler<AddFriendshipCommand, OneOf<AddFriendshipResponse, ValidationErrorResponse, ErrorResponse>>>(),
      Substitute.For<IQueryHandler<GetCommonFriendsQuery, OneOf<IReadOnlyCollection<GetCommonFriendsResponse>, ValidationErrorResponse, ErrorResponse>>>(),
      Substitute.For<ICommandHandler<DeleteUserCommand, Option<ValidationErrorResponse, ErrorResponse>>>(),
      _getConnectionListQueryHandlerMock);
  }

  [Fact]
  public void Then_should_have_constructor_guard_clauses()
  {
    // arrange
    var assertion = new GuardClauseAssertion(_fixture);

    // act
    assertion.Verify(typeof(UserService).GetConstructors());
  }

  [Fact]
  public async Task Then_should_throw_exception_given_null_query()
  {
    // act & assert
    await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetConnectionListAsync(null!, CancellationToken.None));
  }

  [Fact]
  public async Task Then_should_handle_query()
  {
    // arrange
    var query = _fixture.Create<GetConnectionListQuery>();

    _getConnectionListQueryHandlerMock.HandleAsync(Arg.Any<GetConnectionListQuery>(), Arg.Any<CancellationToken>())
      .Returns(new OneOf<IReadOnlyCollection<GetConnectionListResponse>, ValidationErrorResponse, ErrorResponse>());

    // act
    await _sut.GetConnectionListAsync(query, CancellationToken.None);

    // assert
    await _getConnectionListQueryHandlerMock.Received(1).HandleAsync(query);
  }
}
