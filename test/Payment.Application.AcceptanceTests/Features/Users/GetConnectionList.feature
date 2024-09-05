#=============================================
Feature: Get Connection List
![User] (../assets/user.svg)

    In order to support users in the system

    As a user service

    I want to facilitate the retrieval of connection list between two users

    Background:
      Given following users in the system
        | UserId                               | FullName         |
        | 73AC6B13-780F-4395-B468-506904422719 | John Doe         |
        | FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF | Samantha James   |
        | 639A1CDD-7915-47E5-AA2B-263873542B28 | Dr Andrew Martin |
        | 11CC3F2D-081A-451D-9387-8B8989CA9C06 | Sarah Williams   |

    Rule: Get Connection List Requirements
      - Users must exist in the system.
      - Friends must exist in the system.
#-----------------------------------------------------------------------------------------------------------------------
    @user
    @happy-paths
    Scenario Outline: Get connection list - users have no friends
      Given a user1 with id <UserId1>
      And a user2 with id <UserId2>
      And max level is <MaxLevel>
      When I submit the request to get connection list
      Then the response should be successful
      And the response should have empty connected list

      Examples:
        | UserId1                                | UserId2                                | MaxLevel |
        | "73AC6B13-780F-4395-B468-506904422719" | "FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF" | 3        |
#-----------------------------------------------------------------------------------------------------------------------
    @user
    @happy-paths
    Scenario Outline: Get connection list - users are direct friends
      Given a user1 with id <UserId1>
      And a user2 with id <UserId2>
      And the user1 has user2 as friend in the system
        | UserId                               | FriendId                             |
        | 73AC6B13-780F-4395-B468-506904422719 | FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF |
      And the user2 has user1 as friend in the system
        | UserId                               | FriendId                             |
        | FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF | 73AC6B13-780F-4395-B468-506904422719 |
      And max level is <MaxLevel>
      When I submit the request to get connection list
      Then the response should be successful
      And the response should have following connected list
        | UserId                               | FullName       |
        | 73AC6B13-780F-4395-B468-506904422719 | John Doe       |
        | FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF | Samantha James |

      Examples:
        | UserId1                                | UserId2                                | MaxLevel |
        | "73AC6B13-780F-4395-B468-506904422719" | "FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF" | 3        |
#-----------------------------------------------------------------------------------------------------------------------
    @user
    @happy-paths
    Scenario Outline: Get connection list - users have mutual friends
      Given a user1 with id <UserId1>
      And a user2 with id <UserId2>
      And max level is <MaxLevel>
      And user1 has user3 as friend in the system
        | UserId                               | FriendId                             |
        | 73AC6B13-780F-4395-B468-506904422719 | 639A1CDD-7915-47E5-AA2B-263873542B28 |
      And user3 has user4 as friend in the system
        | UserId                               | FriendId                             |
        | 639A1CDD-7915-47E5-AA2B-263873542B28 | 11CC3F2D-081A-451D-9387-8B8989CA9C06 |
      And user4 has user2 as friend in the system
        | UserId                               | FriendId                             |
        | 11CC3F2D-081A-451D-9387-8B8989CA9C06 | FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF |
      When I submit the request to get connection list
      Then the response should be successful
      And the response should have following connected list
        | UserId                               | FullName         |
        | 73AC6B13-780F-4395-B468-506904422719 | John Doe         |
        | 639A1CDD-7915-47E5-AA2B-263873542B28 | Dr Andrew Martin |
        | FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF | Samantha James   |
        | 11CC3F2D-081A-451D-9387-8B8989CA9C06 | Sarah Williams   |

      Examples:
        | UserId1                                | UserId2                                | MaxLevel |
        | "73AC6B13-780F-4395-B468-506904422719" | "FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF" | 3        |
#-----------------------------------------------------------------------------------------------------------------------
