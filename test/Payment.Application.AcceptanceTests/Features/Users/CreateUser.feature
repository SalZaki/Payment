#=======================================================================================================================
Feature: Create User
![User] (../assets/user.svg)

    In order to support users in the system

    As a user service

    I want to facilitate the creation of new users

    Rule: User Creation Requirements
      - User must have a valid fullname.
#-----------------------------------------------------------------------------------------------------------------------
    @user
    @happy-paths
    Scenario Outline: Create a user
      Given a fullname "Steve McNulty"
      And the user does not exist in the system
      When I submit the request to create the user
      Then the response should be successful
      And the response should have a newly created user id
#-----------------------------------------------------------------------------------------------------------------------
