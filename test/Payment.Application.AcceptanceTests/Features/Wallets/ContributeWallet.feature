#=======================================================================================================================
Feature: Contribute Wallet
![Wallet] (../assets/wallet.svg)

    In order to use wallets in the system

    As a wallet service

    I want to facilitate the contributions to the wallets

    Background:
      Given following users in the system
        | UserId                               | Fullname        |
        | 6871AEE6-1814-4EBE-9C0F-EDB7201DAD64 | John Doe        |
        | 06E76F39-79D0-4688-B26E-3B162B3C88FA | François Dupont |
        | A0CB3869-B481-48D8-A452-8E7776F71DF4 | Sara MonaLisa   |
      And following wallet with no shares in the system
        | WalletId                             | UserId                               | Currency | Amount | ShareCount | TotalSharesAmount |
        | 3DF951E4-7317-4F17-AB4A-D9EF924D84BF | 6871AEE6-1814-4EBE-9C0F-EDB7201DAD64 | GBP      | 20.22  | 0          | 0.00              |

    Rule: Wallet Contribution Requirements
      1. Wallet and User Prerequisites:
        - A wallet must be created before any contributions can be made.
        - Users must be created before contributions can be added to other's wallets.
      2. Handling Share Creation:
        - If there are no existing shares for a user in a wallet, the wallet service will create new shares during the contribution process.
        - A user can contribute shares in any currency supported.
        - A user cannot contribute to its own wallet with shares.
      3. Adding to Existing Shares:
        - If a user already holds shares in a wallet, additional contributions will be added to their existing shares.
      4. Multi-currency Consideration:
        - The wallet service takes into account multiple currencies when creating new shares or adding to existing ones.
        - The wallet service should support all 281 of fiat currencies.
#-----------------------------------------------------------------------------------------------------------------------
    @wallet
    @happy-paths-with-no-shares
    Scenario Outline: [1/4] Contribute to a user's wallet with a single share
      Given the following share to be contributed to user's wallet
        | ContributorId                        | Currency | Amount | Country |
        | 06E76F39-79D0-4688-B26E-3B162B3C88FA | EUR      | 62.52  | Italy   |
      And wallet id <WalletId>
      When I submit a single share request
      Then the response should be successful
      And the wallet should have
        | WalletId                             | ContributorId                        | ShareCount | Currency | TotalSharesAmount |
        | 3DF951E4-7317-4F17-AB4A-D9EF924D84BF | 06E76F39-79D0-4688-B26E-3B162B3C88FA | 1          | EUR      | 62.52             |

    Examples:
        | WalletId                             |
        | 3DF951E4-7317-4F17-AB4A-D9EF924D84BF |
#-----------------------------------------------------------------------------------------------------------------------
    @wallet
    @happy-paths-with-shares-by-a-contributor
    Scenario Outline: [2/4] Contribute to a user's wallet with a share by a contributor in same currency
      Given the following share are already added to user's wallet
        | WalletId                             | ContributorId                        | Currency | Amount | Country |
        | 3DF951E4-7317-4F17-AB4A-D9EF924D84BF | A0CB3869-B481-48D8-A452-8E7776F71DF4 | TND      | 262.22 | Tunisia |
      And the following shares to be contributed to user's wallet
        | ContributorId                        | Currency | Amount | Country |
        | A0CB3869-B481-48D8-A452-8E7776F71DF4 | TND      | 30.00  | Tunisia |
      And wallet id <WalletId>
      When I submit a multi share request
      Then the response should be successful
      And the wallet should have
        | WalletId                             | ContributorId                        | ShareCount | Currency | TotalSharesAmount |
        | 3DF951E4-7317-4F17-AB4A-D9EF924D84BF | A0CB3869-B481-48D8-A452-8E7776F71DF4 | 2          | TND      | 292.22            |

    Examples:
        | WalletId                             |
        | 3DF951E4-7317-4F17-AB4A-D9EF924D84BF |
#-----------------------------------------------------------------------------------------------------------------------
    @wallet
    @happy-paths-with-shares-by-a-contributor
    Scenario Outline: [3/4] Contribute to a user's wallet with shares by a contributor
      Given the following share are already added to user's wallet
        | WalletId                             | ContributorId                        | Currency | Amount | Country |
        | 3DF951E4-7317-4F17-AB4A-D9EF924D84BF | A0CB3869-B481-48D8-A452-8E7776F71DF4 | TND      | 262.22 | Tunisia |
      And the following shares to be contributed to user's wallet
        | WalletId                             | ContributorId                        | Currency | Amount | Country     |
        | 3DF951E4-7317-4F17-AB4A-D9EF924D84BF | A0CB3869-B481-48D8-A452-8E7776F71DF4 | NZD      | 31.24  | New Zealand |
        | 3DF951E4-7317-4F17-AB4A-D9EF924D84BF | A0CB3869-B481-48D8-A452-8E7776F71DF4 | TND      | 30.00  | Tunisia     |
      And wallet id <WalletId>
      When I submit a multi share request
      Then the response should be successful
      And the wallet should have
        | WalletId                             | ContributorId                        | ShareCount | Currency | TotalSharesAmount |
        | 3DF951E4-7317-4F17-AB4A-D9EF924D84BF | A0CB3869-B481-48D8-A452-8E7776F71DF4 | 2          | TND      | 292.22            |
        | 3DF951E4-7317-4F17-AB4A-D9EF924D84BF | A0CB3869-B481-48D8-A452-8E7776F71DF4 | 1          | NZD      | 31.24             |

    Examples:
        | WalletId                             |
        | 3DF951E4-7317-4F17-AB4A-D9EF924D84BF |
#-----------------------------------------------------------------------------------------------------------------------
    @wallet
    @happy-paths-with-shares-by-contributors
    Scenario Outline: [4/4] Contribute to user's wallet with shares by other contributors
      Given wallet id <WalletId>
      And the following shares to be contributed to user's wallet
        | ContributorId                        | Currency | Amount  | Country    |
        | 06E76F39-79D0-4688-B26E-3B162B3C88FA | EUR      | 233.61  | Luxembourg |
        | 06E76F39-79D0-4688-B26E-3B162B3C88FA | EUR      | 43.20   | Luxembourg |
        | A0CB3869-B481-48D8-A452-8E7776F71DF4 | JPY      | 1200.00 | Japan      |
      When I submit a multi share request
      Then the response should be successful
      And the wallet should have
        | WalletId                             | ContributorId                        | ShareCount | Currency | TotalSharesAmount |
        | 3DF951E4-7317-4F17-AB4A-D9EF924D84BF | 06E76F39-79D0-4688-B26E-3B162B3C88FA | 2          | EUR      | 276.81            |
        | 3DF951E4-7317-4F17-AB4A-D9EF924D84BF | A0CB3869-B481-48D8-A452-8E7776F71DF4 | 1          | JPY      | 1200.00           |

    Examples:
        | WalletId                             |
        | 3DF951E4-7317-4F17-AB4A-D9EF924D84BF |
#-----------------------------------------------------------------------------------------------------------------------
