using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Payment.Application.Users.Features.AddFriendship;
using Payment.Application.Users.Repositories;
using Payment.Common.Abstraction.Models;
using OneOf;
using Payment.Application.UnitTests.Helpers;
using Payment.Common.Abstraction.Commands;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.Application.UnitTests.Users.Features.AddFriendship.GivenAAddFriendCommandHandler;

/// <summary>
/// Could filter by `dotnet test --filter "Category=AddFriendshipCommandHandler"` in running tests in command line
/// </summary>
[Trait("Category", "AddFriendshipCommandHandler")]
public sealed class WhenHandleAsyncIsCalled
{
  private readonly Fixture _fixture;
  private readonly IUserRepository _userRepositoryMock;
  private readonly IValidator<AddFriendshipCommand> _addFriendshipCommandValidatorMock;
  private readonly ICommandHandler<AddFriendshipCommand, OneOf<AddFriendshipResponse, ValidationErrorResponse, ErrorResponse>> _sut;

  public WhenHandleAsyncIsCalled()
  {
    _fixture = new Fixture();
    _fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
    _userRepositoryMock = Substitute.For<IUserRepository>();
    _addFriendshipCommandValidatorMock = Substitute.For<IValidator<AddFriendshipCommand>>();
    _sut = new AddFriendshipCommandHandler(_userRepositoryMock, _addFriendshipCommandValidatorMock);
  }

  [Fact]
  public void Then_should_have_constructor_guard_clauses()
  {
    // arrange
    var assertion = new GuardClauseAssertion(_fixture);

    // act
    assertion.Verify(typeof(AddFriendshipCommand).GetConstructors());
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
    var command = _fixture.Create<AddFriendshipCommand>();

    var expectedErrorMessage = "some-error-message";

    var validationResult = new ValidationResult
    {
      Errors = {new ValidationFailure("some-property", expectedErrorMessage)}
    };

    _addFriendshipCommandValidatorMock
      .ValidateAsync(Arg.Any<AddFriendshipCommand>())
      .Returns(validationResult);

    // act
    var actualResponse = await _sut.HandleAsync(command, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.IsT1.Should().BeTrue();
    actualResponse.Value.Should().BeOfType<ValidationErrorResponse>();
    actualResponse.AsT1.Errors.Should().HaveCount(1);
    actualResponse.AsT1.Errors.Any(x => x.Contains(expectedErrorMessage)).Should().BeTrue();
    await _addFriendshipCommandValidatorMock.Received(1).ValidateAsync(command);
  }

  [Theory]
  [InlineData(typeof(Exception), "general exception")]
  [InlineData(typeof(ArgumentException), "argument exception")]
  [InlineData(typeof(InvalidOperationException), "invalid operation exception")]
  public async Task Then_should_handle_mapper_exceptions_given_unexpected_errors(Type type, string errorMessage)
  {
    // arrange
    var userId1 = UserId.Create(_fixture.Create<Guid>());
    var userId2 = UserId.Create(_fixture.Create<Guid>());

    var command = new AddFriendshipCommand
    {
      UserId = userId1.ToString(),
      FriendId = userId2.ToString()
    };

    var expectedError = $"Failed to add friend due to {errorMessage}.";

    var exception = ExceptionHelper.CreateExceptionBy(type, errorMessage) ?? new Exception(errorMessage);

    _addFriendshipCommandValidatorMock
      .ValidateAsync(Arg.Any<AddFriendshipCommand>())
      .Returns(new ValidationResult());

    _userRepositoryMock
      .FindByIdAsync(Arg.Any<UserId>())
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
  public async Task Then_should_return_error_response_given_user_not_found()
  {
    // arrange
    var userId = UserId.Create(_fixture.Create<Guid>());
    var friendId = UserId.Create(_fixture.Create<Guid>());

    var query = new AddFriendshipCommand
    {
      UserId = userId.ToString(),
      FriendId = friendId.ToString(),
    };

    var expectedResponse = new ErrorResponse { Error = $"Failed to get user with id {query.UserId}." };

    _addFriendshipCommandValidatorMock
      .ValidateAsync(Arg.Any<AddFriendshipCommand>(), CancellationToken.None)
      .Returns(new ValidationResult());

    _userRepositoryMock
      .FindByIdAsync(Arg.Any<UserId>(), CancellationToken.None)
      .Returns(User.NotFound);

    // act
    var actualResponse = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.Value.Should().BeOfType<ErrorResponse>();
    actualResponse.IsT2.Should().BeTrue();
    actualResponse.AsT2.Should().Be(expectedResponse);
  }

  [Fact]
  public async Task Then_should_return_add_friendship_response_given_valid_command()
  {
    // arrange
    var user = _fixture.Create<User>();
    var friend = _fixture.Create<User>();

    var query = new AddFriendshipCommand
    {
      UserId = user.Id.ToString(),
      FriendId = friend.Id.ToString(),
    };

    var expectedResponse = new AddFriendshipResponse
    {
      FriendId = friend.Id.ToString()
    };

    _addFriendshipCommandValidatorMock
      .ValidateAsync(Arg.Any<AddFriendshipCommand>(), CancellationToken.None)
      .Returns(new ValidationResult());

    _userRepositoryMock
      .FindByIdAsync(user.Id, CancellationToken.None)
      .Returns(user);

    _userRepositoryMock
      .FindByIdAsync(friend.Id, CancellationToken.None)
      .Returns(friend);

    // act
    var actualResponse = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.Value.Should().BeOfType<AddFriendshipResponse>();
    actualResponse.IsT0.Should().BeTrue();
    actualResponse.AsT0.Should().NotBeNull();
    actualResponse.AsT0.Should().Be(expectedResponse);
  }
}
