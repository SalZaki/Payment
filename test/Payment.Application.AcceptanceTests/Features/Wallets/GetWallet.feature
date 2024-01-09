#=======================================================================================================================
Feature: Get Wallet
![Wallet] (../../../assets/wallet.svg)

    In order to use wallets in the system

    As a wallet service

    I want to facilitate the retrieval of wallets

    Background:
      Given following wallets in the system
        | WalletId                             | UserId                               | Currency | Amount |
        | 73AC6B13-780F-4395-B468-506904422719 | 150A98BE-7F10-4BA4-A8B5-05B20E7A3A49 | DZD      | 32.12  |
        | FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF | 20C11779-5236-42A0-A636-1D6F7AE8AF7F | CAD      | 54.33  |
      And following shares for a wallet in the system
        | WalletId                             | ContributorId                        | Currency | Amount | Country |
        | 73AC6B13-780F-4395-B468-506904422719 | 8A959DA8-C4BB-46EC-9CE9-BFD051822A90 | GBP      | 41.10  | UK      |
        | 73AC6B13-780F-4395-B468-506904422719 | 8A959DA8-C4BB-46EC-9CE9-BFD051822A90 | GBP      | 43.00  | UK      |
        | 73AC6B13-780F-4395-B468-506904422719 | 8A959DA8-C4BB-46EC-9CE9-BFD051822A90 | GBP      | 167.50 | UK      |
        | 73AC6B13-780F-4395-B468-506904422719 | 8A959DA8-C4BB-46EC-9CE9-BFD051822A90 | GBP      | 599.99 | UK      |

    Rule: Wallet Retrieval Requirements
      - A wallet must exist in the system.
      - A wallet can contain no shares.
      - A wallet can contain one or more shares.
#-----------------------------------------------------------------------------------------------------------------------
    @wallet
    @happy-paths-with-no-share
    Scenario Outline: [01/02] Get a wallet with no shares
      Given a wallet id <WalletId> with no shares
      When I submit the request to get a wallet
      Then the response should be successful
      And the wallet should have
        | WalletId                             | UserId                               | ContributorId                        | Currency | Amount | ShareCount | TotalSharesAmount |
        | FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF | 20C11779-5236-42A0-A636-1D6F7AE8AF7F | 00000000-0000-0000-0000-000000000000 | CAD      | 54.33  | 0          | 0.00              |

    Examples:
      | WalletId                               |
      | "FF8C0411-745E-4E11-ADAF-8C9C3C5E34AF" |
#-----------------------------------------------------------------------------------------------------------------------
    @wallet
    @happy-paths-with-with-share
    Scenario Outline: [02/02] Get a wallet with shares
      Given a wallet id <WalletId> with shares
      When I submit the request to get a wallet
      Then the response should be successful
      And the wallet should have
        | WalletId                             | UserId                               | ContributorId                        | Currency | Amount | ShareCount | TotalSharesAmount |
        | 73AC6B13-780F-4395-B468-506904422719 | 150A98BE-7F10-4BA4-A8B5-05B20E7A3A49 | 8A959DA8-C4BB-46EC-9CE9-BFD051822A90 | DZD      | 32.12  | 4          | 851.59            |

    Examples:
      | WalletId                               |
      | "73AC6B13-780F-4395-B468-506904422719" |
#-----------------------------------------------------------------------------------------------------------------------
