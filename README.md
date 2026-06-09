> **Note** This repository is developed using .netstandard2.0

[![NuGet Version](https://img.shields.io/nuget/v/RzR.Extensions.UniqueServiceCollection.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/RzR.Extensions.UniqueServiceCollection/)
[![Nuget Downloads](https://img.shields.io/nuget/dt/RzR.Extensions.UniqueServiceCollection.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/RzR.Extensions.UniqueServiceCollection)

<details>

  <summary>Old version</summary>
  
[![NuGet Version](https://img.shields.io/nuget/v/UniqueServiceCollection.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/UniqueServiceCollection/)
[![Nuget Downloads](https://img.shields.io/nuget/dt/UniqueServiceCollection.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/UniqueServiceCollection)

</details>

<br />

Extends `IServiceCollection` with methods to register services without creating duplicates and to detect or clean up accidental duplicate registrations.

The problem this solves: in solutions with multiple projects, the same service can be registered many times as different projects each add their dependencies. The result is multiple identical descriptors in the container — wasted allocations at best, and unexpected behavior at worst.

## Install

```powershell
Install-Package RzR.Extensions.UniqueServiceCollection
```

The package depends only on `Microsoft.Extensions.DependencyInjection.Abstractions`.

## Quick start

```csharp
using RzR.Extensions.UniqueServiceCollection.ServiceCollectionExtensions;

// Last-wins: remove any existing IMyService registration, then add this one.
services.AddUnique<IMyService, MyService>();

// First-wins: add only if IMyService is not already registered.
services.RegisterIfNotExist<IMyService, MyService>();

// Replace: same as AddUnique but returns true when a prior registration existed.
bool replaced = services.ReplaceUnique<IMyService, MyServiceV2>();

// Fluent chaining — all AddUnique* and RegisterIfNotExist* methods return IServiceCollection.
services
    .AddUnique<IServiceA, ServiceA>()
    .AddUnique<IServiceB, ServiceB>(ServiceLifetime.Scoped)
    .RegisterIfNotExist<IServiceC, ServiceC>();

// Startup guardrail — throws InvalidOperationException if exact duplicates exist.
services.ValidateNoDuplicates();
```

## Two registration strategies

| Method group | Strategy | Wins |
| --- | --- | --- |
| `AddUnique*` | Remove all existing registrations for the type, then add the new one | Last registration |
| `RegisterIfNotExist*` | Add only when no registration for the type exists yet | First registration |

Choose `AddUnique*` when you want to guarantee a specific implementation is used regardless of registration order. Choose `RegisterIfNotExist*` when you want a default that callers can override by registering first.

## What counts as a duplicate

A duplicate is a registration that shares the same **ServiceType**, **Lifetime**, and **implementation identity** (same `ImplementationType`, same `ImplementationInstance` reference, or same `ImplementationFactory` reference) as another registration already in the collection.

Intentional multi-registration of **distinct** implementations of the same interface — the standard `IEnumerable<TService>` resolution pattern — is **not** treated as a duplicate and is never removed by the cleanup or validation methods.

## Feature highlights

- `AddUnique<TService, TImplementing>()` — type-mapped registration, configurable lifetime
- `AddUnique<TService>(factory)` — factory registration, configurable lifetime
- `AddUnique<TService>(instance)` — singleton instance registration
- `RegisterIfNotExist<TService, TImplementing>()` — first-wins registration
- `ReplaceUnique<TService, TImplementing>()` — replace and report whether anything was replaced
- `CheckAndCleanUpDuplicateService<TService>()` — remove exact duplicates for one type
- `CheckAndCleanUpAllDuplicates()` — remove exact duplicates across all types
- `FindExactDuplicates()` / `FindExactDuplicates<TService>()` — inspect without mutating
- `FindServiceDuplicate()` / `FindServiceDuplicate<TService>()` — legacy overloads (count-based)
- `ValidateNoDuplicates()` — fail-fast guardrail, suitable for unit tests and CI startup checks

## Content

1. [USING](docs/usage.md)
2. [CHANGELOG](docs/CHANGELOG.md)
3. [BRANCH-GUIDE](docs/branch-guide.md)
