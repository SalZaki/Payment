using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using NSubstitute;
using Payment.Application.Users.Features.AddFriendship;
using Payment.Application.Users.Features.CreateUser;
using Payment.Application.Users.Features.GetCommonFriends;
using Payment.Application.Users.Features.GetConnectionList;
using Payment.Application.Users.Features.GetUser;
using Payment.Application.Users.Services;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;
using OneOf;
using Optional;
using Payment.Application.Users.Features.DeleteUser;
using Xunit;

namespace Payment.Application.UnitTests.Users.Services.GivenAUserService;

/// <summary>
/// Could filter by `dotnet test --filter "Category=WalletService.AddFriendAsync"` in running tests in command line
/// </summary>
[Trait("Category", "WalletService.AddFriendAsync")]
public class WhenAddFriendshipAsyncIsCalled
{
  private readonly IUserService _sut;
  private readonly Fixture _fixture;
  private readonly ICommandHandler<AddFriendshipCommand, OneOf<AddFriendshipResponse, ValidationErrorResponse, ErrorResponse>> _addFriendshipCommandHandlerMock;

  public WhenAddFriendshipAsyncIsCalled()
  {
    _fixture = new Fixture();
    _fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
    _addFriendshipCommandHandlerMock = Substitute.For<ICommandHandler<AddFriendshipCommand, OneOf<AddFriendshipResponse, ValidationErrorResponse, ErrorResponse>>>();

    _sut = new UserService(
      Substitute.For<IQueryHandler<GetUserQuery, OneOf<GetUserResponse, ValidationErrorResponse, ErrorResponse>>>(),
      Substitute.For<ICommandHandler<CreateUserCommand, OneOf<CreateUserResponse, ValidationErrorResponse, ErrorResponse>>>(),
      _addFriendshipCommandHandlerMock,
      Substitute.For<IQueryHandler<GetCommonFriendsQuery, OneOf<IReadOnlyCollection<GetCommonFriendsResponse>, ValidationErrorResponse, ErrorResponse>>>(),
      Substitute.For<ICommandHandler<DeleteUserCommand, Option<ValidationErrorResponse, ErrorResponse>>>(),
      Substitute.For<IQueryHandler<GetConnectionListQuery, OneOf<IReadOnlyCollection<GetConnectionListResponse>, ValidationErrorResponse, ErrorResponse>>>());
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
  public async Task Then_should_throw_exception_given_null_command()
  {
    // act & assert
    await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.AddFriendAsync(null!, CancellationToken.None));
  }

  [Fact]
  public async Task Then_should_handle_command()
  {
    // arrange
    var command = _fixture.Create<AddFriendshipCommand>();

    _addFriendshipCommandHandlerMock.HandleAsync(Arg.Any<AddFriendshipCommand>(), Arg.Any<CancellationToken>())
      .Returns(new OneOf<AddFriendshipResponse, ValidationErrorResponse, ErrorResponse>());

    // act
    await _sut.AddFriendAsync(command, CancellationToken.None);

    // assert
    await _addFriendshipCommandHandlerMock.Received(1).HandleAsync(command);
  }
}
