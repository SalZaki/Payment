using System.Collections.ObjectModel;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Payment.Application.Users.Features.GetCommonFriends;
using Payment.Application.Users.Repositories;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;
using OneOf;
using Payment.Application.UnitTests.Helpers;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.Application.UnitTests.Users.Features.GetCommonFriends.GivenAGetCommonFriendsQueryHandler;

/// <summary>
/// Could filter by `dotnet test --filter "Category=GetCommonFriendsQueryHandler"` in running tests in command line
/// </summary>
[Trait("Category", "GetCommonFriendsQueryHandler")]
public sealed class WhenHandleAsyncIsCalled
{
  private readonly Fixture _fixture;
  private readonly IUserRepository _userRepositoryMock;
  private readonly IValidator<GetCommonFriendsQuery> _getCommonFriendsQueryValidatorMock;
  private readonly IQueryHandler<GetCommonFriendsQuery, OneOf<IReadOnlyCollection<GetCommonFriendsResponse>, ValidationErrorResponse, ErrorResponse>> _sut;

  public WhenHandleAsyncIsCalled()
  {
    _fixture = new Fixture();
    _fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
    _userRepositoryMock = Substitute.For<IUserRepository>();
    _getCommonFriendsQueryValidatorMock = Substitute.For<IValidator<GetCommonFriendsQuery>>();
    _sut = new GetCommonFriendsQueryHandler(_userRepositoryMock, _getCommonFriendsQueryValidatorMock);
  }

  [Fact]
  public void Then_should_have_constructor_guard_clauses()
  {
    // arrange
    var assertion = new GuardClauseAssertion(_fixture);

    // act
    assertion.Verify(typeof(GetCommonFriendsQuery).GetConstructors());
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
    var query = _fixture.Create<GetCommonFriendsQuery>();

    var expectedErrorMessage = "some-error-message";

    var validationResult = new ValidationResult
    {
      Errors = {new ValidationFailure("some-property", expectedErrorMessage)}
    };

    _getCommonFriendsQueryValidatorMock
      .ValidateAsync(Arg.Any<GetCommonFriendsQuery>())
      .Returns(validationResult);

    // act
    var response = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    response.Should().NotBeNull();
    response.IsT1.Should().BeTrue();
    response.Value.Should().BeOfType<ValidationErrorResponse>();
    response.AsT1.Errors.Should().HaveCount(1);
    response.AsT1.Errors.Any(x => x.Contains(expectedErrorMessage)).Should().BeTrue();
    await _getCommonFriendsQueryValidatorMock.Received(1).ValidateAsync(query);
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

    var query = new GetCommonFriendsQuery
    {
      UserId1 = userId1.ToString(),
      UserId2 = userId2.ToString()
    };

    var expectedError = $"Failed to get common friends due to {errorMessage}.";

    var exception = ExceptionHelper.CreateExceptionBy(type, errorMessage) ?? new Exception(errorMessage);

    _getCommonFriendsQueryValidatorMock
      .ValidateAsync(Arg.Any<GetCommonFriendsQuery>())
      .Returns(new ValidationResult());

    _userRepositoryMock
      .FindByIdAsync(Arg.Any<UserId>())
      .Throws(exception);

    // act
    var response = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    response.Should().NotBeNull();
    response.Value.Should().BeOfType<ErrorResponse>();
    response.IsT2.Should().BeTrue();
    response.AsT2.Error.Should().Be(expectedError);
  }

  [Fact]
  public async Task Then_should_return_error_response_given_user_not_found()
  {
    // arrange
    var userId1 = UserId.Create(_fixture.Create<Guid>());
    var userId2 = UserId.Create(_fixture.Create<Guid>());

    var query = new GetCommonFriendsQuery
    {
      UserId1 = userId1.ToString(),
      UserId2 = userId2.ToString(),
    };

    var expectedResponse = new ErrorResponse { Error = $"Failed to get user with id {query.UserId1}." };

    _getCommonFriendsQueryValidatorMock
      .ValidateAsync(Arg.Any<GetCommonFriendsQuery>(), CancellationToken.None)
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
  public async Task Then_should_return_get_common_friends_response_given_valid_query()
  {
    // arrange
    var commonFriend = User.Create(_fixture.Create<UserId>(), FullName.Create("John Doe"));

    var user1 = _fixture.Create<User>();
    user1.AddFriend(commonFriend);

    var user2 = _fixture.Create<User>();
    user2.AddFriend(commonFriend);

    var query = new GetCommonFriendsQuery
    {
      UserId1 = user1.Id.ToString(),
      UserId2 = user2.Id.ToString(),
    };

    var expectedResponse = new List<GetCommonFriendsResponse>
    {
      new()
      {
        UserId = commonFriend.Id.ToString(),
        FullName = commonFriend.FullName.Value,
      }
    };

    _getCommonFriendsQueryValidatorMock
      .ValidateAsync(Arg.Any<GetCommonFriendsQuery>(), CancellationToken.None)
      .Returns(new ValidationResult());

    _userRepositoryMock
      .FindByIdAsync(user1.Id, CancellationToken.None)
      .Returns(user1);

    _userRepositoryMock
      .FindByIdAsync(user2.Id, CancellationToken.None)
      .Returns(user2);

    // act
    var actualResponse = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.Value.Should().BeOfType<ReadOnlyCollection<GetCommonFriendsResponse>>();
    actualResponse.IsT0.Should().BeTrue();
    actualResponse.AsT0.Should().NotBeNull();
    actualResponse.AsT0.Count.Should().Be(expectedResponse.Count);
    var actualCommonFriendIds = actualResponse.AsT0.Select(x => x.UserId);
    var expectedCommonFriendIds = expectedResponse.Select(x => x.UserId);
    actualCommonFriendIds.Should().Contain(expectedCommonFriendIds);
  }
}
