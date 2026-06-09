// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2026-06-08 00:00
//
//  Last Modified By : RzR
//  Last Modified On : 2026-06-08 00:00
// ***********************************************************************
//  <copyright file="DuplicateServiceReport.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
//
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

#endregion

namespace RzR.Extensions.UniqueServiceCollection.DTO
{
    /// <summary>
    ///     Describes all registrations found for a service type and identifies which one was retained.
    /// </summary>
    /// <remarks>
    ///     A non-empty <see cref="DuplicateRegistrations" /> list means that genuinely identical
    ///     registrations (same ServiceType, ImplementationType/Instance/Factory, and Lifetime) existed
    ///     and were removed. <see cref="RetainedDescriptor" /> is the descriptor that survived.
    ///     Collection properties default to empty arrays so consumers can enumerate safely without null checks.
    /// </remarks>
    public class DuplicateServiceReport
    {
        /// <summary>
        ///     All registrations that existed for the service type at the time the report was generated.
        ///     Never null; defaults to an empty array.
        /// </summary>
        public IReadOnlyList<ServiceDescriptor> AllRegistrations { get; set; } = Array.Empty<ServiceDescriptor>();

        /// <summary>
        ///     The registration that was kept after removing exact duplicates.
        /// </summary>
        public ServiceDescriptor RetainedDescriptor { get; set; }

        /// <summary>
        ///     The registrations that were identified as exact duplicates and removed.
        ///     An empty list means the type had multiple distinct implementations — none were removed.
        ///     Never null; defaults to an empty array.
        /// </summary>
        public IReadOnlyList<ServiceDescriptor> DuplicateRegistrations { get; set; } = Array.Empty<ServiceDescriptor>();
    }
}
