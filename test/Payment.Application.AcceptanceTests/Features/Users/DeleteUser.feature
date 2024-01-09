#=======================================================================================================================
Feature: Delete User
![User] (../../../assets/user.svg)

    In order to support users in the system

    As a user service

    I want to facilitate the deletion of users

    Background:
      Given following user in the system
        | UserId                               | FullName         |
        | 73AC6B13-780F-4395-B468-506904422719 | John Doe         |

    Rule: Delete User Requirement(s)
      - User must exist in the system.
#-----------------------------------------------------------------------------------------------------------------------
    @user
    @happy-paths
    Scenario Outline: Delete a user
      Given a user id <UserId>
      When I submit the request to delete the user
      Then the response should be successful

      Examples:
      | UserId                                 |
      | "73AC6B13-780F-4395-B468-506904422719" |
