---
layout: home
title: Payment Microservice
nav_order: 1
has_children: false
---

# 💳 Payment Microservice

[![Commitizen friendly](https://img.shields.io/badge/commitizen-friendly-brightgreen.svg?logoColor=white&style=for-the-badge)](http://commitizen.github.io/cz-cli/)

## Introduction

The Payment Microservice provides a robust and scalable solution for handling financial operations such as **User Wallet Management**, transaction processing, and balance tracking. Built on **Clean Architecture** principles, this service ensures maintainability and flexibility, enabling seamless integration into various systems.

## 🏗️ Architecture

The architecture adheres to **Clean Architecture** principles, structured in layers:

- **Payment Application**: Manages request processing, repository definitions, and interaction with external services.
- **Payment Domain**: Follows **Domain Driven Design** (DDD) with entities like `User` and `Wallet`, applying business rules and logic.
- **Payment Infrastructure**: Contains infrastructural code, including repositories for databases and external components.

## 🗂️ Solution Structure

```bash
    .
    │
    ├─ Payment
    │   ├─ Payment.Application
    │   ├─ Payment.Domain
    │   ├─ Payment.Infrastructure
    │   └─ Payment.Application.Tests
    │
    └─ README.md
```

### 📦 Payment Application

The **Payment Application** layer, also known as the outermost or edge layer, handles interactions with clients and is responsible for processing requests. It contains implementation details for any repository used in the application.

### 📚 Payment Domain

The **Payment Domain** is the heart of the system and focuses on business logic. Key principles applied to the Domain Models include:

1. **Encapsulation**: Members are private by default, promoting data integrity.
2. **Persistence Ignorance**: Domain models are POCOs, with no infrastructure dependencies.
3. **Rich Behavior**: Business logic is contained within domain models, ensuring a clear separation of concerns.
4. **Value Objects**: Group related primitive attributes into **Value Objects** to reduce primitive obsession.
5. **Business Language**: All naming conventions in the domain reflect the language used in the business context.
6. **Testability**: The domain is isolated and easy to test.

```csharp
public sealed record Wallet : BaseEntity<WalletId>, IComparable<Wallet>, IComparable
{
  private readonly HashSet<Share> _shares = new();

  public required UserId OwnerId { get; init; }
  public required Money Amount { get; init; }
  public ImmutableHashSet<Share> Shares => _shares.ToImmutableHashSet();

  public static readonly Wallet NotFound = Create(WalletId.Create(Guid.Empty), UserId.Create(Guid.Empty));

  private Wallet(WalletId walletId) : base(walletId) { }

  public static Wallet Create(WalletId walletId, UserId ownerId, Money? amount = null)
  {
    Guard.Against.Null(walletId, nameof(walletId), "WalletId cannot be null.");
    Guard.Against.Null(ownerId, nameof(ownerId), "OwnerId cannot be null.");

    return new Wallet(walletId)
    {
      OwnerId = ownerId,
      Amount = amount ?? Money.Empty(),
    };
  }

  public void Contribute(Money amount, UserId contributorId)
  {
    Guard.Against.Negative(amount.InMajorUnits);
    Guard.Against.Negative(amount.InMinorUnits);

    CheckPolicy(new NoSelfContributionPolicy(OwnerId, contributorId));
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

  private void AppendShare(Money amount, UserId contributorId) => _shares.Append(Id, contributorId, amount);

  private void AddShare(Money amount, UserId contributorId)
  {
    var shareId = ShareId.Create(Guid.NewGuid());
    var share = Share.Create(shareId, Id, contributorId, amount);
    _shares.Add(share);
  }
}
```

### 💡 T4 Template for ISO4217 Currencies Lookup

In the **Payment Domain**, we use a **T4 template** to automatically generate code for handling ISO4217 currency representations. This process ensures that our currency-related code is always up-to-date and synchronized with the ISO4217 specification, minimizing manual errors.

#### Purpose

The T4 template automates code generation for currency-related operations (e.g., enumerations, lookup tables). It processes data such as three-letter codes, numeric codes, and currency names, producing C# code for easy integration.

#### Usage

To regenerate the currency-related code, execute the T4 template transformation via Visual Studio's **"Run Custom Tool"** or Rider's **"Run Template"** feature.

```csharp
public static readonly Currency EUR = Currency.Create(
  "Euro", "EUR", 978, 2,
  new []{ "AUSTRIA", "BELGIUM", "FINLAND", "FRANCE", "GERMANY" });
```

### 🔗 Payment Infrastructure

The **Payment Infrastructure** layer provides two key repositories:

- **InMemoryUserRepository**
- **InMemoryWalletRepository**

These ensure efficient data storage and retrieval.

### 🧪 Payment Application Tests

This project ensures robust unit testing using **mocking** to simulate various scenarios and test the application in isolation.

## ✨ Fun Quotes
```
  To move, to breathe, to fly, to float
  To gain all while you give
  To roam the roads of lands remote
  To travel is to live.
```
**[H.C. Andersen](https://en.wikipedia.org/wiki/Hans_Christian_Andersen)**

```
Do anything, but let it produce joy.
```
**[Walt Whitman](https://en.wikipedia.org/wiki/Walt_Whitman)**

---
