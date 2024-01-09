using System.Collections.ObjectModel;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Payment.Application.Users.Features.GetConnectionList;
using Payment.Application.Users.Repositories;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;
using OneOf;
using Payment.Application.UnitTests.Helpers;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.Application.UnitTests.Users.Features.GetConnectionList.GivenAGetConnectionListQueryHandler;

/// <summary>
/// Could filter by `dotnet test --filter "Category=GetConnectionListQueryHandler"` in running tests in command line
/// </summary>
[Trait("Category", "GetConnectionListQueryHandler")]
public sealed class WhenHandleAsyncIsCalled
{
  private readonly Fixture _fixture;
  private readonly IUserRepository _userRepositoryMock;
  private readonly IValidator<GetConnectionListQuery> _getConnectionListQueryValidatorMock;
  private readonly IQueryHandler<GetConnectionListQuery, OneOf<IReadOnlyCollection<GetConnectionListResponse>, ValidationErrorResponse, ErrorResponse>> _sut;

  public WhenHandleAsyncIsCalled()
  {
    _fixture = new Fixture();
    _fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
    _userRepositoryMock = Substitute.For<IUserRepository>();
    _getConnectionListQueryValidatorMock = Substitute.For<IValidator<GetConnectionListQuery>>();
    _sut = new GetConnectionListQueryHandler(_userRepositoryMock, _getConnectionListQueryValidatorMock);
  }

  [Fact]
  public void Then_should_have_constructor_guard_clauses()
  {
    // arrange
    var assertion = new GuardClauseAssertion(_fixture);

    // act
    assertion.Verify(typeof(GetConnectionListQuery).GetConstructors());
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
    var query = _fixture.Create<GetConnectionListQuery>();

    var expectedErrorMessage = "some-error-message";

    var validationResult = new ValidationResult
    {
      Errors = {new ValidationFailure("some-property", expectedErrorMessage)}
    };

    _getConnectionListQueryValidatorMock
      .ValidateAsync(Arg.Any<GetConnectionListQuery>(),Arg.Any<CancellationToken>())
      .Returns(validationResult);

    // act
    var actualResponse = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.IsT1.Should().BeTrue();
    actualResponse.Value.Should().BeOfType<ValidationErrorResponse>();
    actualResponse.AsT1.Errors.Should().HaveCount(1);
    actualResponse.AsT1.Errors.Any(x => x.Contains(expectedErrorMessage)).Should().BeTrue();
    await _getConnectionListQueryValidatorMock.Received(1).ValidateAsync(query);
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

    var query = new GetConnectionListQuery
    {
      UserId1 = userId1.ToString(),
      UserId2 = userId2.ToString()
    };

    var expectedError = $"Failed to get connection list due to {errorMessage}.";

    var exception = ExceptionHelper.CreateExceptionBy(type, errorMessage) ?? new Exception(errorMessage);

    _getConnectionListQueryValidatorMock
      .ValidateAsync(Arg.Any<GetConnectionListQuery>(), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult());

    _userRepositoryMock
      .FindByIdAsync(Arg.Any<UserId>(),Arg.Any<CancellationToken>())
      .Throws(exception);

    // act
    var actualResponse = await _sut.HandleAsync(query, CancellationToken.None);

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
    var userId1 = UserId.Create(_fixture.Create<Guid>());
    var userId2 = UserId.Create(_fixture.Create<Guid>());

    var query = new GetConnectionListQuery
    {
      UserId1 = userId1.ToString(),
      UserId2 = userId2.ToString(),
    };

    var expectedResponse = new ErrorResponse { Error = $"Failed to get user with id {query.UserId1}." };

    _getConnectionListQueryValidatorMock
      .ValidateAsync(Arg.Any<GetConnectionListQuery>(), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult());

    _userRepositoryMock
      .FindByIdAsync(userId1, Arg.Any<CancellationToken>())
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
  public async Task Then_should_return_get_connection_list_response_given_valid_query()
  {
    // arrange
    var user1 = User.Create(_fixture.Create<UserId>(), FullName.Create("John Doe"));
    var user2 = User.Create(_fixture.Create<UserId>(), FullName.Create("Amanda James"));
    var user3 = User.Create(_fixture.Create<UserId>(), FullName.Create("Martin McCole"));
    var user4 = User.Create(_fixture.Create<UserId>(), FullName.Create("Sam Johnson"));
    var user5 = User.Create(_fixture.Create<UserId>(), FullName.Create("Matt Dawson"));
    var user6 = User.Create(_fixture.Create<UserId>(), FullName.Create("Steve Smith"));

    // user1 has user3 as a friend
    user1.AddFriend(user3);

    // user3 has user4 as a friend
    user3.AddFriend(user4);

    // user4 has user5 as a friend
    user4.AddFriend(user5);

    // user5 has user6 as a friend
    user5.AddFriend(user6);

    // user6 has user2 as a friend
    user6.AddFriend(user2);

    var query = new GetConnectionListQuery
    {
      UserId1 = user1.Id.ToString(),
      UserId2 = user2.Id.ToString(),
      MaxLevel = 100
    };

    var expectedResponse = new List<GetConnectionListResponse>
    {
      new() { UserId = user1.Id.ToString(), FullName = user1.FullName.Value },
      new() { UserId = user3.Id.ToString(), FullName = user3.FullName.Value },
      new() { UserId = user4.Id.ToString(), FullName = user4.FullName.Value },
      new() { UserId = user5.Id.ToString(), FullName = user5.FullName.Value },
      new() { UserId = user6.Id.ToString(), FullName = user6.FullName.Value },
      new() { UserId = user2.Id.ToString(), FullName = user2.FullName.Value }
    };

    _getConnectionListQueryValidatorMock
      .ValidateAsync(Arg.Any<GetConnectionListQuery>(), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult());

    _userRepositoryMock
      .FindByIdAsync(user1.Id, Arg.Any<CancellationToken>())
      .Returns(user1);

    _userRepositoryMock
      .FindByIdAsync(user2.Id, Arg.Any<CancellationToken>())
      .Returns(user2);

    // act
    var actualResponse = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.Value.Should().BeOfType<ReadOnlyCollection<GetConnectionListResponse>>();
    actualResponse.IsT0.Should().BeTrue();
    actualResponse.AsT0.Should().NotBeNull();
    actualResponse.AsT0.Count.Should().Be(expectedResponse.Count);
    var actualCommonFriendIds = actualResponse.AsT0.Select(x => x.UserId);
    var expectedCommonFriendIds = expectedResponse.Select(x => x.UserId);
    actualCommonFriendIds.Should().Contain(expectedCommonFriendIds);
  }

  [Fact]
  public async Task Then_should_return_empty_get_connection_list_response_given_max_level_is_reached()
  {
    // arrange
    var user1 = User.Create(_fixture.Create<UserId>(), FullName.Create("John Doe"));
    var user2 = User.Create(_fixture.Create<UserId>(), FullName.Create("Amanda James"));
    var user3 = User.Create(_fixture.Create<UserId>(), FullName.Create("Martin McCole"));
    var user4 = User.Create(_fixture.Create<UserId>(), FullName.Create("Sam Johnson"));
    var user5 = User.Create(_fixture.Create<UserId>(), FullName.Create("Matt Dawson"));
    var user6 = User.Create(_fixture.Create<UserId>(), FullName.Create("Steve Smith"));

    // user1 has user3 as a friend
    user1.AddFriend(user3);

    // user3 has user4 as a friend
    user3.AddFriend(user4);

    // user4 has user5 as a friend
    user4.AddFriend(user5);

    // user5 has user6 as a friend
    user5.AddFriend(user6);

    // user6 has user2 as a friend
    user6.AddFriend(user2);

    var query = new GetConnectionListQuery
    {
      UserId1 = user1.Id.ToString(),
      UserId2 = user2.Id.ToString(),
      MaxLevel = 3
    };

    _getConnectionListQueryValidatorMock
      .ValidateAsync(Arg.Any<GetConnectionListQuery>(), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult());

    _userRepositoryMock
      .FindByIdAsync(user1.Id, Arg.Any<CancellationToken>())
      .Returns(user1);

    _userRepositoryMock
      .FindByIdAsync(user2.Id, Arg.Any<CancellationToken>())
      .Returns(user2);

    // act
    var actualResponse = await _sut.HandleAsync(query, CancellationToken.None);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.Value.Should().BeOfType<ReadOnlyCollection<GetConnectionListResponse>>();
    actualResponse.IsT0.Should().BeTrue();
    actualResponse.AsT0.Should().NotBeNull();
    actualResponse.AsT0.Should().BeEquivalentTo(Array.Empty<GetConnectionListResponse>());
  }
}
