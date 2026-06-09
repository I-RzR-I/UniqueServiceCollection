### **v3.0.0.8109** [[RzR](mailto:108324929+I-RzR-I@users.noreply.github.com)] 09-06-2026
* [FIX] - (RzR) -> `AddUnique<TService>(TService instance)` silently discarded the supplied instance and registered the type for container construction; it now correctly registers the provided instance via `AddSingleton(typeof(TService), instance)`.
* [FIX] - (RzR) -> `SCHasNoAny(null)` returned `false`; a null collection now correctly reports `true` (both `Type` and generic overloads).
* [FIX] - (RzR) -> Removed dead, boxing `IfNullThrowArgumentNullException` null-checks on the non-nullable `ServiceLifetime` enum.
* [FIX] - (RzR) -> Corrected the `AddUnique(Type, object)` XML-doc example (was showing the factory-delegate overload).
* [DEV] - (RzR) -> `ReplaceUnique<TService, TImplementing>(...)` — last-wins registration returning `bool` (true when an existing registration was replaced). Deliberately distinct from the BCL `TryAdd` (add-if-absent) convention.
* [DEV] - (RzR) -> `CheckAndCleanUpAllDuplicates()` — non-generic; cleans every duplicated type in the collection.
* [DEV] - (RzR) -> `FindExactDuplicates()` / `FindExactDuplicates<TService>()` — exact-duplicate queries (vs. the legacy `FindServiceDuplicate` overloads, retained for backward compatibility).
* [DEV] - (RzR) -> `ValidateNoDuplicates()` — guardrail that throws `InvalidOperationException` (listing offending service types and lifetimes) when exact duplicates exist; usable in unit tests / CI startup checks.
* [DEV] - (RzR) -> `DuplicateServiceReport` — structured report exposing all registrations for a type, the retained descriptor, and the removed duplicates (collections default to empty, never null).


### v**2.0.0.7916** [[RzR](mailto:108324929+I-RzR-I@users.noreply.github.com)] 19-06-2025
* [38fb241] (RzR) -> Adjust read me and using files.
* [95d8415] (RzR) -> Diable solution build on script execution
* [026962d] (RzR) -> Add new version generate scripts
* [e64ac42] (RzR) -> Remove duplicate/irelevant extensions and refactor methods
* [3e5bc37] (RzR) -> Adjust and supply with new tests for Service Collection
* [72b0806] (RzR) -> Add internal extension that throw exception
* [1a97010] (RzR) -> Add tests for new public extensions
* [cea8928] (RzR) -> Adjust public extension methods using new methods.
* [8bc9909] (RzR) -> Add internal service collection extensions
* [abef421] (RzR) -> Add new internal service collection extension methods
* [02ca75f] (RzR) -> Adjust namespace for internal extensions
* [4eb6886] (RzR) -> Adjust ServiceCollection extensions and tests
* [361a908] (RzR) -> Add collection & rename collection/object tests
* [97964f6] (RzR) -> Adjust code to use enumerable extensions.
* [0946dc3] (RzR) -> Adjust tests after method rename.
* [9368947] (RzR) -> Add new internal ServiceCollection extensions and rename existing.
* [b5fb18f] (RzR) -> Rename internal object extensions.
* [888c78a] (RzR) -> Add internal Enumerable/Collection extensions.
* [9f55836] (RzR) -> Add ServiceCollection extensions and tests
* [349a620] (RzR) -> Upgrade version for `CodeSource` package.
* [e3aa75f] (RzR) -> Adjust maintenance year.
* [2c261e6] (RzR) -> Move collection extensions to a separate folder.

### **v1.0.4.2241** 
-> Fix wrong modification.<br />

### **1.0.2.0831** 
-> Change visibility for used extension.<br />

### **1.0.2.0617** 
-> Add extension methods `AddUnique` as AddSingleton.<br />

### **1.0.1.1352** 
-> Add extension methods `AddUnique` and `CheckAndCleanUpDuplicateService` for `ServiceCollection`.<br />

### **1.0.0.0** 
-> Init commit.<br />
-> Add extension methods `AddUnique` and `CheckAndCleanUpDuplicateService`.<br />
