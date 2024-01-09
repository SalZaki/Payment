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
/// Could filter by `dotnet test --filter "Category=WalletService.WhenCreateUserAsyncIsCalled"` in running tests in command line
/// </summary>
[Trait("Category", "WalletService.WhenCreateUserAsyncIsCalled")]
public class WhenCreateUserAsyncIsCalled
{
  private readonly IUserService _sut;
  private readonly Fixture _fixture;
  private readonly ICommandHandler<CreateUserCommand, OneOf<CreateUserResponse, ValidationErrorResponse, ErrorResponse>> _createUserCommandHandlerMock;

  public WhenCreateUserAsyncIsCalled()
  {
    _fixture = new Fixture();
    _fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
    _createUserCommandHandlerMock = Substitute.For<ICommandHandler<CreateUserCommand, OneOf<CreateUserResponse, ValidationErrorResponse, ErrorResponse>>>();

    _sut = new UserService(
      Substitute.For<IQueryHandler<GetUserQuery, OneOf<GetUserResponse, ValidationErrorResponse, ErrorResponse>>>(),
      _createUserCommandHandlerMock,
      Substitute.For<ICommandHandler<AddFriendshipCommand, OneOf<AddFriendshipResponse, ValidationErrorResponse, ErrorResponse>>>(),
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
    await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.CreateUserAsync(null!, CancellationToken.None));
  }

  [Fact]
  public async Task Then_should_handle_command()
  {
    // arrange
    var command = _fixture.Create<CreateUserCommand>();

    _createUserCommandHandlerMock.HandleAsync(Arg.Any<CreateUserCommand>(), Arg.Any<CancellationToken>())
      .Returns(new OneOf<CreateUserResponse, ValidationErrorResponse, ErrorResponse>());

    // act
    await _sut.CreateUserAsync(command, CancellationToken.None);

    // assert
    await _createUserCommandHandlerMock.Received(1).HandleAsync(command);
  }
}
