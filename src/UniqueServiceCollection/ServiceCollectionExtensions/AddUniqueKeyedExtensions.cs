// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2026-08-14 15:20
//
//  Last Modified By : RzR
//  Last Modified On : 2026-08-23 23:53
// ***********************************************************************
//  <copyright file="AddUniqueKeyedExtensions.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
//
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.Extensions.DependencyInjection;
using RzR.Extensions.UniqueServiceCollection.Extensions;
using RzR.Extensions.UniqueServiceCollection.Helpers;
using System;

#endregion

namespace RzR.Extensions.UniqueServiceCollection.ServiceCollectionExtensions
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Add unique keyed services to the application service collection.
    /// </summary>
    /// =================================================================================================
    public static class AddUniqueKeyedExtensions
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Add unique keyed service of the type <typeparamref name="TService" /> with its
        ///     implementation on <typeparamref name="TImplementing" /> under <paramref name="serviceKey" />.
        /// </summary>
        /// <remarks>
        ///     Any previous registration of <typeparamref name="TService" /> under the same key is removed
        ///     first. Other keys and the non-keyed registration are left untouched.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="serviceCollection" /> or <paramref name="serviceKey" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="serviceKey" /> is the resolution-time wildcard key.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when the loaded DependencyInjection.Abstractions predates 8.0.
        /// </exception>
        /// <typeparam name="TService">Type of service that will be added.</typeparam>
        /// <typeparam name="TImplementing">Type of service implementation.</typeparam>
        /// <param name="serviceCollection">Required. Service collection.</param>
        /// <param name="serviceKey">Required. The registration key.</param>
        /// <param name="lifetime">(Optional) The default value is ServiceLifetime.Singleton.</param>
        /// <returns>
        ///     The original <paramref name="serviceCollection" />, for fluent chaining.
        /// </returns>
        /// <example>
        ///     <code>
        ///     serviceCollection.AddUniqueKeyed&lt;IStore, BlobStore&gt;("tenantA");
        ///     serviceCollection.AddUniqueKeyed&lt;IStore, FileStore&gt;("tenantB", ServiceLifetime.Scoped);
        ///     </code>
        /// </example>
        /// =================================================================================================
        public static IServiceCollection AddUniqueKeyed<TService, TImplementing>(this IServiceCollection serviceCollection,
            object serviceKey, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
            where TImplementing : class, TService
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            InternalKeyedSupport.ThrowIfNotSupported();
            InternalKeyedSupport.SCValidateServiceKey(serviceKey, nameof(serviceKey));
            lifetime.SCValidateLifetime(nameof(lifetime));

            serviceCollection.SCRemoveAllKeyed(typeof(TService), serviceKey);
            serviceCollection.Add(InternalKeyedSupport.SCDescribeKeyed(
                typeof(TService), serviceKey, typeof(TImplementing), lifetime));

            return serviceCollection;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Add unique keyed service of the type <typeparamref name="TService" /> as its own
        ///     implementation under <paramref name="serviceKey" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="serviceCollection" /> or <paramref name="serviceKey" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="serviceKey" /> is the resolution-time wildcard key.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when the loaded DependencyInjection.Abstractions predates 8.0.
        /// </exception>
        /// <typeparam name="TService">Type of the service. Must be a concrete, instantiable type.</typeparam>
        /// <param name="serviceCollection">Required. Service collection.</param>
        /// <param name="serviceKey">Required. The registration key.</param>
        /// <param name="lifetime">(Optional) The default value is ServiceLifetime.Singleton.</param>
        /// <returns>
        ///     The original <paramref name="serviceCollection" />, for fluent chaining.
        /// </returns>
        /// =================================================================================================
        public static IServiceCollection AddUniqueKeyed<TService>(this IServiceCollection serviceCollection,
            object serviceKey, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
            => serviceCollection.AddUniqueKeyed(typeof(TService), serviceKey, typeof(TService), lifetime);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Add unique keyed service of the type <typeparamref name="TService" /> with the supplied
        ///     factory under <paramref name="serviceKey" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="serviceCollection" />, <paramref name="serviceKey" /> or
        ///     <paramref name="factory" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="serviceKey" /> is the resolution-time wildcard key.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when the loaded DependencyInjection.Abstractions predates 8.0.
        /// </exception>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceCollection">Required. Service collection.</param>
        /// <param name="serviceKey">Required. The registration key.</param>
        /// <param name="factory">
        ///     Required. Factory receiving the service provider and the key the service was resolved with.
        /// </param>
        /// <param name="lifetime">(Optional) The default value is ServiceLifetime.Singleton.</param>
        /// <returns>
        ///     The original <paramref name="serviceCollection" />, for fluent chaining.
        /// </returns>
        /// <example>
        ///     <code>
        ///     serviceCollection.AddUniqueKeyed&lt;IStore&gt;("tenantA",
        ///         (provider, key) =&gt; new BlobStore(provider.GetRequiredService&lt;IConfig&gt;(), key));
        ///     </code>
        /// </example>
        /// =================================================================================================
        public static IServiceCollection AddUniqueKeyed<TService>(this IServiceCollection serviceCollection,
            object serviceKey, Func<IServiceProvider, object, TService> factory,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            InternalKeyedSupport.ThrowIfNotSupported();
            InternalKeyedSupport.SCValidateServiceKey(serviceKey, nameof(serviceKey));
            factory.IfNullThrowArgumentNullException(nameof(factory));
            lifetime.SCValidateLifetime(nameof(lifetime));

            serviceCollection.SCRemoveAllKeyed(typeof(TService), serviceKey);
            serviceCollection.Add(InternalKeyedSupport.SCDescribeKeyed(
                typeof(TService), serviceKey, factory, lifetime));

            return serviceCollection;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Add unique keyed service of the type <paramref name="serviceType" /> with its implementation
        ///     on <paramref name="implementationType" /> under <paramref name="serviceKey" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="serviceKey" /> is the resolution-time wildcard key.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when the loaded DependencyInjection.Abstractions predates 8.0.
        /// </exception>
        /// <param name="serviceCollection">Required. Service collection.</param>
        /// <param name="serviceType">Required. Type of the service.</param>
        /// <param name="serviceKey">Required. The registration key.</param>
        /// <param name="implementationType">Required. Type of the implementation.</param>
        /// <param name="lifetime">(Optional) The default value is ServiceLifetime.Singleton.</param>
        /// <returns>
        ///     The original <paramref name="serviceCollection" />, for fluent chaining.
        /// </returns>
        /// =================================================================================================
        public static IServiceCollection AddUniqueKeyed(this IServiceCollection serviceCollection,
            Type serviceType, object serviceKey, Type implementationType,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            InternalKeyedSupport.ThrowIfNotSupported();
            serviceType.IfNullThrowArgumentNullException(nameof(serviceType));
            implementationType.IfNullThrowArgumentNullException(nameof(implementationType));
            InternalKeyedSupport.SCValidateServiceKey(serviceKey, nameof(serviceKey));
            lifetime.SCValidateLifetime(nameof(lifetime));

            serviceCollection.SCRemoveAllKeyed(serviceType, serviceKey);
            serviceCollection.Add(InternalKeyedSupport.SCDescribeKeyed(
                serviceType, serviceKey, implementationType, lifetime));

            return serviceCollection;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Registers <typeparamref name="TImplementing" /> under <paramref name="serviceKey" /> when no
        ///     registration of <typeparamref name="TService" /> exists for that key (first-wins).
        /// </summary>
        /// <remarks>
        ///     Nothing is ever removed. A registration under a different key, or the non-keyed registration
        ///     of the same service type, does not block the add — only an existing registration under the
        ///     same key does.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="serviceCollection" /> or <paramref name="serviceKey" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="serviceKey" /> is the resolution-time wildcard key.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when the loaded DependencyInjection.Abstractions predates 8.0.
        /// </exception>
        /// <typeparam name="TService">Type of service that will be added.</typeparam>
        /// <typeparam name="TImplementing">Type of service implementation.</typeparam>
        /// <param name="serviceCollection">Required. Service collection.</param>
        /// <param name="serviceKey">Required. The registration key.</param>
        /// <param name="lifetime">(Optional) The default value is ServiceLifetime.Singleton.</param>
        /// <returns>
        ///     True when the registration was added; false when one already existed for that key.
        /// </returns>
        /// =================================================================================================
        public static bool TryAddUniqueKeyed<TService, TImplementing>(this IServiceCollection serviceCollection,
            object serviceKey, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
            where TImplementing : class, TService
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            InternalKeyedSupport.ThrowIfNotSupported();
            InternalKeyedSupport.SCValidateServiceKey(serviceKey, nameof(serviceKey));
            lifetime.SCValidateLifetime(nameof(lifetime));

            if (serviceCollection.SCHasAnyKeyed(typeof(TService), serviceKey))
                return false;

            serviceCollection.Add(InternalKeyedSupport.SCDescribeKeyed(
                typeof(TService), serviceKey, typeof(TImplementing), lifetime));

            return true;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Registers <typeparamref name="TService" /> as its own implementation under
        ///     <paramref name="serviceKey" /> when no registration exists for that key (first-wins).
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="serviceCollection" /> or <paramref name="serviceKey" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="serviceKey" /> is the resolution-time wildcard key.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when the loaded DependencyInjection.Abstractions predates 8.0.
        /// </exception>
        /// <typeparam name="TService">Type of the service. Must be a concrete, instantiable type.</typeparam>
        /// <param name="serviceCollection">Required. Service collection.</param>
        /// <param name="serviceKey">Required. The registration key.</param>
        /// <param name="lifetime">(Optional) The default value is ServiceLifetime.Singleton.</param>
        /// <returns>
        ///     True when the registration was added; false when one already existed for that key.
        /// </returns>
        /// =================================================================================================
        public static bool TryAddUniqueKeyed<TService>(this IServiceCollection serviceCollection,
            object serviceKey, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            InternalKeyedSupport.ThrowIfNotSupported();
            InternalKeyedSupport.SCValidateServiceKey(serviceKey, nameof(serviceKey));
            lifetime.SCValidateLifetime(nameof(lifetime));

            if (serviceCollection.SCHasAnyKeyed(typeof(TService), serviceKey))
                return false;

            serviceCollection.Add(InternalKeyedSupport.SCDescribeKeyed(
                typeof(TService), serviceKey, typeof(TService), lifetime));

            return true;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Registers <typeparamref name="TService" /> with the supplied factory under
        ///     <paramref name="serviceKey" /> when no registration exists for that key (first-wins).
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="serviceCollection" />, <paramref name="serviceKey" /> or
        ///     <paramref name="factory" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="serviceKey" /> is the resolution-time wildcard key.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when the loaded DependencyInjection.Abstractions predates 8.0.
        /// </exception>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceCollection">Required. Service collection.</param>
        /// <param name="serviceKey">Required. The registration key.</param>
        /// <param name="factory">
        ///     Required. Factory receiving the service provider and the key the service was resolved with.
        /// </param>
        /// <param name="lifetime">(Optional) The default value is ServiceLifetime.Singleton.</param>
        /// <returns>
        ///     True when the registration was added; false when one already existed for that key.
        /// </returns>
        /// =================================================================================================
        public static bool TryAddUniqueKeyed<TService>(this IServiceCollection serviceCollection,
            object serviceKey, Func<IServiceProvider, object, TService> factory,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            InternalKeyedSupport.ThrowIfNotSupported();
            InternalKeyedSupport.SCValidateServiceKey(serviceKey, nameof(serviceKey));
            factory.IfNullThrowArgumentNullException(nameof(factory));
            lifetime.SCValidateLifetime(nameof(lifetime));

            if (serviceCollection.SCHasAnyKeyed(typeof(TService), serviceKey))
                return false;

            serviceCollection.Add(InternalKeyedSupport.SCDescribeKeyed(
                typeof(TService), serviceKey, factory, lifetime));

            return true;
        }
    }
}
