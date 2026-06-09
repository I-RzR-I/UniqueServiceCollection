# Usage

This page is the full API reference for `RzR.Extensions.UniqueServiceCollection`. All methods are extension methods on `IServiceCollection` and live in the namespace `RzR.Extensions.UniqueServiceCollection.ServiceCollectionExtensions` unless noted.

---

## What counts as a duplicate

A registration is an exact duplicate when it shares the same **ServiceType**, **Lifetime**, and **implementation identity** as a registration already present in the collection.

Implementation identity is determined in this order:

- `ImplementationType` — compared by type equality.
- `ImplementationInstance` — compared by reference (`ReferenceEquals`).
- `ImplementationFactory` — compared by reference (`ReferenceEquals`).

Intentional multi-registration of **distinct** implementations of the same interface — the standard `IEnumerable<TService>` resolution pattern — is **not** a duplicate. The monitoring and cleanup methods preserve every distinct registration and only remove registrations that are genuinely identical.

---

## Two complementary strategies

The library provides two groups of registration methods that are mirror opposites.

**`AddUnique*` — last-wins (remove-then-add)**

Removes all existing registrations for the service type before adding the new one. Use this when you want to guarantee a specific implementation wins regardless of what was registered before.

**`RegisterIfNotExist*` — first-wins (add-only-if-absent)**

Adds the registration only when the service type has no existing entry. Use this when you want a default that a caller can override by registering earlier.

Both method groups return `IServiceCollection` and support fluent chaining.

---

## AddUnique — type-mapped and factory registrations

Class: `AddUniqueCollectionExtensions`

### Type-mapped with implementation

```csharp
IServiceCollection AddUnique<TService, TImplementing>(
    this IServiceCollection serviceCollection,
    ServiceLifetime lifetime = ServiceLifetime.Singleton)
    where TService : class
    where TImplementing : class, TService
```

Removes all registrations for `TService` then registers `TImplementing` at the given lifetime. Defaults to `Singleton`.

```csharp
services.AddUnique<IMyService, MyService>();
services.AddUnique<IMyService, MyService>(ServiceLifetime.Scoped);
```

### Self-registration

```csharp
IServiceCollection AddUnique<TService>(
    this IServiceCollection serviceCollection,
    ServiceLifetime lifetime)
    where TService : class
```

Registers `TService` as its own implementation at the specified lifetime.

```csharp
services.AddUnique<MyService>(ServiceLifetime.Scoped);
```

### Non-generic self-registration

```csharp
IServiceCollection AddUnique(
    this IServiceCollection serviceCollection,
    Type serviceType,
    ServiceLifetime lifetime)
```

```csharp
services.AddUnique(typeof(MyService), ServiceLifetime.Transient);
```

### Factory registration

```csharp
IServiceCollection AddUnique<TService>(
    this IServiceCollection serviceCollection,
    Func<IServiceProvider, TService> factory,
    ServiceLifetime lifetime = ServiceLifetime.Singleton)
    where TService : class
```

```csharp
services.AddUnique<IMyService>(
    sp => new MyService(sp.GetRequiredService<IDependency>()),
    ServiceLifetime.Scoped);
```

---

## AddUnique — singleton instance registrations

Class: `AddUniqueCollectionExtensions` (partial, defined in `AddUniqueSingletonExtensions.cs`)

These overloads always register as `Singleton` because they accept a pre-constructed instance.

### Generic instance

```csharp
IServiceCollection AddUnique<TService>(
    this IServiceCollection serviceCollection,
    TService instance)
    where TService : class
```

```csharp
var impl = new MyService();
services.AddUnique<IMyService>(impl);
```

### Non-generic instance

```csharp
IServiceCollection AddUnique(
    this IServiceCollection serviceCollection,
    Type serviceType,
    object instance)
```

```csharp
services.AddUnique(typeof(IMyService), new MyService());
```

### No-argument singleton (self-register)

```csharp
IServiceCollection AddUnique<TService>(
    this IServiceCollection serviceCollection)
    where TService : class
```

Registers `TService` as its own `Singleton` implementation.

```csharp
services.AddUnique<MyService>();
```

---

## RegisterIfNotExist — first-wins registration

Class: `RegisterServiceCollectionExtensions`

These methods add a registration only when no registration for the service type exists. Existing registrations are always left untouched.

### RegisterIfNotExist — type-mapped

```csharp
IServiceCollection RegisterIfNotExist<TService, TImplementing>(
    this IServiceCollection serviceCollection,
    ServiceLifetime lifetime = ServiceLifetime.Singleton)
    where TService : class
    where TImplementing : class, TService
```

```csharp
services.RegisterIfNotExist<IMyService, MyService>();
services.RegisterIfNotExist<IMyService, MyService>(ServiceLifetime.Transient);
```

### RegisterIfNotExist — self-registration

```csharp
IServiceCollection RegisterIfNotExist<TService>(
    this IServiceCollection serviceCollection,
    ServiceLifetime lifetime = ServiceLifetime.Singleton)
    where TService : class
```

```csharp
services.RegisterIfNotExist<MyService>(ServiceLifetime.Scoped);
```

### RegisterIfNotExist — factory registration

```csharp
IServiceCollection RegisterIfNotExist<TService>(
    this IServiceCollection serviceCollection,
    Func<IServiceProvider, TService> factory,
    ServiceLifetime lifetime = ServiceLifetime.Singleton)
    where TService : class
```

```csharp
services.RegisterIfNotExist<IMyService>(
    sp => new MyService(sp.GetRequiredService<IDependency>()));
```

---

## Fluent chaining

Every `AddUnique*` and `RegisterIfNotExist*` method returns `IServiceCollection`, so calls can be chained.

```csharp
services
    .AddUnique<IServiceA, ServiceA>()
    .AddUnique<IServiceB, ServiceB>(ServiceLifetime.Scoped)
    .RegisterIfNotExist<IServiceC, ServiceC>()
    .RegisterIfNotExist<IServiceD, ServiceD>(ServiceLifetime.Transient);
```

---

## ReplaceUnique — replace with feedback

Class: `ReplaceUniqueExtensions`

```csharp
bool ReplaceUnique<TService, TImplementing>(
    this IServiceCollection serviceCollection,
    ServiceLifetime lifetime = ServiceLifetime.Singleton)
    where TService : class
    where TImplementing : class, TService
```

Removes all existing registrations for `TService` and registers `TImplementing`. Returns `true` when at least one prior registration existed and was replaced; `false` when this is the first registration for `TService`.

**This is not the BCL `TryAdd` convention.** `TryAdd*` methods are first-wins (add-if-absent). `ReplaceUnique` is always last-wins and uses the return value purely to report whether a replacement occurred.

```csharp
bool replaced = services.ReplaceUnique<IMyService, MyServiceV2>();
if (replaced)
    Console.WriteLine("Previous IMyService registration was replaced.");
```

A typical use case is a test project that swaps a production implementation for a test double:

```csharp
// Production code registered MyService somewhere up the chain.
services.ReplaceUnique<IMyService, FakeMyService>(ServiceLifetime.Singleton);
```

---

## Monitoring and cleanup

Class: `MonitoringUniqueCollectionExtension`

All cleanup methods return `IServiceCollection` for fluent chaining. All inspection methods return enumerable results or a single report object.

### CheckAndCleanUpDuplicateService\<T\>

```csharp
IServiceCollection CheckAndCleanUpDuplicateService<TService>(
    this IServiceCollection serviceCollection)
    where TService : class
```

Removes exact duplicate registrations for `TService` only. Distinct implementations of the same interface are preserved.

```csharp
services.CheckAndCleanUpDuplicateService<IMyService>();
```

### CheckAndCleanUpAllDuplicates

```csharp
IServiceCollection CheckAndCleanUpAllDuplicates(
    this IServiceCollection serviceCollection)
```

Scans every service type in the collection and removes exact duplicates across all of them.

```csharp
services.CheckAndCleanUpAllDuplicates();
```

### FindExactDuplicates — all types

```csharp
IEnumerable<DuplicateServiceReport> FindExactDuplicates(
    this IServiceCollection serviceCollection)
```

Returns a `DuplicateServiceReport` for each service type that has at least one genuinely identical duplicate registration. Types with multiple distinct implementations are not included. Does not mutate the collection.

```csharp
var reports = services.FindExactDuplicates();
foreach (var report in reports)
{
    Console.WriteLine($"{report.RetainedDescriptor.ServiceType.Name}: " +
        $"{report.DuplicateRegistrations.Count} duplicate(s) found");
}
```

### FindExactDuplicates\<T\> — single type

```csharp
DuplicateServiceReport FindExactDuplicates<TService>(
    this IServiceCollection serviceCollection)
    where TService : class
```

Returns the `DuplicateServiceReport` for `TService` when exact duplicates exist, or `null` when there are none.

```csharp
var report = services.FindExactDuplicates<IMyService>();
if (report != null)
{
    // report.DuplicateRegistrations lists the redundant descriptors
}
```

### FindServiceDuplicate — legacy overloads

```csharp
IEnumerable<DuplicateServicesDto> FindServiceDuplicate(
    this IServiceCollection serviceCollection)

IEnumerable<DuplicateServicesDto> FindServiceDuplicate<TService>(
    this IServiceCollection serviceCollection)
    where TService : class
```

These are legacy methods retained for backward compatibility. They group registrations by `ServiceType` and return a `DuplicateServicesDto` for every type registered more than once, regardless of whether the registrations are identical or intentionally distinct. Intentional multi-registrations (different implementations of the same interface) are included in the results.

Use `FindExactDuplicates` or `FindExactDuplicates<TService>` instead when you need to distinguish accidental duplicates from intentional multi-registrations.

```csharp
// Legacy — reports every type with more than one registration, including intentional ones.
var allMultiRegistered = services.FindServiceDuplicate();

// Preferred — reports only genuinely identical registrations.
var exactDuplicates = services.FindExactDuplicates();
```

### ValidateNoDuplicates

```csharp
IServiceCollection ValidateNoDuplicates(
    this IServiceCollection serviceCollection)
```

Returns the collection unchanged when no exact duplicates exist. Throws `InvalidOperationException` when at least one exact duplicate is found. The exception message names the affected service types and their lifetimes.

Use this at the end of startup configuration to fail fast, or in unit tests to assert a clean container state.

```csharp
// In Program.cs / Startup.cs — fails the application on startup if duplicates exist.
services
    .CheckAndCleanUpAllDuplicates() // clean up first if you want to auto-repair
    .ValidateNoDuplicates();        // then assert clean state

// In a unit test.
Assert.DoesNotThrow(() => services.ValidateNoDuplicates());
```

---

## Result types

Namespace: `RzR.Extensions.UniqueServiceCollection.DTO`

### DuplicateServiceReport

Returned by `FindExactDuplicates` and `FindExactDuplicates<TService>`. Produced internally by `CheckAndCleanUpDuplicateService<TService>` and `CheckAndCleanUpAllDuplicates`.

```csharp
public class DuplicateServiceReport
{
    // All registrations present for the service type when the report was built.
    // Never null — defaults to an empty array.
    public IReadOnlyList<ServiceDescriptor> AllRegistrations { get; set; }

    // The single registration that was kept (the first distinct one encountered).
    public ServiceDescriptor RetainedDescriptor { get; set; }

    // The registrations identified as exact duplicates.
    // Empty means the type had multiple distinct implementations — none were removed.
    // Never null — defaults to an empty array.
    public IReadOnlyList<ServiceDescriptor> DuplicateRegistrations { get; set; }
}
```

A non-empty `DuplicateRegistrations` list means exact duplicates were found. An empty list on a report returned by `FindExactDuplicates` will not appear — the method filters those out before returning.

### DuplicateServicesDto

Returned by the legacy `FindServiceDuplicate` overloads.

```csharp
public class DuplicateServicesDto
{
    // Total number of registrations found for the same ServiceType.
    public int Count { get; set; }

    // The first ServiceDescriptor in the group.
    public ServiceDescriptor ServiceDescriptor { get; set; }
}
```

---

## Common patterns

### Guarantee a specific implementation in a multi-project solution

```csharp
// LibraryA registers its default.
services.RegisterIfNotExist<IEmailSender, SmtpEmailSender>();

// The host application overrides it without caring what LibraryA registered.
services.AddUnique<IEmailSender, SendGridEmailSender>();
```

### Swap an implementation in tests

```csharp
// Replace whatever the production startup registered.
services.ReplaceUnique<IEmailSender, NullEmailSender>(ServiceLifetime.Singleton);
```

### Clean up then assert in a startup test

```csharp
var services = new ServiceCollection();
// ... configure services ...

services
    .CheckAndCleanUpAllDuplicates()
    .ValidateNoDuplicates();
```

### Inspect without mutating

```csharp
foreach (var report in services.FindExactDuplicates())
{
    Console.WriteLine(
        $"[WARN] {report.RetainedDescriptor.ServiceType.FullName} has " +
        $"{report.DuplicateRegistrations.Count} exact duplicate(s).");
}
```
