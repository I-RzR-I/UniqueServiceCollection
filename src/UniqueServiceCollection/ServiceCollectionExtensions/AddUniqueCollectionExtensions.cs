// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2023-05-12 14:52
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-05-14 23:28
// ***********************************************************************
//  <copyright file="AddUniqueCollectionExtensions.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.Extensions.DependencyInjection;
using System;
using UniqueServiceCollection.Extensions;

#endregion

namespace UniqueServiceCollection.ServiceCollectionExtensions
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Add unique service to application service collection.
    /// </summary>
    /// <remarks>
    ///     All previous defined service of specified type will be removed from collection.
    /// </remarks>
    /// =================================================================================================
    public static partial class AddUniqueCollectionExtensions
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Add unique service of the type <typeparamref name="TService" />
        ///     with his implementation on the specified type <typeparamref name="TImplementing" />
        ///     to current <see cref="IServiceCollection" />.
        /// </summary>
        /// <remarks>
        ///     Before add new service instance of the type <typeparamref name="TService" />, all
        ///     previously  defined services will be removed.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <typeparam name="TService">Type of service that will be added.</typeparam>
        /// <typeparam name="TImplementing">Type of service implementation.</typeparam>
        /// <param name="serviceCollection">Required. Service collection.</param>
        /// <param name="lifetime">
        ///     (Optional) Optional. The default value is ServiceLifetime.Singleton.
        /// </param>
        /// <example>
        ///     serviceCollection.AddUnique&lt;IServiceOne, ServiceOne&gt;();
        ///     serviceCollection.AddUnique&lt;IServiceOne, ServiceOne&gt;(ServiceLifetime.Scoped);
        /// 
        /// </example>
        /// =================================================================================================
        public static void AddUnique<TService, TImplementing>(this IServiceCollection serviceCollection,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
            where TImplementing : class, TService
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));

            serviceCollection.SCRemoveAllIfHasAny<TService>();

            //Add new service instance
            serviceCollection.SCAddIfHasNoAny(typeof(TService), typeof(TImplementing), lifetime);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Add unique service of the type <typeparamref name="TService" /> with specified  factory
        ///     to current <see cref="IServiceCollection" />.
        /// </summary>
        /// <remarks>
        ///     Before add new service instance of the type <typeparamref name="TService" />, all
        ///     previously  defined services will be removed.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <typeparam name="TService">Type of service that will be added.</typeparam>
        /// <param name="serviceCollection">Required. Service collection.</param>
        /// <param name="factory">Required. Service provider factory.</param>
        /// <param name="lifetime">
        ///     (Optional) Optional. The default value is ServiceLifetime.Singleton.
        /// </param>
        /// <example>
        ///     serviceCollection.AddUnique&lt;IService&gt;(factory =>
        ///     {
        ///     IHostingEnvironment hostingEnvironment = factory.GetRequiredService&lt;
        ///     IHostingEnvironment&gt;();
        ///     return new OsHelper(hostingEnvironment);
        ///     });
        /// </example>
        /// =================================================================================================
        public static void AddUnique<TService>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TService> factory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            factory.IfNullThrowArgumentNullException(nameof(factory));

            serviceCollection.SCRemoveAllIfHasAny<TService>();

            //Add new service instance
            serviceCollection.SCAddIfHasNoAny(factory, lifetime);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Add unique service of the type <typeparamref name="TService" />
        ///     to current <see cref="IServiceCollection" />.
        /// </summary>
        /// <remarks>
        ///     Before add new service instance of the type <typeparamref name="TService" />, all
        ///     previously  defined services will be removed.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceCollection">Required. Service collection.</param>
        /// <param name="lifetime">Optional. The default value is ServiceLifetime.Singleton.</param>
        /// =================================================================================================
        public static void AddUnique<TService>(this IServiceCollection serviceCollection,
            ServiceLifetime lifetime)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));

            serviceCollection.SCRemoveAllIfHasAny<TService>();

            //Add new service instance
            serviceCollection.SCAddIfHasNoAny(typeof(TService), lifetime);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Add unique service of the type <paramref name="serviceType" />
        ///     to current <see cref="IServiceCollection" />.
        /// </summary>
        /// <remarks>
        ///     Before add new service instance of the type <paramref name="serviceType" />, all
        ///     previously  defined services will be removed.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <param name="serviceCollection">Required. Service collection.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="lifetime">Optional. The default value is ServiceLifetime.Singleton.</param>
        /// =================================================================================================
        public static void AddUnique(this IServiceCollection serviceCollection,
            Type serviceType, ServiceLifetime lifetime)
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));

            serviceCollection.SCRemoveAllIfHasAny(serviceType);

            //Add new service instance
            serviceCollection.SCAddIfHasNoAny(serviceType, lifetime);
        }
    }
}