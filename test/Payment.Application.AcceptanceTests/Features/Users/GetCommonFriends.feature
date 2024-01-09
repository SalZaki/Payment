#=======================================================================================================================
Feature: Get Common Friends
![User] (../../../assets/user.svg)

    In order to support users in the system

    As a user service

    I want to facilitate the retrieval of common friends between two users

    Background:
      Given following users in the system
        | UserId                               | FullName         |
        | 73AC6B13-780F-4395-B468-506904422719 | John Doe         |
        | FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF | Samantha James   |
        | 639A1CDD-7915-47E5-AA2B-263873542B28 | Dr Andrew Martin |
      And the users has the following friends in the system
        | UserId                               | FriendId                             |
        | FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF | 73AC6B13-780F-4395-B468-506904422719 |
        | FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF | 639A1CDD-7915-47E5-AA2B-263873542B28 |
        | 73AC6B13-780F-4395-B468-506904422719 | 639A1CDD-7915-47E5-AA2B-263873542B28 |

    Rule: Get Common Friend Requirements
      - Users must exist in the system.
#-----------------------------------------------------------------------------------------------------------------------
    @user
    @happy-paths
    Scenario Outline: Get Common friends
      Given a user1 with id <UserId1>
      And a user2 with id <UserId2>
      When I submit the request to get common friends
      Then the response should be successful
      And the response should have following common friend
        | UserId                               | FullName         |
        | 639A1CDD-7915-47E5-AA2B-263873542B28 | Dr Andrew Martin |

      Examples:
        | UserId1                                | UserId2                                |
        | "73AC6B13-780F-4395-B468-506904422719" | "FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF" |
#-----------------------------------------------------------------------------------------------------------------------
