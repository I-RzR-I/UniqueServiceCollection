// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2023-05-12 17:23
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-05-14 20:10
// ***********************************************************************
//  <copyright file="MonitoringUniqueCollectionExtension.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UniqueServiceCollection.DTO;
using UniqueServiceCollection.Extensions;

#endregion

namespace UniqueServiceCollection
{
    /// <summary>
    ///     Monitoring duplicate collection
    /// </summary>
    /// <remarks>Check status and eliminate duplicate service injection</remarks>
    public static class MonitoringUniqueCollectionExtension
    {
        /// <summary>
        ///     Check injected service for duplication and remove it
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <typeparam name="TService">Current injected service</typeparam>
        /// <remarks>Find duplicate injections and leave only one</remarks>
        public static void CheckAndCleanUpDuplicateService<TService>(this IServiceCollection serviceCollection)
            where TService : class
        {
            var duplicates = serviceCollection.FindServiceDuplicate<TService>().ToList();
            if (duplicates.IsNull() || !duplicates.Any())
                return;
            foreach (var d in duplicates)
            {
                serviceCollection.RemoveAll(d.ServiceDescriptor.ServiceType);
                serviceCollection.Add(d.ServiceDescriptor);
            }
        }

        /// <summary>
        ///     Check injected service for duplication and remove it
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <typeparam name="TService">Current injected service</typeparam>
        /// <remarks>Find duplicate injections and leave only one</remarks>
        public static void CheckAndCleanUpDuplicateService<TService>(this ServiceCollection serviceCollection)
            where TService : class
        {
            var duplicates = serviceCollection.FindServiceDuplicate<TService>().ToList();
            if (duplicates.IsNull() || !duplicates.Any())
                return;
            foreach (var d in duplicates)
            {
                serviceCollection.RemoveAll(d.ServiceDescriptor.ServiceType);
                serviceCollection.Add(d.ServiceDescriptor);
            }
        }

        /// <summary>
        ///     Find all duplicate service
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <returns>Return list of duplicate services</returns>
        /// <remarks>Search in  all service</remarks>
        public static IEnumerable<DuplicateServicesDto> FindServiceDuplicate(this IServiceCollection serviceCollection)
            => serviceCollection
                .GroupBy(x => x.ServiceType)
                .Select(a => new DuplicateServicesDto { Count = a.Count(), ServiceDescriptor = a.FirstOrDefault() })
                .Where(x => x.Count > 1);

        /// <summary>
        ///     Find all duplicate service
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <returns>Return list of duplicate services</returns>
        /// <remarks>Search in  all service</remarks>
        public static IEnumerable<DuplicateServicesDto> FindServiceDuplicate(this ServiceCollection serviceCollection)
            => serviceCollection
                .GroupBy(x => x.ServiceType)
                .Select(a => new DuplicateServicesDto { Count = a.Count(), ServiceDescriptor = a.FirstOrDefault() })
                .Where(x => x.Count > 1);

        /// <summary>
        ///     Find duplicate service
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <returns>Return list of duplicate services</returns>
        /// <remarks>Search in  all service</remarks>
        public static IEnumerable<DuplicateServicesDto> FindServiceDuplicate<TService>(this IServiceCollection serviceCollection)
            where TService : class
            => serviceCollection
                .GroupBy(x => x.ServiceType)
                .Select(a => new DuplicateServicesDto { Count = a.Count(), ServiceDescriptor = a.FirstOrDefault() })
                .Where(x => x.ServiceDescriptor.ServiceType == typeof(TService) && x.Count > 1);

        /// <summary>
        ///     Find duplicate service
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <returns>Return list of duplicate services</returns>
        /// <remarks>Search in  all service</remarks>
        public static IEnumerable<DuplicateServicesDto> FindServiceDuplicate<TService>(this ServiceCollection serviceCollection)
            where TService : class
            => serviceCollection
                .GroupBy(x => x.ServiceType)
                .Select(a => new DuplicateServicesDto { Count = a.Count(), ServiceDescriptor = a.FirstOrDefault() })
                .Where(x => x.ServiceDescriptor.ServiceType == typeof(TService) && x.Count > 1);
    }
}