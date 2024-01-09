using FluentAssertions;
using Payment.Application.AcceptanceTests.Contexts;
using Payment.Application.Users.Features.CreateUser;
using Payment.Common.Abstraction.Models;
using TechTalk.SpecFlow;
using OneOf;
using Payment.Domain.Entities;

namespace Payment.Application.AcceptanceTests.Steps.Users;

[Binding]
[Scope(Feature = "Create User")]
public class CreateUserHappyPaths(ScenarioContext scenarioContext, TestContext testContext)
{
  private const string FullNameKey = "FullName";
  private const string ResponseKey = "Response";
  private const string CreateUserResponseKey = "CreateUserResponse";

  [Given(@"a fullname ""(.*)""")]
  public void GivenAFullname(string fullName)
  {
    scenarioContext.Add(FullNameKey, fullName);
  }

  [Given("the user does not exist in the system")]
  public async Task GivenTheUserDoesNotExistInTheSystem()
  {
    var fullName = scenarioContext.Get<string>(FullNameKey);
    fullName.Should().NotBeEmpty();

    var user = await testContext.UserRepository.FindOneAsync(x => x.FullName == fullName, CancellationToken.None);

    user.Should().Be(User.NotFound);
  }

  [When("I submit the request to create the user")]
  public async Task WhenISubmitTheRequestToCreateTheUser()
  {
    var fullName = scenarioContext.Get<string>(FullNameKey);
    fullName.Should().NotBeEmpty();

    var createUserRequest = new CreateUserCommand
    {
      FullName = fullName
    };

    var response = await testContext.UserService.CreateUserAsync(createUserRequest, CancellationToken.None);

    scenarioContext.Add(ResponseKey, response);
  }

  [Then("the response should be successful")]
  public void ThenTheResponseShouldBeSuccessful()
  {
    var response = scenarioContext.Get<OneOf<CreateUserResponse, ValidationErrorResponse, ErrorResponse>>(ResponseKey);
    response.Should().NotBeNull();
    response.Value.Should().BeOfType<CreateUserResponse>();
    response.IsT0.Should().BeTrue();

    scenarioContext.Add(CreateUserResponseKey, response.AsT0);
  }

  [Then("the response should have a newly created user id")]
  public void ThenTheResponseShouldHaveANewlyCreatedUserId()
  {
    var response = scenarioContext.Get<CreateUserResponse>(CreateUserResponseKey);
    response.UserId.Should().NotBeNullOrEmpty();
  }
}
