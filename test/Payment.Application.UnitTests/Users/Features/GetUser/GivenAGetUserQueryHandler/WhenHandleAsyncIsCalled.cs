using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Payment.Application.Users.Features.GetUser;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;
using Payment.Common.Mappers;
using Payment.Domain.Entities;
using OneOf;
using Payment.Application.UnitTests.Helpers;
using Payment.Application.Users.Repositories;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.Application.UnitTests.Users.Features.GetUser.GivenAGetUserQueryHandler;

/// <summary>
/// Could filter by `dotnet test --filter "Category=GetUserQueryHandler"` in running tests in command line
/// </summary>
[Trait("Category", "GetUserQueryHandler")]
public sealed class WhenHandleAsyncIsCalled
{
  private readonly Fixture _fixture;
  private readonly IUserRepository _userRepositoryMock;
  private readonly IValidator<GetUserQuery> _getUserQueryValidatorMock;
  private readonly IMapper<User, GetUserResponse> _userToGetUserResponseMapperMock;
  private readonly IQueryHandler<GetUserQuery, OneOf<GetUserResponse, ValidationErrorResponse, ErrorResponse>> _sut;

  public WhenHandleAsyncIsCalled()
  {
    _fixture = new Fixture();
    _fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
    _userRepositoryMock = Substitute.For<IUserRepository>();
    _userToGetUserResponseMapperMock = Substitute.For<IMapper<User, GetUserResponse>>();
    _getUserQueryValidatorMock = Substitute.For<IValidator<GetUserQuery>>();
    _sut = new GetUserQueryHandler(_userRepositoryMock, _getUserQueryValidatorMock, _userToGetUserResponseMapperMock);
  }

  [Fact]
  public void Then_should_have_constructor_guard_clauses()
  {
    // arrange
    var assertion = new GuardClauseAssertion(_fixture);

    // act
    assertion.Verify(typeof(GetUserQueryHandler).GetConstructors());
  }

  [Fact]
  public async Task Then_should_throw_exception_given_null_query()
  {
    // act & assert
    await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.HandleAsync(null!, CancellationToken.None));
  }

  [Fact]
  public async Task Then_should_handle_validation_errors_given_invalid_query()
  {
    // arrange
    var query = _fixture.Create<GetUserQuery>();

    var expectedErrorMessage = "some-error-message";

    var validationResult = new ValidationResult
    {
      Errors = {new ValidationFailure("some-property", expectedErrorMessage)}
    };

    _getUserQueryValidatorMock
      .ValidateAsync(Arg.Any<GetUserQuery>())
      .Returns(validationResult);

    // act
    var response = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    response.Should().NotBeNull();
    response.IsT1.Should().BeTrue();
    response.Value.Should().BeOfType<ValidationErrorResponse>();
    response.AsT1.Errors.Should().HaveCount(1);
    response.AsT1.Errors.Any(x => x.Contains(expectedErrorMessage)).Should().BeTrue();
    await _getUserQueryValidatorMock.Received(1).ValidateAsync(query);
  }

  [Theory]
  [InlineData(typeof(Exception), "general exception")]
  [InlineData(typeof(ArgumentException), "argument exception")]
  [InlineData(typeof(InvalidOperationException), "invalid operation exception")]
  public async Task Then_should_handle_mapper_exceptions_given_unexpected_errors(Type type, string errorMessage)
  {
    // arrange
    var userId = UserId.Create(_fixture.Create<Guid>());

    var query = new GetUserQuery
    {
      UserId = userId.ToString()
    };

    var expectedError = $"Failed to get user with id {query.UserId} due to {errorMessage}.";

    var exception = ExceptionHelper.CreateExceptionBy(type, errorMessage) ?? new Exception(errorMessage);

    _getUserQueryValidatorMock
      .ValidateAsync(Arg.Any<GetUserQuery>())
      .Returns(new ValidationResult());

    _userToGetUserResponseMapperMock
      .Map(Arg.Any<User>())
      .Throws(exception);

    // act
    var response = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    response.Should().NotBeNull();
    response.Value.Should().BeOfType<ErrorResponse>();
    response.IsT2.Should().BeTrue();
    response.AsT2.Error.Should().Be(expectedError);
    _userToGetUserResponseMapperMock.Received(1).Map(Arg.Any<User>());
  }

  [Fact]
  public async Task Then_should_return_error_response_given_user_not_found()
  {
    // arrange
    var userId = UserId.Create(_fixture.Create<Guid>());

    var query = new GetUserQuery
    {
      UserId = userId.ToString()
    };

    var expectedResponse = new ErrorResponse { Error = $"Failed to get user with id {query.UserId}." };

    _getUserQueryValidatorMock
      .ValidateAsync(Arg.Any<GetUserQuery>(), CancellationToken.None)
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
  public async Task Then_should_return_get_user_response_given_valid_query()
  {
    // arrange
    var userId = UserId.Create(_fixture.Create<Guid>());
    var fullName = FullName.Create("Tom Jones");
    var user = User.Create(userId, fullName);

    var query = new GetUserQuery
    {
      UserId = userId.ToString()
    };

    var expectedResponse = new GetUserResponse
    {
      UserId = userId.ToString(),
      FullName = fullName.Value,
    };

    _getUserQueryValidatorMock
      .ValidateAsync(Arg.Any<GetUserQuery>(), CancellationToken.None)
      .Returns(new ValidationResult());

    _userRepositoryMock
      .FindByIdAsync(Arg.Any<UserId>(), CancellationToken.None)
      .Returns(user);

    _userToGetUserResponseMapperMock
      .Map(Arg.Any<User>())
      .Returns(expectedResponse);

    // act
    var actualResponse = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.Value.Should().BeOfType<GetUserResponse>();
    actualResponse.IsT0.Should().BeTrue();
    actualResponse.AsT0.Should().Be(expectedResponse);
  }
}
