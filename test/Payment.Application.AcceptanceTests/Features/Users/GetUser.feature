#=============================================
Feature: Get User
![User] (../../../assets/user.svg)

    In order to support users in the system

    As a user service

    I want to facilitate the retrieval of users

    Background:
      Given following users in the system
        | UserId                               | FullName          |
        | 73AC6B13-780F-4395-B468-506904422719 | John Doe          |
        | FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF | Guillaume Reynard |
        | 639A1CDD-7915-47E5-AA2B-263873542B28 | Dr Andrew Martin  |

    Rule: User Retrieval Requirements
      - A user must exist in the system.
      - A user can contain no friends.
#-----------------------------------------------------------------------------------------------------------------------
    @user
    @happy-paths
    Scenario Outline: Get a user
      Given a user with id <UserId>
      When I submit the query to get the user
      Then the response should be successful
      And the response should be a user with id <UserId>

    Examples:
      | UserId                               |
      | 73AC6B13-780F-4395-B468-506904422719 |
#-----------------------------------------------------------------------------------------------------------------------
    @user
    @happy-paths
    Scenario Outline: Get a user with friends
      Given a user with id <UserId>
      And following friendships for the user exist in the system
        | UserId                               | FriendId                             |
        | 73AC6B13-780F-4395-B468-506904422719 | FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF |
        | 73AC6B13-780F-4395-B468-506904422719 | 639A1CDD-7915-47E5-AA2B-263873542B28 |
      When I submit the query to get the user
      Then the response should be successful
      And the response should be a user with id <UserId>
      And the user should have a friend with id <FriendId> and fullname <FriendFullName>

      Examples:
        | UserId                               | FriendId                             | FriendFullName      |
        | 73AC6B13-780F-4395-B468-506904422719 | FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF | "Guillaume Reynard" |
        | 73AC6B13-780F-4395-B468-506904422719 | 639A1CDD-7915-47E5-AA2B-263873542B28 | "Dr Andrew Martin"  |
#-----------------------------------------------------------------------------------------------------------------------
