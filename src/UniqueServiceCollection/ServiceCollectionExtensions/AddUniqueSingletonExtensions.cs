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

using Microsoft.Extensions.DependencyInjection;
using RzR.Extensions.UniqueServiceCollection.Extensions;
using System;

#endregion

namespace RzR.Extensions.UniqueServiceCollection.ServiceCollectionExtensions
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
        ///     var instance = new TempService();
        ///     serviceCollection.AddUnique(typeof(ITempService), instance);
        /// </example>
        /// <param name="serviceCollection">Service collection</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="instance">Service instance</param>
        /// <remarks>
        ///     Before add new service instance of the type <paramref name="serviceType" />,
        ///     all previously  defined services will be removed.
        /// </remarks>
        public static IServiceCollection AddUnique(this IServiceCollection serviceCollection,
            Type serviceType, object instance)
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            serviceType.IfNullThrowArgumentNullException(nameof(serviceType));
            instance.IfNullThrowArgumentNullException(nameof(instance));

            serviceCollection.SCRemoveAllIfHasAny(serviceType);

            serviceCollection.AddSingleton(serviceType, instance);

            return serviceCollection;
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
        public static IServiceCollection AddUnique<TService>(this IServiceCollection serviceCollection, TService instance)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            instance.IfNullThrowArgumentNullException(nameof(instance));

            serviceCollection.SCRemoveAllIfHasAny<TService>();

            serviceCollection.AddSingleton(typeof(TService), instance);

            return serviceCollection;
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
        public static IServiceCollection AddUnique<TService>(this IServiceCollection serviceCollection)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));

            serviceCollection.SCRemoveAllIfHasAny<TService>();

            serviceCollection.SCAddToServiceCollection(typeof(TService), ServiceLifetime.Singleton);

            return serviceCollection;
        }
    }
}