// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2026-08-14 15:20
//
//  Last Modified By : RzR
//  Last Modified On : 2026-08-23 23:53
// ***********************************************************************
//  <copyright file="InternalKeyedServiceCollectionExtensions.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
//
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.Extensions.DependencyInjection;
using RzR.Extensions.UniqueServiceCollection.Helpers;
using System;
using System.Linq;

// ReSharper disable InconsistentNaming

#endregion

namespace RzR.Extensions.UniqueServiceCollection.Extensions
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Service collection helpers scoped to keyed registrations.
    /// </summary>
    /// =================================================================================================
    internal static class InternalKeyedServiceCollectionExtensions
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A ServiceDescriptor extension method that queries whether the descriptor is a keyed
        ///     registration of the given service type under the given key.
        /// </summary>
        /// <param name="descriptor">The descriptor to act on.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceKey">The registration key.</param>
        /// <returns>
        ///     True when the descriptor matches both the service type and the key.
        /// </returns>
        /// =================================================================================================
        internal static bool SCMatchesKeyed(this ServiceDescriptor descriptor, Type serviceType, object serviceKey)
            => descriptor.SCIsKeyed()
               && descriptor.ServiceType == serviceType
               && Equals(descriptor.SCGetServiceKey(), serviceKey);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IServiceCollection extension method that queries whether any registration exists for the
        ///     given service type under the given key.
        /// </summary>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceKey">The registration key.</param>
        /// <returns>
        ///     True if any, false if not.
        /// </returns>
        /// =================================================================================================
        internal static bool SCHasAnyKeyed(this IServiceCollection serviceCollection, Type serviceType, object serviceKey)
            => serviceCollection.IsNotNull()
               && serviceCollection.Any(x => x.SCMatchesKeyed(serviceType, serviceKey));

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IServiceCollection extension method that counts registrations for the given service type
        ///     under the given key.
        /// </summary>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceKey">The registration key.</param>
        /// <returns>
        ///     An int.
        /// </returns>
        /// =================================================================================================
        internal static int SCCountByKeyed(this IServiceCollection serviceCollection, Type serviceType, object serviceKey)
            => serviceCollection.IsNull()
                ? 0
                : serviceCollection.Count(x => x.SCMatchesKeyed(serviceType, serviceKey));

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IServiceCollection extension method that removes every registration for the given service
        ///     type under the given key, leaving other keys and the non-keyed registration untouched.
        /// </summary>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceKey">The registration key.</param>
        /// =================================================================================================
        internal static void SCRemoveAllKeyed(this IServiceCollection serviceCollection, Type serviceType, object serviceKey)
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));

            for (var i = serviceCollection.Count - 1; i >= 0; i--)
            {
                if (serviceCollection[i].SCMatchesKeyed(serviceType, serviceKey))
                    serviceCollection.RemoveAt(i);
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IServiceCollection extension method that validates a lifetime value.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when the lifetime is outside the accepted range.
        /// </exception>
        /// <param name="lifetime">The lifetime.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// =================================================================================================
        internal static void SCValidateLifetime(this ServiceLifetime lifetime, string paramName)
        {
            if (lifetime != ServiceLifetime.Singleton
                && lifetime != ServiceLifetime.Scoped
                && lifetime != ServiceLifetime.Transient)
                lifetime.ThrowArgumentOutOfRangeException(paramName);
        }
    }
}
