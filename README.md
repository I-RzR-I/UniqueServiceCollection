> **Note** This repository is developed using .netstandard2.0

[![NuGet Version](https://img.shields.io/nuget/v/UniqueServiceCollection.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/UniqueServiceCollection/)
[![Nuget Downloads](https://img.shields.io/nuget/dt/UniqueServiceCollection.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/UniqueServiceCollection)

This repository results from the necessity to avoid multiple and duplicate services injection.

This can be a problem when in solution are multiple projects and a lot of services are injected into every project, and if some of the injected services required additional services, in some cases as a result of this manipulation services may be injected multiple times in the same project. In other words circular injection.

To solve this problem are available some extension methods `AddUnique` or `CheckAndCleanUpDuplicateService` for `IServiceCollection`.

**In case you wish to use it in your project, u can install the package from <a href="https://www.nuget.org/packages/UniqueServiceCollection" target="_blank">nuget.org</a>** or specify what version you want:


> `Install-Package UniqueServiceCollection -Version x.x.x.x`

## Content
1. [USING](docs/usage.md)
1. [CHANGELOG](docs/CHANGELOG.md)
1. [BRANCH-GUIDE](docs/branch-guide.md)