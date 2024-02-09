[![Commitizen friendly](https://img.shields.io/badge/commitizen-friendly-brightgreen.svg?logoColor=white&style=for-the-badge)](http://commitizen.github.io/cz-cli/)

# 💳 Payment Microservice

## Architecture

The architecture adheres to Clean Architecture principles and is structured as follows:

**Payment Application** - Handles requests processing, including User and Wallet services, and repository definitions.

**Payment Domain** - Embraces Domain Driven Design, implementing the Bounded Context with entities like User and Wallet.

**Payment Infrastructure** - Houses infrastructural code for external components like databases.

## Solution Structure

```bash
    .
    │
    ├─ Payment                                 
    │   ├─ Payment.Application            
    │   ├─ Payment.Domain
    │   ├─ Payment.Infrastructe
    │   └─ Payment.Application.Tests
    │
    └─ README.md
        
```
### Payment Application

The Payment Application of the most outside layer or edge layer which interacts with clients and responsible for request processing. it also provides implementation details for any repository. 

### Payment Domain

Payment Domain, which is the central and most critical part in the system, should be designed with special attention. Here are some key principles and attributes which are applied to Domain Models of each module:

1. **High level of encapsulation**

   All members are ``private`` by default, then ``internal`` - only ``public`` at the very edge.

2. **High level of PI (Persistence Ignorance)**

   No dependencies to infrastructure, databases, etc. All classes are [POCOs](https://en.wikipedia.org/wiki/Plain_old_CLR_object).

3. **Rich in behavior**

   All business or domain logic is located in the Payment Domain. No leaks to the application layer or elsewhere.

4. **Low level of Primitive Obsession**

   Primitive attributes of Entites grouped together using ValueObjects.

5. **Business language**

   All classes, methods and other members are named in business language used in the Bounded Context of User and Wallet. The business logic within these context are enforced by domain policies.

6. **Testable**

   The Domain is a critical part of the system so it should be easy to test (Testable Design).

```csharp
public sealed record Wallet : BaseEntity<WalletId>, IComparable<Wallet>, IComparable
{
  private readonly HashSet<Share> _shares = new();

  public required UserId OwnerId { get; init; }

  public required Money Amount { get; init; }

  public ImmutableHashSet<Share> Shares => _shares.ToImmutableHashSet();

  public static readonly Wallet NotFound = Create(WalletId.Create(Guid.Empty), UserId.Create(Guid.Empty));

  private Wallet(WalletId walletId) : base(walletId) { }

  public static Wallet Create(
    WalletId walletId,
    UserId ownerId,
    Money? amount = null)
  {
    Guard.Against.Null(walletId, nameof(walletId), "WalletId can not be null.");
    Guard.Against.Null(ownerId, nameof(ownerId), "OwnerId can not be null.");

    var wallet = new Wallet(walletId)
    {
      OwnerId = ownerId,
      Amount = amount ?? Money.Empty(),
    };

    return wallet;
  }

  public void Contribute(
    Money amount,
    UserId contributorId)
  {
    // We are assuming amount can not be negative
    Guard.Against.Negative(amount.InMajorUnits);
    Guard.Against.Negative(amount.InMinorUnits);

    // A wallet owner cannot contribute to its own wallet
    CheckPolicy(new NoSelfContributionPolicy(OwnerId, contributorId));

    // In my opinion, wallets should support multi-currencies for shares, but here we are supporting single currency
    CheckPolicy(new SameCurrencyContributionPolicy(Amount.Currency, amount.Currency));

    if (_shares.ContributorHasShares(Id, contributorId, amount))
    {
      AppendShare(amount, contributorId);
    }
    else
    {
      AddShare(amount, contributorId);
    }
  }

  public int CompareTo(Wallet? other)
  {
    if (other is null) return 1;

    return Id == other.Id ? 0 : 1;
  }

  public int CompareTo(object? obj)
  {
    if (obj is null) return 1;

    return obj is not Wallet wallet ? 1 : CompareTo(wallet);
  }

  private void AppendShare(Money amount, UserId contributorId)
  {
    _shares.Append(Id, contributorId, amount);
  }

  private void AddShare(Money amount, UserId contributorId)
  {
    var shareId = ShareId.Create(Guid.NewGuid());

    var share = Share.Create(shareId, Id, contributorId, amount);

    _shares.Add(share);
  }
}
```

## T4 Template for ISO4217 Currencies Lookup

In the Payment Domain project, I leveraged T4 template to dynamically generate source code for handling ISO4217 representation of fiat currencies. ISO4217 is an international standard that defines three-letter codes for currencies, as established by the International Organization for Standardization (ISO), from [Six Group](https://www.six-group.com/dam/download/financial-information/data-center/iso-currrency/lists/list-one.xml)

### Purpose

The T4 template is employed to automate the generation of code related to currency handling, such as enumerations, lookup tables, and related utility functions. This approach ensures that the currency-related code is always up-to-date with the latest ISO4217 specifications, reducing manual effort and the risk of discrepancies.

### Implementation

The T4 template processes information about fiat currencies, including their three-letter codes, numeric codes, and names, and produces corresponding C# code. This generated code provides a structured and maintainable way to work with currencies within the application.

### Usage

To regenerate the currency-related code or update it based on changes in ISO4217, simply run the T4 template transformation. This can typically be done using Visual Studio's "Run Custom Tool" or "Run Template" in Rider feature on the T4 file.

### Currency Generated File


```csharp
// ------------------------------------------------------------------------------
// Author: Sal Zaki
// <auto-generated>
//   This code has been auto-generated by the associated .tt file.
//   Any changes made to in this file will be lost when the file is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
#nullable enable

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Payment.Domain.ValueObjects;

/// <summary>
/// ISO4217 representation 280 of fiat currencies.
/// </summary>
/// <remarks> Currencies not included:
/// 'ANTARCTICA', 'INTERNATIONAL MONETARY FUND (IMF) ', 'MEMBER COUNTRIES OF THE AFRICAN DEVELOPMENT BANK GROUP', 'PALESTINE, STATE OF', 'SISTEMA UNITARIO DE COMPENSACION REGIONAL DE PAGOS "SUCRE"', 'SOUTH GEORGIA AND THE SOUTH SANDWICH ISLANDS', 'ZZ01_Bond Markets Unit European_EURCO', 'ZZ02_Bond Markets Unit European_EMU-6', 'ZZ03_Bond Markets Unit European_EUA-9', 'ZZ04_Bond Markets Unit European_EUA-17', 'ZZ06_Testing_Code', 'ZZ07_No_Currency', 'ZZ08_Gold', 'ZZ09_Palladium', 'ZZ10_Platinum', 'ZZ11_Silver'
/// </remarks>
/// <remarks> Published On: 01/01/2024 00:00:00 </remarks>
/// <remarks> Build On: 01/14/2024 20:41:20 </remarks>
/// <remarks> Source: https://www.six-group.com/dam/download/financial-information/data-center/iso-currrency/lists/list-one.xml </remarks>
public partial record Currency
{

  /// <summary>
  /// ISO4217 currency representation of Afghani (AFN).
  /// </summary>
  /// <remarks> Country: AFGHANISTAN </remarks>
  /// <remarks> Name: Afghani </remarks>
  /// <remarks> Code: AFN </remarks>
  /// <remarks> Number: 971 </remarks>
  /// <remarks> Minor Units: 2 </remarks>
  public static readonly Currency AFN = Currency.Create("Afghani", "AFN", 971, 2, new []{ "AFGHANISTAN" });

  /// <summary>
  /// ISO4217 currency representation of Euro (EUR).
  /// </summary>
  /// <remarks> Countries: ÅLAND ISLANDS, ANDORRA, AUSTRIA, BELGIUM, CROATIA, CYPRUS, ESTONIA, EUROPEAN UNION, FINLAND, FRANCE, FRENCH GUIANA, FRENCH SOUTHERN TERRITORIES (THE), GERMANY, GREECE, GUADELOUPE, HOLY SEE (THE), IRELAND, ITALY, LATVIA, LITHUANIA, LUXEMBOURG, MALTA, MARTINIQUE, MAYOTTE, MONACO, MONTENEGRO, NETHERLANDS (THE), PORTUGAL, RÉUNION, SAINT BARTHÉLEMY, SAINT MARTIN (FRENCH PART), SAINT PIERRE AND MIQUELON, SAN MARINO, SLOVAKIA, SLOVENIA, SPAIN </remarks>
  /// <remarks> Name: Euro </remarks>
  /// <remarks> Code: EUR </remarks>
  /// <remarks> Number: 978 </remarks>
  /// <remarks> Minor Units: 2 </remarks>
  public static readonly Currency EUR = Currency.Create("Euro", "EUR", 978, 2, new []{ "ÅLAND ISLANDS", "ANDORRA", "AUSTRIA", "BELGIUM", "CROATIA", "CYPRUS", "ESTONIA", "EUROPEAN UNION", "FINLAND", "FRANCE", "FRENCH GUIANA", "FRENCH SOUTHERN TERRITORIES (THE)", "GERMANY", "GREECE", "GUADELOUPE", "HOLY SEE (THE)", "IRELAND", "ITALY", "LATVIA", "LITHUANIA", "LUXEMBOURG", "MALTA", "MARTINIQUE", "MAYOTTE", "MONACO", "MONTENEGRO", "NETHERLANDS (THE)", "PORTUGAL", "RÉUNION", "SAINT BARTHÉLEMY", "SAINT MARTIN (FRENCH PART)", "SAINT PIERRE AND MIQUELON", "SAN MARINO", "SLOVAKIA", "SLOVENIA", "SPAIN" });
....
```

### Notes

- Ensure that the ISO4217 data source used by the T4 template is kept up-to-date for accurate currency information.
- Review the generated code regularly to incorporate any changes or additions to the ISO4217 standard.

For more details on ISO4217 and the list of fiat currencies, visit [ISO 4217 Currency Codes](https://www.iso.org/iso-4217-currency-codes.html).

### Payment Infrastructure

The Payment Infrastructure project has a straightforward purpose: to implement the following two repositories:

- InMemoryUserRepository
- InMemoryWalletRepository

These repositories serve as the backbone for data storage and retrieval, ensuring efficient interactions between the application and its underlying data sources.

### Payment Application Tests

The Payment Application Tests project is dedicated to comprehensive unit testing. Leveraging the power of mocking, it meticulously isolates each test scenario by creating a controlled environment for testing. This approach guarantees that individual components are thoroughly examined in isolation, providing confidence in the robustness and reliability of the Payment Application.

### Fun Qoutes

> To move, to breathe, to fly, to float,  
> To gain all while you give,  
> To roam the roads of lands remote,  
> To travel is to live.
>
> **[H.C. Andersen](https://en.wikipedia.org/wiki/Hans_Christian_Andersen)**


> Do anything, but let it produce joy
>
> **[Walt Whitman](https://en.wikipedia.org/wiki/Walt_Whitman)**
