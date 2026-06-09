// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2025-06-17 16:26
// 
//  Last Modified By : RzR
//  Last Modified On : 2025-06-18 17:35
// ***********************************************************************
//  <copyright file="RegisterServiceCollectionExtensions.cs" company="RzR SOFT & TECH">
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
    ///     A register service collection extensions.
    /// </summary>
    /// =================================================================================================
    public static class RegisterServiceCollectionExtensions
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A ServiceCollection extension method that registers service if not exist.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TImplementing">Type of the implementing.</typeparam>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="lifetime">(Optional) The lifetime.</param>
        /// =================================================================================================
        public static IServiceCollection RegisterIfNotExist<TService, TImplementing>(this IServiceCollection serviceCollection,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
            where TImplementing : class, TService
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));

            serviceCollection.SCAddIfHasNoAny(typeof(TService), typeof(TImplementing), lifetime);

            return serviceCollection;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A ServiceCollection extension method that registers service if not exist.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="lifetime">(Optional) The lifetime.</param>
        /// =================================================================================================
        public static IServiceCollection RegisterIfNotExist<TService>(this IServiceCollection serviceCollection,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));

            serviceCollection.SCAddIfHasNoAny(typeof(TService), lifetime);

            return serviceCollection;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A ServiceCollection extension method that registers service if not exist.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceCollection">The serviceCollection to act on.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="lifetime">(Optional) The lifetime.</param>
        /// =================================================================================================
        public static IServiceCollection RegisterIfNotExist<TService>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TService> factory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            factory.IfNullThrowArgumentNullException(nameof(factory));

            serviceCollection.SCAddIfHasNoAny(factory, lifetime);

            return serviceCollection;
        }
    }
}