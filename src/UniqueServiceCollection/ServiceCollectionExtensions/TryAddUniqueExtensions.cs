// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2026-08-14 15:20
//
//  Last Modified By : RzR
//  Last Modified On : 2026-08-23 23:53
// ***********************************************************************
//  <copyright file="TryAddUniqueExtensions.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
//
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.Extensions.DependencyInjection;
using RzR.Extensions.UniqueServiceCollection.Extensions;
using System;

#endregion

namespace RzR.Extensions.UniqueServiceCollection.ServiceCollectionExtensions
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Register a service only when its service type is not already registered (first-wins), and
    ///     report whether the registration happened.
    /// </summary>
    /// =================================================================================================
    public static class TryAddUniqueExtensions
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Registers <typeparamref name="TImplementing" /> as the implementation of
        ///     <typeparamref name="TService" /> when no registration of <typeparamref name="TService" />
        ///     exists yet.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="serviceCollection" /> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="lifetime" /> is not a defined lifetime.
        /// </exception>
        /// <typeparam name="TService">Type of service that will be added.</typeparam>
        /// <typeparam name="TImplementing">Type of service implementation.</typeparam>
        /// <param name="serviceCollection">Required. Service collection.</param>
        /// <param name="lifetime">(Optional) The default value is ServiceLifetime.Singleton.</param>
        /// <returns>
        ///     True when the registration was added; false when a registration for
        ///     <typeparamref name="TService" /> already existed and nothing was added.
        /// </returns>
        /// <example>
        ///     <code>
        ///     if (!serviceCollection.TryAddUnique&lt;IServiceOne, ServiceOne&gt;())
        ///         logger.LogWarning("IServiceOne was already registered by another module.");
        ///     </code>
        /// </example>
        /// =================================================================================================
        public static bool TryAddUnique<TService, TImplementing>(this IServiceCollection serviceCollection,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
            where TImplementing : class, TService
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            lifetime.SCValidateLifetime(nameof(lifetime));

            if (serviceCollection.SCHasAny<TService>())
                return false;

            serviceCollection.SCAddToServiceCollection(typeof(TService), typeof(TImplementing), lifetime);

            return true;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Registers <typeparamref name="TService" /> as its own implementation when no registration of
        ///     <typeparamref name="TService" /> exists yet.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="serviceCollection" /> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="lifetime" /> is not a defined lifetime.
        /// </exception>
        /// <typeparam name="TService">Type of the service. Must be a concrete, instantiable type.</typeparam>
        /// <param name="serviceCollection">Required. Service collection.</param>
        /// <param name="lifetime">(Optional) The default value is ServiceLifetime.Singleton.</param>
        /// <returns>
        ///     True when the registration was added; false when one already existed.
        /// </returns>
        /// =================================================================================================
        public static bool TryAddUnique<TService>(this IServiceCollection serviceCollection,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            lifetime.SCValidateLifetime(nameof(lifetime));

            if (serviceCollection.SCHasAny<TService>())
                return false;

            serviceCollection.SCAddToServiceCollection(typeof(TService), lifetime);

            return true;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Registers <typeparamref name="TService" /> with the supplied factory when no registration of
        ///     <typeparamref name="TService" /> exists yet.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="serviceCollection" /> or <paramref name="factory" /> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="lifetime" /> is not a defined lifetime.
        /// </exception>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceCollection">Required. Service collection.</param>
        /// <param name="factory">Required. Service provider factory.</param>
        /// <param name="lifetime">(Optional) The default value is ServiceLifetime.Singleton.</param>
        /// <returns>
        ///     True when the registration was added; false when one already existed.
        /// </returns>
        /// =================================================================================================
        public static bool TryAddUnique<TService>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TService> factory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            factory.IfNullThrowArgumentNullException(nameof(factory));
            lifetime.SCValidateLifetime(nameof(lifetime));

            if (serviceCollection.SCHasAny<TService>())
                return false;

            serviceCollection.Add(ServiceDescriptor.Describe(typeof(TService), factory, lifetime));

            return true;
        }
    }
}
