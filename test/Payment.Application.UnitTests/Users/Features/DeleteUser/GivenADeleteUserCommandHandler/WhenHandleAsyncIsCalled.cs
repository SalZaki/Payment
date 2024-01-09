using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Optional;
using Optional.Unsafe;
using Payment.Application.UnitTests.Helpers;
using Payment.Application.Users.Features.DeleteUser;
using Payment.Application.Users.Repositories;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.Application.UnitTests.Users.Features.DeleteUser.GivenADeleteUserCommandHandler;

/// <summary>
/// Could filter by `dotnet test --filter "Category=DeleteUserCommandHandler"` in running tests in command line
/// </summary>
[Trait("Category", "DeleteUserCommandHandler")]
public sealed class WhenHandleAsyncIsCalled
{
  private readonly Fixture _fixture;
  private readonly IUserRepository _userRepositoryMock;
  private readonly IValidator<DeleteUserCommand> _deleteUserCommandValidatorMock;
  private readonly ICommandHandler<DeleteUserCommand, Option<ValidationErrorResponse, ErrorResponse>> _sut;

  public WhenHandleAsyncIsCalled()
  {
    _fixture = new Fixture();
    _fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
    _userRepositoryMock = Substitute.For<IUserRepository>();
    _deleteUserCommandValidatorMock = Substitute.For<IValidator<DeleteUserCommand>>();
    _sut = new DeleteUserCommandHandler(_userRepositoryMock, _deleteUserCommandValidatorMock);
  }

  [Fact]
  public void Then_should_have_constructor_guard_clauses()
  {
    // arrange
    var assertion = new GuardClauseAssertion(_fixture);

    // act
    assertion.Verify(typeof(DeleteUserCommand).GetConstructors());
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
    var command = _fixture.Create<DeleteUserCommand>();

    var expectedErrorMessage = "some-error-message";

    var validationResult = new ValidationResult
    {
      Errors = {new ValidationFailure("some-property", expectedErrorMessage)}
    };

    _deleteUserCommandValidatorMock
      .ValidateAsync(Arg.Any<DeleteUserCommand>(),Arg.Any<CancellationToken>())
      .Returns(validationResult);

    // act
    var actualResponse = await _sut.HandleAsync(command, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.HasValue.Should().BeTrue();
    actualResponse.ValueOrDefault().Should().BeOfType<ValidationErrorResponse>();
    actualResponse.ValueOrDefault().Errors.Should().HaveCount(1);
    actualResponse.ValueOrDefault().Errors.Any(x => x.Contains(expectedErrorMessage)).Should().BeTrue();
    await _deleteUserCommandValidatorMock.Received(1).ValidateAsync(command);
  }

  [Theory]
  [InlineData(typeof(Exception), "general exception")]
  [InlineData(typeof(ArgumentException), "argument exception")]
  [InlineData(typeof(InvalidOperationException), "invalid operation exception")]
  public async Task Then_should_handle_mapper_exceptions_given_unexpected_errors(Type type, string errorMessage)
  {
    // arrange
    var userId = _fixture.Create<UserId>();

    var command = new DeleteUserCommand
    {
      UserId = userId.ToString()
    };

    var expectedError = $"Failed to delete user due to {errorMessage}.";

    var exception = ExceptionHelper.CreateExceptionBy(type, errorMessage) ?? new Exception(errorMessage);

    _deleteUserCommandValidatorMock
      .ValidateAsync(Arg.Any<DeleteUserCommand>(), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult());

    _userRepositoryMock
      .DeleteByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
      .Throws(exception);

    // act
    var actualResponse = await _sut.HandleAsync(command, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.HasValue.Should().BeFalse();
    var errorResponse = null as ErrorResponse;
    actualResponse.MatchNone(x => errorResponse = x);
    errorResponse?.Should().NotBeNull();
    errorResponse?.Error.Should().Be(expectedError);
  }


  [Fact]
  public async Task Then_should_succeed_given_valid_command()
  {
    // arrange
    var userId = _fixture.Create<UserId>();

    var query = new DeleteUserCommand
    {
      UserId = userId.ToString()
    };

    _deleteUserCommandValidatorMock
      .ValidateAsync(Arg.Any<DeleteUserCommand>(), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult());

    _userRepositoryMock
      .DeleteByIdAsync(userId, Arg.Any<CancellationToken>())
      .Returns(Task.CompletedTask);

    // act
    var actualResponse = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.HasValue.Should().BeFalse();
  }
}
