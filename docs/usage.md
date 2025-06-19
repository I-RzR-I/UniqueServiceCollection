# USING
To avoid problems with duplicate injected services here are available some methods as extensions for `IServiceCollection`.

**AddUnique** (Remove all existing services than add the new one)
* `AddUnique<TService, TImplementing>(
            this IServiceCollection serviceCollection, ServiceLifetime lifetime = ServiceLifetime.Singleton)`;
* `AddUnique<TService>(
            this IServiceCollection serviceCollection,
            Func<IServiceProvider, TService> factory, ServiceLifetime lifetime = ServiceLifetime.Singleton)`;
* `AddUnique(this IServiceCollection serviceCollection, Type serviceType, object instance)`;
* `AddUnique<TService>(this IServiceCollection serviceCollection, TService instance)`.

**RegisterIfNotExist** (Check if the service exists, if the service doesn't exist, then add it)
* `RegisterIfNotExist<TService, TImplementing>(this IServiceCollection serviceCollection, ServiceLifetime lifetime = ServiceLifetime.Singleton)`;
* `RegisterIfNotExist<TService>(this IServiceCollection serviceCollection, 
            ServiceLifetime lifetime = ServiceLifetime.Singleton)`;
* `RegisterIfNotExist<TService>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TService> factory, ServiceLifetime lifetime = ServiceLifetime.Singleton)`.

The specified method can solve your problem by using it where you want and when you want to inject some service.

Or you can use `CheckAndCleanUpDuplicateService<TService>(this IServiceCollection serviceCollection)` which can check and remove duplication inject for specified service types.