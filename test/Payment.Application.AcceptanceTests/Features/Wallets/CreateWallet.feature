#=======================================================================================================================
Feature: Create Wallet
![Wallet] (../../../assets/wallet.svg)

    In order to use wallets in the system

    As a wallet service

    I want to facilitate the creation of new wallets

    Background:
      Given the following users exists in the system
        | UserId                               | Fullname    |
        | 4E162382-324E-49B7-8629-EB5A929BCB44 | Susan Reed  |
        | 06839E7B-18D1-4FFB-8688-5B0E60F6A5B5 | John Peters |
      And there are no wallets

    Rule: Wallet Creation Requirements
      - User must be created before a wallet.
      - Users can have only one wallet simultaneously.
      - Wallets can define their currency and amount.
      - Wallets can have zero or many shares.
#-----------------------------------------------------------------------------------------------------------------------
    @wallet
    @happy-paths-with-no-amount
    Scenario Outline: [01/02] Create a wallet with no amount
      And a user id <UserId>
      When I submit the request
      Then the response should be successful
      And the response should have a newly created wallet id
      And the wallet should have
        | UserId                               | ShareCount | Amount | TotalSharesAmount |
        | 06839E7B-18D1-4FFB-8688-5B0E60F6A5B5 | 0          | 0.00   | 0.00              |

    Examples:
        | UserId                               |
        | 06839E7B-18D1-4FFB-8688-5B0E60F6A5B5 |
#-----------------------------------------------------------------------------------------------------------------------
    @wallet
    @happy-paths-with-amount
    Scenario Outline: [02/02] Create a wallet with amount
      And a user id <UserId>
      And currency <Currency> and amount <Amount>
      When I submit the request
      Then the response should be successful
      And the response should have a newly created wallet id
      And the wallet should have
        | UserId                               | ShareCount | Currency | Amount | TotalSharesAmount |
        | 06839E7B-18D1-4FFB-8688-5B0E60F6A5B5 | 0          | GBP      | 100.00 | 0.00              |

    Examples:
        | UserId                               | Currency | Amount |
        | 06839E7B-18D1-4FFB-8688-5B0E60F6A5B5 | GBP      | 100.00 |
#-----------------------------------------------------------------------------------------------------------------------
