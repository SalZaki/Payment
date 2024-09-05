#=======================================================================================================================
Feature: Add Friendship
![User] (../assets/user.svg)

    In order to support users in the system

    As a user service

    I want to facilitate the addition of friendships

    Background:
      Given following users in the system
        | UserId                               | FullName         |
        | 73AC6B13-780F-4395-B468-506904422719 | John Doe         |
        | FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF | Samantha James   |
        | 639A1CDD-7915-47E5-AA2B-263873542B28 | Dr Andrew Martin |

    Rule: Add Friend Requirements
      - User must exist in the system.
      - Friend must exist in the system.
#-----------------------------------------------------------------------------------------------------------------------
    @user
    @happy-paths
    Scenario Outline: Add Friendship
      Given a user id <UserId>
      And a friend id <FriendId>
      When I submit the request to add friendship
      Then the response should be successful
      And the response should have a newly created friend id
      And the friend should exit in the friendships list
      And both should be mutual friends

    Examples:
      | UserId                               | FriendId                             |
      | 73AC6B13-780F-4395-B468-506904422719 | 639A1CDD-7915-47E5-AA2B-263873542B28 |
#-----------------------------------------------------------------------------------------------------------------------
