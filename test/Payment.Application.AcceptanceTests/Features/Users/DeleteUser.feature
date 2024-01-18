#=======================================================================================================================
Feature: Delete User
![User] (../../../assets/user.svg)

    In order to support users in the system

    As a user service

    I want to facilitate the deletion of users

    Background:
      Given following users in the system
        | UserId                               | FullName       |
        | B404D785-C887-4142-A6B3-E567BB699F24 | Sarah Atkins   |
        | C5BC9FE8-AE48-4932-B7DF-ACDB64188C16 | Richard Dawson |

    Rule: Delete User Requirement(s)
      - User must exist in the system.
      - If a user is deleted, its mutual friendships should also be deleted.
#-----------------------------------------------------------------------------------------------------------------------
    @user
    @happy-paths
    Scenario Outline: Delete a user
      Given a user id <UserId>
      When I submit the request to delete the user
      Then the response should be successful

    Examples:
      | UserId                               |
      | B404D785-C887-4142-A6B3-E567BB699F24 |
#-----------------------------------------------------------------------------------------------------------------------
    @user
    @happy-paths-with-friendship
    Scenario Outline: Delete a user with friend
      Given a user id <UserId>
      And the user has the following friend in the system
        | UserId                               | FriendId                             |
        | B404D785-C887-4142-A6B3-E567BB699F24 | C5BC9FE8-AE48-4932-B7DF-ACDB64188C16 |
      When I submit the request to delete the user
      Then the response should be successful
      And the user's friendship with user id <FriendId> should also be deleted

    Examples:
      | UserId                               | FriendId                             |
      | B404D785-C887-4142-A6B3-E567BB699F24 | C5BC9FE8-AE48-4932-B7DF-ACDB64188C16 |
