// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2023-05-18 08:57
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-05-18 09:04
// ***********************************************************************
//  <copyright file="AddUniqueSingletonExtensions.cs" company="">
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
        ///     Add unique `Singleton` service of the type <paramref name="serviceType" />
        ///     to current <see cref="IServiceCollection" />.
        /// </summary>
        /// <example>
        ///     serviceCollection.AddUnique(f =>
        ///     new TempService(f.GetRequiredService&lt;Service1&gt;(),
        ///     f.GetRequiredService&lt;Service2&gt;()));
        /// </example>
        /// <param name="serviceCollection">Service collection</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="instance">Service instance</param>
        /// <remarks>
        ///     Before add new service instance of the type <paramref name="serviceType" />,
        ///     all previously  defined services will be removed.
        /// </remarks>
        public static void AddUnique(this IServiceCollection serviceCollection, Type serviceType, object instance)
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceType.IsNull())
                throw new ArgumentNullException(nameof(serviceType));

            if (instance.IsNull())
                throw new ArgumentNullException(nameof(instance));

            if (serviceCollection.Any(x => x.ServiceType == serviceType))
                //Remove all type of TService
                serviceCollection.RemoveAll(serviceType);

            //Add new service instance of serviceType
            serviceCollection.AddSingleton(serviceType, instance);
        }

        /// <summary>
        ///     Add unique `Singleton` service of the type <paramref name="serviceType" />
        ///     to current <see cref="ServiceCollection" />.
        /// </summary>
        /// <example>
        ///     serviceCollection.AddUnique(f =>
        ///     new TempService(f.GetRequiredService&lt;Service1&gt;(),
        ///     f.GetRequiredService&lt;Service2&gt;()));
        /// </example>
        /// <param name="serviceCollection">Service collection</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="instance">Service instance</param>
        /// <remarks>
        ///     Before add new service instance of the type <paramref name="serviceType" />,
        ///     all previously  defined services will be removed.
        /// </remarks>
        public static void AddUnique(this ServiceCollection serviceCollection, Type serviceType, object instance)
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceType.IsNull())
                throw new ArgumentNullException(nameof(serviceType));

            if (instance.IsNull())
                throw new ArgumentNullException(nameof(instance));

            if (serviceCollection.Any(x => x.ServiceType == serviceType))
                //Remove all type of TService
                serviceCollection.RemoveAll(serviceType);

            //Add new service instance of serviceType
            serviceCollection.AddSingleton(serviceType, instance);
        }

        /// <summary>
        ///     Add unique `Singleton` service of the type <typeparamref name="TService" />
        ///     to current <see cref="IServiceCollection" />.
        /// </summary>
        /// <example>
        ///     serviceCollection.AddUnique&lt;ITempService&gt;(f =>
        ///     new TempService(f.GetRequiredService&lt;Service1&gt;(),
        ///     f.GetRequiredService&lt;Service2&gt;()));
        /// </example>
        /// <param name="serviceCollection">Service collection</param>
        /// <param name="instance">Service instance</param>
        /// <typeparam name="TService">Type of service that will be added</typeparam>
        /// <remarks>
        ///     Before add new service instance of the type <typeparamref name="TService" />,
        ///     all previously  defined services will be removed.
        /// </remarks>
        public static void AddUnique<TService>(this IServiceCollection serviceCollection, TService instance)
            where TService : class
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (instance.IsNull())
                throw new ArgumentNullException(nameof(instance));

            if (serviceCollection.Any(x => x.ServiceType == typeof(TService)))
                //Remove all type of TService
                serviceCollection.RemoveAll<TService>();

            //Add new service instance
            serviceCollection.AddSingleton(instance);
        }

        /// <summary>
        ///     Add unique `Singleton` service of the type <typeparamref name="TService" />
        ///     to current <see cref="ServiceCollection" />.
        /// </summary>
        /// <example>
        ///     serviceCollection.AddUnique&lt;ITempService&gt;(f =>
        ///     new TempService(f.GetRequiredService&lt;Service1&gt;(),
        ///     f.GetRequiredService&lt;Service2&gt;()));
        /// </example>
        /// <param name="serviceCollection">Service collection</param>
        /// <param name="instance">Service instance</param>
        /// <typeparam name="TService">Type of service that will be added</typeparam>
        /// <remarks>
        ///     Before add new service instance of the type <typeparamref name="TService" />,
        ///     all previously  defined services will be removed.
        /// </remarks>
        public static void AddUnique<TService>(this ServiceCollection serviceCollection, TService instance)
            where TService : class
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (instance.IsNull())
                throw new ArgumentNullException(nameof(instance));

            if (serviceCollection.Any(x => x.ServiceType == typeof(TService)))
                //Remove all type of TService
                serviceCollection.RemoveAll<TService>();

            //Add new service instance
            serviceCollection.AddSingleton(instance);
        }

        /// <summary>
        ///     Add unique `Singleton` service of the type <typeparamref name="TService" />
        ///     to current <see cref="IServiceCollection" />.
        /// </summary>
        /// <example>
        ///     serviceCollection.AddUnique&lt;Service1&gt;();
        /// </example>
        /// <param name="serviceCollection">Service collection</param>
        /// <typeparam name="TService">Type of service that will be added</typeparam>
        /// <remarks>
        ///     Before add new service instance of the type <typeparamref name="TService" />,
        ///     all previously  defined services will be removed.
        /// </remarks>
        public static void AddUnique<TService>(this IServiceCollection serviceCollection)
            where TService : class
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceCollection.Any(x => x.ServiceType == typeof(TService)))
                //Remove all type of TService
                serviceCollection.RemoveAll<TService>();

            //Add new service instance
            serviceCollection.AddSingleton<TService>();
        }

        /// <summary>
        ///     Add unique `Singleton` service of the type <typeparamref name="TService" />
        ///     to current <see cref="ServiceCollection" />.
        /// </summary>
        /// <example>
        ///     serviceCollection.AddUnique&lt;Service1&gt;();
        /// </example>
        /// <param name="serviceCollection">Service collection</param>
        /// <typeparam name="TService">Type of service that will be added</typeparam>
        /// <remarks>
        ///     Before add new service instance of the type <typeparamref name="TService" />,
        ///     all previously  defined services will be removed.
        /// </remarks>
        public static void AddUnique<TService>(this ServiceCollection serviceCollection)
            where TService : class
        {
            if (serviceCollection.IsNull())
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceCollection.Any(x => x.ServiceType == typeof(TService)))
                //Remove all type of TService
                serviceCollection.RemoveAll<TService>();

            //Add new service instance
            serviceCollection.AddSingleton<TService>();
        }
    }
}