// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2025-06-17 16:40
// 
//  Last Modified By : RzR
//  Last Modified On : 2025-06-17 19:00
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

namespace UniqueServiceCollection.Extensions
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     An service collection extensions.
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
        internal static bool SCHasAny(this ServiceCollection serviceCollection, Type collectionType)
        {
            if (collectionType.IsNull())
                throw new ArgumentNullException(nameof(collectionType));

            return serviceCollection.IsNotNull() && serviceCollection.Any(x => x.ServiceType == collectionType);
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
        internal static bool SCHasNoAny(this ServiceCollection serviceCollection, Type collectionType)
            => serviceCollection.IsNotNull() && !serviceCollection.SCHasAny(collectionType);

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
            if (collectionType.IsNull())
                throw new ArgumentNullException(nameof(collectionType));

            return serviceCollection.IsNotNull() && serviceCollection.Any(x => x.ServiceType == collectionType);
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
            => serviceCollection.IsNotNull() && !serviceCollection.SCHasAny(collectionType);

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
        internal static bool SCHasAny<TService>(this ServiceCollection serviceCollection)
            where TService : class
            => serviceCollection.IsNotNull() && serviceCollection.Any(x => x.ServiceType == typeof(TService));

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
        internal static bool SCHasNoAny<TService>(this ServiceCollection serviceCollection)
            where TService : class
            => serviceCollection.IsNotNull() && !serviceCollection.SCHasAny<TService>();

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
            => serviceCollection.IsNotNull() && serviceCollection.Any(x => x.ServiceType == typeof(TService));

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
            => serviceCollection.IsNotNull() && !serviceCollection.SCHasAny<TService>();

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
        internal static void SCIfHasAny(this ServiceCollection serviceCollection, Type collectionType, Action executeAction)
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (collectionType.IsNull())
                throw new ArgumentNullException(nameof(collectionType));

            if (executeAction.IsNull())
                throw new ArgumentNullException(nameof(executeAction));

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
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="collectionType">Type of the collection.</param>
        /// <param name="executeAction">The execute action.</param>
        /// =================================================================================================
        internal static void SCIfHasAny(this IServiceCollection serviceCollection, Type collectionType, Action executeAction)
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (collectionType.IsNull())
                throw new ArgumentNullException(nameof(collectionType));

            if (executeAction.IsNull())
                throw new ArgumentNullException(nameof(executeAction));

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
        internal static void SCIfHasAny<TService>(this ServiceCollection serviceCollection, Action executeAction)
            where TService : class
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (executeAction.IsNull())
                throw new ArgumentNullException(nameof(executeAction));

            if (serviceCollection.SCHasAny<TService>()) 
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
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (executeAction.IsNull())
                throw new ArgumentNullException(nameof(executeAction));

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
        internal static void SCRemoveAllIfHasAny(this ServiceCollection serviceCollection, Type collectionType)
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (collectionType.IsNull())
                throw new ArgumentNullException(nameof(collectionType));

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
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="collectionType">Type of the collection.</param>
        /// =================================================================================================
        internal static void SCRemoveAllIfHasAny(this IServiceCollection serviceCollection, Type collectionType)
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (collectionType.IsNull())
                throw new ArgumentNullException(nameof(collectionType));

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
        internal static void SCRemoveAllIfHasAny<TService>(this ServiceCollection serviceCollection)
            where TService : class
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceCollection.SCHasAny<TService>()) 
                serviceCollection.RemoveAll<TService>();
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
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceCollection.SCHasAny<TService>()) 
                serviceCollection.RemoveAll<TService>();
        }
    }
}