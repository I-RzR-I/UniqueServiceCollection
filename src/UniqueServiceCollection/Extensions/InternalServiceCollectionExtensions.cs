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
        internal static bool HasAny(this ServiceCollection serviceCollection, Type collectionType)
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (collectionType.IsNull())
                throw new ArgumentNullException(nameof(collectionType));

            return serviceCollection.IsNotNull() && serviceCollection.Any(x => x.ServiceType == collectionType);
        }

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
        internal static bool HasAny<TService>(this ServiceCollection serviceCollection)
            where TService : class
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            return serviceCollection.IsNotNull() && serviceCollection.Any(x => x.ServiceType == typeof(TService));
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
        internal static void IfHasAny(this ServiceCollection serviceCollection, Type collectionType, Action executeAction)
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (collectionType.IsNull())
                throw new ArgumentNullException(nameof(collectionType));

            if (executeAction.IsNull())
                throw new ArgumentNullException(nameof(executeAction));

            if (serviceCollection.HasAny(collectionType)) 
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
        internal static void IfHasAny<TService>(this ServiceCollection serviceCollection, Action executeAction)
            where TService : class
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (executeAction.IsNull())
                throw new ArgumentNullException(nameof(executeAction));

            if (serviceCollection.HasAny<TService>()) 
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
        internal static void RemoveAllIfHasAny(this ServiceCollection serviceCollection, Type collectionType)
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (collectionType.IsNull())
                throw new ArgumentNullException(nameof(collectionType));

            if (serviceCollection.HasAny(collectionType))
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
        internal static void RemoveAllIfHasAny<TService>(this ServiceCollection serviceCollection)
            where TService : class
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceCollection.HasAny<TService>()) 
                serviceCollection.RemoveAll<TService>();
        }
    }
}