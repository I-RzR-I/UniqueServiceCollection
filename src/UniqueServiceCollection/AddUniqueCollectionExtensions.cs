﻿// ***********************************************************************
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

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UniqueServiceCollection.Extensions;

#endregion

namespace UniqueServiceCollection
{
    /// <summary>
    ///     Add unique service to application service collection.
    /// </summary>
    /// <remarks>All previous defined service of specified type will be removed from collection.</remarks>
    public static partial class AddUniqueCollectionExtensions
    {
        /// <summary>
        ///     Add unique service of the type <typeparamref name="TService" />
        ///     with his implementation on the specified type <typeparamref name="TImplementing" />
        ///     to current <see cref="IServiceCollection" />.
        /// </summary>
        /// <example>
        ///     serviceCollection.AddUnique&lt;IServiceOne, ServiceOne&gt;();
        ///     serviceCollection.AddUnique&lt;IServiceOne, ServiceOne&gt;(ServiceLifetime.Scoped);
        /// </example>
        /// <param name="serviceCollection">Required. Service collection </param>
        /// <param name="lifetime">Optional. The default value is ServiceLifetime.Singleton.</param>
        /// <typeparam name="TService">Type of service that will be added</typeparam>
        /// <typeparam name="TImplementing">Type of service implementation</typeparam>
        /// <remarks>
        ///     Before add new service instance of the type <typeparamref name="TService" />,
        ///     all previously  defined services will be removed.
        /// </remarks>
        public static void AddUnique<TService, TImplementing>(
            this IServiceCollection serviceCollection, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
            where TImplementing : class, TService
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceCollection.Any(x => x.ServiceType == typeof(TService)))
                //Remove all type of TService
                serviceCollection.RemoveAll<TService>();

            //Add new service instance
            serviceCollection.Add(ServiceDescriptor.Describe(typeof(TService), typeof(TImplementing), lifetime));
        }
        
        /// <summary>
        ///     Add unique service of the type <typeparamref name="TService" />
        ///     with his implementation on the specified type <typeparamref name="TImplementing" />
        ///     to current <see cref="ServiceCollection" />.
        /// </summary>
        /// <example>
        ///     serviceCollection.AddUnique&lt;IServiceOne, ServiceOne&gt;();
        ///     serviceCollection.AddUnique&lt;IServiceOne, ServiceOne&gt;(ServiceLifetime.Scoped);
        /// </example>
        /// <param name="serviceCollection">Required. Service collection </param>
        /// <param name="lifetime">Optional. The default value is ServiceLifetime.Singleton.</param>
        /// <typeparam name="TService">Type of service that will be added</typeparam>
        /// <typeparam name="TImplementing">Type of service implementation</typeparam>
        /// <remarks>
        ///     Before add new service instance of the type <typeparamref name="TService" />,
        ///     all previously  defined services will be removed.
        /// </remarks>
        public static void AddUnique<TService, TImplementing>(
            this ServiceCollection serviceCollection, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
            where TImplementing : class, TService
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceCollection.Any(x => x.ServiceType == typeof(TService)))
                //Remove all type of TService
                serviceCollection.RemoveAll<TService>();

            //Add new service instance
            serviceCollection.Add(ServiceDescriptor.Describe(typeof(TService), typeof(TImplementing), lifetime));
        }

        /// <summary>
        ///     Add unique service of the type <typeparamref name="TService" /> with specified  factory
        ///     to current <see cref="IServiceCollection" />.
        /// </summary>
        /// <example>
        ///     serviceCollection.AddUnique&lt;IService&gt;(factory =>
        ///     {
        ///     IHostingEnvironment hostingEnvironment = factory.GetRequiredService&lt;IHostingEnvironment&gt;();
        ///     return new OsHelper(hostingEnvironment);
        ///     });
        /// </example>
        /// <param name="serviceCollection">Required. Service collection</param>
        /// <param name="factory">Required. Service provider factory</param>
        /// <param name="lifetime">Optional. The default value is ServiceLifetime.Singleton.</param>
        /// <typeparam name="TService">Type of service that will be added</typeparam>
        /// <remarks>
        ///     Before add new service instance of the type <typeparamref name="TService" />,
        ///     all previously  defined services will be removed.
        /// </remarks>
        public static void AddUnique<TService>(
            this IServiceCollection serviceCollection,
            Func<IServiceProvider, TService> factory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (factory.IsNull())
                throw new ArgumentNullException(nameof(factory));

            if (serviceCollection.Any(x => x.ServiceType == typeof(TService)))
                //Remove all type of TService
                serviceCollection.RemoveAll<TService>();

            //Add new service instance
            serviceCollection.Add(ServiceDescriptor.Describe(typeof(TService), factory, lifetime));
        }

        /// <summary>
        ///     Add unique service of the type <typeparamref name="TService" /> with specified  factory
        ///     to current <see cref="ServiceCollection" />.
        /// </summary>
        /// <example>
        ///     serviceCollection.AddUnique&lt;IService&gt;(factory =>
        ///     {
        ///     IHostingEnvironment hostingEnvironment = factory.GetRequiredService&lt;IHostingEnvironment&gt;();
        ///     return new OsHelper(hostingEnvironment);
        ///     });
        /// </example>
        /// <param name="serviceCollection">Required. Service collection</param>
        /// <param name="factory">Required. Service provider factory</param>
        /// <param name="lifetime">Optional. The default value is ServiceLifetime.Singleton.</param>
        /// <typeparam name="TService">Type of service that will be added</typeparam>
        /// <remarks>
        ///     Before add new service instance of the type <typeparamref name="TService" />,
        ///     all previously  defined services will be removed.
        /// </remarks>
        public static void AddUnique<TService>(
            this ServiceCollection serviceCollection,
            Func<IServiceProvider, TService> factory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (factory.IsNull())
                throw new ArgumentNullException(nameof(factory));

            if (serviceCollection.Any(x => x.ServiceType == typeof(TService)))
                //Remove all type of TService
                serviceCollection.RemoveAll<TService>();

            //Add new service instance
            serviceCollection.Add(ServiceDescriptor.Describe(typeof(TService), factory, lifetime));
        }
    }
}