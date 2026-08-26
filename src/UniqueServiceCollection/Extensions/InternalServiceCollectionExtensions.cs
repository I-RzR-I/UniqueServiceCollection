// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2025-06-17 16:40
//
//  Last Modified By : RzR
//  Last Modified On : 2026-08-23 23:53
// ***********************************************************************
//  <copyright file="InternalServiceCollectionExtensions.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

// ReSharper disable InconsistentNaming

#endregion

namespace RzR.Extensions.UniqueServiceCollection.Extensions
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     A service collection extensions.
    /// </summary>
    /// =================================================================================================
    internal static class InternalServiceCollectionExtensions
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A ServiceCollection extension method that query if 'serviceCollection' has any.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="collectionType">Type of the collection.</param>
        /// <returns>
        ///     True if any, false if not.
        /// </returns>
        /// =================================================================================================
        internal static bool SCHasAny(this IServiceCollection serviceCollection, Type collectionType)
        {
            collectionType.IfNullThrowArgumentNullException(nameof(collectionType));

            return serviceCollection.IsNotNull()
                   && serviceCollection.Any(x => x.ServiceType == collectionType && x.SCIsNotKeyed());
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IServiceCollection extension method that query if 'serviceCollection' has no any.
        /// </summary>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="collectionType">Type of the collection.</param>
        /// <returns>
        ///     True if no any, false if not.
        /// </returns>
        /// =================================================================================================
        internal static bool SCHasNoAny(this IServiceCollection serviceCollection, Type collectionType)
            => serviceCollection.IsNull() || !serviceCollection.SCHasAny(collectionType);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A ServiceCollection extension method that query if 'serviceCollection' has any.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <returns>
        ///     True if any, false if not.
        /// </returns>
        /// =================================================================================================
        internal static bool SCHasAny<TService>(this IServiceCollection serviceCollection)
            where TService : class
            => serviceCollection.IsNotNull()
               && serviceCollection.Any(x => x.ServiceType == typeof(TService) && x.SCIsNotKeyed());

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IServiceCollection extension method that screen has no any.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        /// =================================================================================================
        internal static bool SCHasNoAny<TService>(this IServiceCollection serviceCollection)
            where TService : class
            => serviceCollection.IsNull() || !serviceCollection.SCHasAny<TService>();

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A ServiceCollection extension method that screen count by type.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <returns>
        ///     An int.
        /// </returns>
        /// =================================================================================================
        internal static int SCCountByType<TService>(this IServiceCollection serviceCollection)
            where TService : class
        {
            if (serviceCollection.IsNull())
                return 0;

            return serviceCollection.Count(x => x.ServiceType == typeof(TService) && x.SCIsNotKeyed());
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A ServiceCollection extension method that screen count by type.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="collectionType">Type of the collection.</param>
        /// <returns>
        ///     An int.
        /// </returns>
        /// =================================================================================================
        internal static int SCCountByType(this IServiceCollection serviceCollection, Type collectionType)
        {
            collectionType.IfNullThrowArgumentNullException(nameof(collectionType));

            if (serviceCollection.IsNull())
                return 0;

            return serviceCollection.Count(x => x.ServiceType == collectionType && x.SCIsNotKeyed());
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A ServiceCollection extension method that if has any execute action.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="collectionType">Type of the collection.</param>
        /// <param name="executeAction">The execute action.</param>
        /// =================================================================================================
        internal static void SCIfHasAny(this IServiceCollection serviceCollection,
            Type collectionType, Action executeAction)
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            collectionType.IfNullThrowArgumentNullException(nameof(collectionType));
            executeAction.IfNullThrowArgumentNullException(nameof(executeAction));

            if (serviceCollection.SCHasAny(collectionType))
                executeAction.Invoke();
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A ServiceCollection extension method that if has any execute action.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="executeAction">The execute action.</param>
        /// =================================================================================================
        internal static void SCIfHasAny<TService>(this IServiceCollection serviceCollection, Action executeAction)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            executeAction.IfNullThrowArgumentNullException(nameof(executeAction));

            if (serviceCollection.SCHasAny<TService>())
                executeAction.Invoke();
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A ServiceCollection extension method that removes all if has any described by
        ///     serviceCollection.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="collectionType">Type of the collection.</param>
        /// =================================================================================================
        internal static void SCRemoveAllIfHasAny(this IServiceCollection serviceCollection, Type collectionType)
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            collectionType.IfNullThrowArgumentNullException(nameof(collectionType));

            if (serviceCollection.SCHasAny(collectionType))
                serviceCollection.RemoveAll(collectionType);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A ServiceCollection extension method that removes all if has any described by
        ///     serviceCollection.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// =================================================================================================
        internal static void SCRemoveAllIfHasAny<TService>(this IServiceCollection serviceCollection)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));

            if (serviceCollection.SCHasAny<TService>())
                serviceCollection.RemoveAll<TService>();
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IServiceCollection extension method that screen add if has no any.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="lifetime">The lifetime.</param>
        /// =================================================================================================
        internal static void SCAddIfHasNoAny<TService>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TService> factory, ServiceLifetime lifetime)
            where TService : class
        {
            if (serviceCollection.SCHasNoAny(typeof(TService)))
                serviceCollection.Add(ServiceDescriptor.Describe(typeof(TService), factory, lifetime));
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IServiceCollection extension method that screen add if has no any.
        /// </summary>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="lifetime">The lifetime.</param>
        /// =================================================================================================
        internal static void SCAddIfHasNoAny(this IServiceCollection serviceCollection,
            Type serviceType, ServiceLifetime lifetime)
        {
            if (serviceCollection.SCHasNoAny(serviceType))
                serviceCollection.SCAddToServiceCollection(serviceType, lifetime);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IServiceCollection extension method that screen add if has no any.
        /// </summary>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="lifetime">The lifetime.</param>
        /// =================================================================================================
        internal static void SCAddIfHasNoAny(this IServiceCollection serviceCollection,
            Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            if (serviceCollection.SCHasNoAny(serviceType))
                serviceCollection.SCAddToServiceCollection(serviceType, implementationType, lifetime);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IServiceCollection extension method that screen add to service collection.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when one or more arguments are outside the required range.
        /// </exception>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="lifetime">The lifetime.</param>
        /// =================================================================================================
        internal static void SCAddToServiceCollection(this IServiceCollection serviceCollection,
            Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            serviceType.IfNullThrowArgumentNullException(nameof(serviceType));
            implementationType.IfNullThrowArgumentNullException(nameof(implementationType));

            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    serviceCollection.AddSingleton(serviceType, implementationType);
                    break;
                case ServiceLifetime.Scoped:
                    serviceCollection.AddScoped(serviceType, implementationType);
                    break;
                case ServiceLifetime.Transient:
                    serviceCollection.AddTransient(serviceType, implementationType);
                    break;
                default:
                    lifetime.ThrowArgumentOutOfRangeException(nameof(lifetime));
                    break;
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IServiceCollection extension method that screen add to service collection.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when one or more arguments are outside the required range.
        /// </exception>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="lifetime">The lifetime.</param>
        /// =================================================================================================
        internal static void SCAddToServiceCollection(this IServiceCollection serviceCollection,
            Type serviceType, ServiceLifetime lifetime)
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            serviceType.IfNullThrowArgumentNullException(nameof(serviceType));

            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    serviceCollection.AddSingleton(serviceType);
                    break;
                case ServiceLifetime.Scoped:
                    serviceCollection.AddScoped(serviceType);
                    break;
                case ServiceLifetime.Transient:
                    serviceCollection.AddTransient(serviceType);
                    break;
                default:
                    lifetime.ThrowArgumentOutOfRangeException(nameof(lifetime));
                    break;
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