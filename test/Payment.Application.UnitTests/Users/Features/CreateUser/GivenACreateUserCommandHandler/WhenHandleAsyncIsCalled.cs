using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Payment.Application.Users.Features.CreateUser;
using Payment.Application.Users.Repositories;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using OneOf;
using Payment.Application.UnitTests.Helpers;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.Application.UnitTests.Users.Features.CreateUser.GivenACreateUserCommandHandler;

/// <summary>
/// Could filter by `dotnet test --filter "Category=CreateUserCommandHandler"` in running tests in command line
/// </summary>
[Trait("Category", "CreateUserCommandHandler")]
public sealed class WhenHandleAsyncIsCalled
{
  private readonly Fixture _fixture;
  private readonly IUserRepository _userRepositoryMock;
  private readonly IValidator<CreateUserCommand> _createUserCommandValidatorMock;
  private readonly ICommandHandler<CreateUserCommand, OneOf<CreateUserResponse, ValidationErrorResponse, ErrorResponse>> _sut;

  public WhenHandleAsyncIsCalled()
  {
    _fixture = new Fixture();
    _fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
    _userRepositoryMock = Substitute.For<IUserRepository>();
    _createUserCommandValidatorMock = Substitute.For<IValidator<CreateUserCommand>>();
    _sut = new CreateUserCommandHandler(_userRepositoryMock, _createUserCommandValidatorMock);
  }

  [Fact]
  public void Then_should_have_constructor_guard_clauses()
  {
    // arrange
    var assertion = new GuardClauseAssertion(_fixture);

    // act
    assertion.Verify(typeof(CreateUserCommand).GetConstructors());
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
    var command = _fixture.Create<CreateUserCommand>();

    var expectedErrorMessage = "some-error-message";

    var validationResult = new ValidationResult
    {
      Errors = {new ValidationFailure("some-property", expectedErrorMessage)}
    };

    _createUserCommandValidatorMock
      .ValidateAsync(Arg.Any<CreateUserCommand>(), CancellationToken.None)
      .Returns(validationResult);

    // act
    var response = await _sut.HandleAsync(command, CancellationToken.None);

    // assert
    response.Should().NotBeNull();
    response.IsT1.Should().BeTrue();
    response.Value.Should().BeOfType<ValidationErrorResponse>();
    response.AsT1.Errors.Should().HaveCount(1);
    response.AsT1.Errors.Any(x => x.Contains(expectedErrorMessage)).Should().BeTrue();
    await _createUserCommandValidatorMock.Received(1).ValidateAsync(command);
  }

  [Theory]
  [InlineData(typeof(Exception), "general exception")]
  [InlineData(typeof(ArgumentException), "argument exception")]
  [InlineData(typeof(InvalidOperationException), "invalid operation exception")]
  public async Task Then_should_handle_mapper_exceptions_given_unexpected_errors(Type type, string errorMessage)
  {
    // arrange
    var fullName = _fixture.Create<string>();

    var command = new CreateUserCommand
    {
      FullName = fullName
    };

    var expectedError = $"Failed to create user due to {errorMessage}.";

    var exception = ExceptionHelper.CreateExceptionBy(type, errorMessage) ?? new Exception(errorMessage);

    _createUserCommandValidatorMock
      .ValidateAsync(Arg.Any<CreateUserCommand>(), CancellationToken.None)
      .Returns(new ValidationResult());

    _userRepositoryMock
      .AddAsync(Arg.Any<User>(), CancellationToken.None)
      .Throws(exception);

    // act
    var response = await _sut.HandleAsync(command, CancellationToken.None);

    // assert
    response.Should().NotBeNull();
    response.Value.Should().BeOfType<ErrorResponse>();
    response.IsT2.Should().BeTrue();
    response.AsT2.Error.Should().Be(expectedError);
  }

  [Fact]
  public async Task Then_should_return_create_user_response_given_valid_command()
  {
    // arrange
    var fullName = _fixture.Create<FullName>();
    var userId = _fixture.Create<UserId>();
    var user = User.Create(userId, fullName);

    var command = new CreateUserCommand
    {
      FullName = fullName
    };

    _createUserCommandValidatorMock
      .ValidateAsync(Arg.Any<CreateUserCommand>(), CancellationToken.None)
      .Returns(new ValidationResult());

    _userRepositoryMock
      .AddAsync(Arg.Any<User>(), CancellationToken.None)
      .Returns(user);

    // act
    var actualResponse = await _sut.HandleAsync(command, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.Value.Should().BeOfType<CreateUserResponse>();
    actualResponse.IsT0.Should().BeTrue();
    actualResponse.AsT0.Should().NotBeNull();
    actualResponse.AsT0.UserId.Should().NotBeEmpty();
  }
}
