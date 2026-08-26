// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2023-05-12 17:23
//
//  Last Modified By : RzR
//  Last Modified On : 2026-08-23 23:53
// ***********************************************************************
//  <copyright file="MonitoringUniqueCollectionExtension.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
//
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.Extensions.DependencyInjection;
using RzR.Extensions.UniqueServiceCollection.DTO;
using RzR.Extensions.UniqueServiceCollection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace RzR.Extensions.UniqueServiceCollection.ServiceCollectionExtensions
{
    /// <summary>
    ///     Monitoring duplicate collection
    /// </summary>
    /// <remarks>
    ///     A "duplicate" registration is one that shares the same ServiceType, Lifetime, AND the same
    ///     implementation identity (ImplementationType, ImplementationInstance, or ImplementationFactory
    ///     reference). Intentional multi-registration (e.g. multiple distinct implementations of the same
    ///     interface for IEnumerable resolution) is preserved — only genuinely identical registrations are
    ///     collapsed to a single entry.
    /// </remarks>
    public static class MonitoringUniqueCollectionExtension
    {
        /// <summary>
        ///     Check injected service for duplication and remove exact duplicates.
        ///     Distinct implementations of the same service type are always preserved.
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <typeparam name="TService">Current injected service</typeparam>
        /// <returns>The original service collection, for fluent chaining.</returns>
        /// <remarks>
        ///     Only removes registrations that are identical in ServiceType, ImplementationType /
        ///     ImplementationInstance / ImplementationFactory, and Lifetime. Multiple distinct
        ///     implementations registered for the same interface are not touched.
        /// </remarks>
        public static IServiceCollection CheckAndCleanUpDuplicateService<TService>(
            this IServiceCollection serviceCollection)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));

            var report = serviceCollection.BuildDuplicateReport<TService>();
            RemoveDuplicatesFromReport(serviceCollection, new[] { report });

            return serviceCollection;
        }

        /// <summary>
        ///     Clean up exact duplicates across all registered service types.
        ///     Distinct implementations of the same service type are always preserved.
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <returns>The original service collection, for fluent chaining.</returns>
        /// <remarks>
        ///     Scans every service type present in the collection and removes registrations that are
        ///     identical in ServiceType, implementation identity, and Lifetime.
        /// </remarks>
        public static IServiceCollection CheckAndCleanUpAllDuplicates(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));

            var reports = serviceCollection
                .GroupBy(x => x.ServiceType)
                .Select(g => BuildDuplicateReportForGroup(g.Key, g.ToList()))
                .ToList();

            RemoveDuplicatesFromReport(serviceCollection, reports);

            return serviceCollection;
        }

        /// <summary>
        ///     Find all duplicate service registrations (legacy overload).
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <returns>
        ///     Descriptors where the same ServiceType appears more than once, grouped with a count.
        ///     Note: this includes intentional multi-registrations. Use
        ///     <see cref="FindExactDuplicates" /> to find only genuinely identical registrations.
        /// </returns>
        public static IEnumerable<DuplicateServicesDto> FindServiceDuplicate(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));

            return serviceCollection
                .Where(x => x.SCIsNotKeyed())
                .GroupBy(x => x.ServiceType)
                .Where(g => g.Count() > 1)
                .Select(g => new DuplicateServicesDto
                {
                    Count = g.Count(),
                    ServiceDescriptor = g.First()
                });
        }

        /// <summary>
        ///     Find duplicate service for the given type (legacy overload).
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <typeparam name="TService">Service type to inspect</typeparam>
        /// <returns>
        ///     A single-element enumerable when the type is registered more than once (any lifetime or
        ///     implementation), otherwise empty. Use <see cref="FindExactDuplicates{TService}" /> to
        ///     distinguish intentional multi-registrations from true duplicates.
        /// </returns>
        public static IEnumerable<DuplicateServicesDto> FindServiceDuplicate<TService>(
            this IServiceCollection serviceCollection)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));

            return serviceCollection
                .Where(x => x.ServiceType == typeof(TService) && x.SCIsNotKeyed())
                .GroupBy(x => x.ServiceType)
                .Where(g => g.Count() > 1)
                .Select(g => new DuplicateServicesDto
                {
                    Count = g.Count(),
                    ServiceDescriptor = g.First()
                });
        }

        /// <summary>
        ///     Find genuinely identical (exact) duplicate registrations for all service types.
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <returns>
        ///     A <see cref="DuplicateServiceReport" /> for each service type that has at least one
        ///     exact duplicate registration. Types with multiple distinct implementations are not included.
        /// </returns>
        public static IEnumerable<DuplicateServiceReport> FindExactDuplicates(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));

            return serviceCollection
                .GroupBy(x => x.ServiceType)
                .Select(g => BuildDuplicateReportForGroup(g.Key, g.ToList()))
                .Where(r => r.DuplicateRegistrations.Count > 0);
        }

        /// <summary>
        ///     Find genuinely identical (exact) duplicate registrations for a specific service type.
        /// </summary>
        /// <typeparam name="TService">Service type to inspect</typeparam>
        /// <param name="serviceCollection">Service collection</param>
        /// <returns>
        ///     A <see cref="DuplicateServiceReport" /> when exact duplicates exist, otherwise null.
        /// </returns>
        public static DuplicateServiceReport FindExactDuplicates<TService>(
            this IServiceCollection serviceCollection)
            where TService : class
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));

            var report = serviceCollection.BuildDuplicateReport<TService>();
            return report.DuplicateRegistrations.Count > 0 ? report : null;
        }

        /// <summary>
        ///     Validates that the service collection contains no exact duplicate registrations and returns
        ///     the collection unchanged when validation passes, enabling fluent chaining.
        /// </summary>
        /// <param name="serviceCollection">Service collection to validate</param>
        /// <returns>The original <paramref name="serviceCollection" /> for fluent chaining.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="serviceCollection" /> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when at least one exact duplicate registration is detected. The exception message
        ///     lists the affected service type names and their lifetimes. Only genuinely identical
        ///     registrations (same ServiceType, same implementation identity, and same Lifetime) cause
        ///     this exception. Intentional multi-registration of distinct implementations for the same
        ///     interface (the IEnumerable&lt;T&gt; resolution pattern) is not flagged.
        /// </exception>
        /// <remarks>
        ///     This method delegates to <see cref="FindExactDuplicates(IServiceCollection)" /> so the
        ///     duplicate-detection logic is not duplicated. Call this method at the end of your startup
        ///     <see cref="IServiceCollection" /> configuration to fail fast when accidental duplicate
        ///     registrations are present.
        /// </remarks>
        public static IServiceCollection ValidateNoDuplicates(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));

            var reports = serviceCollection.FindExactDuplicates().ToList();

            if (reports.Count == 0)
                return serviceCollection;

            var offenders = string.Join(", ", reports.Select(r =>
                $"{r.RetainedDescriptor?.ServiceType.Name ?? "unknown"} ({r.RetainedDescriptor?.Lifetime})"));

            throw new InvalidOperationException(
                $"Exact duplicate service registrations were detected for: {offenders}. " +
                "Remove the duplicate registrations or call CheckAndCleanUpAllDuplicates() before ValidateNoDuplicates().");
        }

        /// <summary>
        ///     Builds a <see cref="DuplicateServiceReport" /> for a specific service type.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="serviceCollection">Service collection</param>
        private static DuplicateServiceReport BuildDuplicateReport<TService>(
            this IServiceCollection serviceCollection)
            where TService : class
        {
            var registrations = serviceCollection
                .Where(x => x.ServiceType == typeof(TService))
                .ToList();

            return BuildDuplicateReportForGroup(typeof(TService), registrations);
        }

        /// <summary>
        ///     Builds a <see cref="DuplicateServiceReport" /> for a set of registrations belonging to one
        ///     service type. Exact duplicates are identified by comparing Lifetime and implementation
        ///     identity (ImplementationType, ImplementationInstance, or ImplementationFactory).
        /// </summary>
        /// <param name="serviceType">The service type all registrations belong to</param>
        /// <param name="registrations">All registrations for that service type</param>
        /// <remarks>
        ///     Keyed registrations are removed before the analysis starts, so they can never surface as
        ///     <see cref="DuplicateServiceReport.RetainedDescriptor" />, in
        ///     <see cref="DuplicateServiceReport.DuplicateRegistrations" />, or in
        ///     <see cref="DuplicateServiceReport.AllRegistrations" />. Filtering here rather than only
        ///     inside the comparison keeps every caller of this method keyed-blind.
        /// </remarks>
        private static DuplicateServiceReport BuildDuplicateReportForGroup(
            Type serviceType,
            IList<ServiceDescriptor> registrations)
        {
            var candidates = registrations.Where(x => x.SCIsNotKeyed()).ToList();

            var distinct = new List<ServiceDescriptor>();
            var duplicates = new List<ServiceDescriptor>();

            foreach (var descriptor in candidates)
            {
                if (distinct.Any(d => AreExactDuplicates(d, descriptor)))
                    duplicates.Add(descriptor);
                else
                    distinct.Add(descriptor);
            }

            return new DuplicateServiceReport
            {
                AllRegistrations = candidates,
                RetainedDescriptor = distinct.FirstOrDefault(),
                DuplicateRegistrations = duplicates
            };
        }

        /// <summary>
        ///     Removes all exact duplicate descriptors identified in the given reports from the collection.
        /// </summary>
        /// <param name="serviceCollection">Service collection to mutate</param>
        /// <param name="reports">Reports containing the duplicates to remove</param>
        private static void RemoveDuplicatesFromReport(
            IServiceCollection serviceCollection,
            IEnumerable<DuplicateServiceReport> reports)
        {
            foreach (var report in reports)
            {
                foreach (var dup in report.DuplicateRegistrations)
                    serviceCollection.Remove(dup);
            }
        }

        /// <summary>
        ///     Returns true when two service descriptors are genuinely identical: same Lifetime and
        ///     same implementation identity (ImplementationType, ImplementationInstance reference, or
        ///     ImplementationFactory reference). ServiceType is assumed equal by the caller.
        /// </summary>
        /// <param name="a">First descriptor</param>
        /// <param name="b">Second descriptor</param>
        private static bool AreExactDuplicates(ServiceDescriptor a, ServiceDescriptor b)
        {
            if (a.SCIsKeyed() || b.SCIsKeyed())
                return false;

            if (a.Lifetime != b.Lifetime)
                return false;

            if (a.ImplementationType.IsNotNull() || b.ImplementationType.IsNotNull())
                return a.ImplementationType == b.ImplementationType;

            if (a.ImplementationInstance.IsNotNull() || b.ImplementationInstance.IsNotNull())
                return ReferenceEquals(a.ImplementationInstance, b.ImplementationInstance);

            return ReferenceEquals(a.ImplementationFactory, b.ImplementationFactory);
        }
    }
}
