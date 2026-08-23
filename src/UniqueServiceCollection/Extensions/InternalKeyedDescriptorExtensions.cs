// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2026-08-14 15:20
//
//  Last Modified By : RzR
//  Last Modified On : 2026-08-23 23:53
// ***********************************************************************
//  <copyright file="InternalKeyedDescriptorExtensions.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
//
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.Extensions.DependencyInjection;

// ReSharper disable InconsistentNaming

#endregion

namespace RzR.Extensions.UniqueServiceCollection.Extensions
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Detection of keyed service descriptors from a netstandard2.0 assembly.
    /// </summary>
    /// =================================================================================================
    internal static class InternalKeyedDescriptorExtensions
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A ServiceDescriptor extension method that queries whether the descriptor is a keyed
        ///     registration.
        /// </summary>
        /// <param name="descriptor">The descriptor to act on.</param>
        /// <returns>
        ///     True when the descriptor represents a keyed service registration, false when it is a
        ///     conventional (non-keyed) registration or when <paramref name="descriptor" /> is null.
        /// </returns>
        /// =================================================================================================
        internal static bool SCIsKeyed(this ServiceDescriptor descriptor)
            => descriptor.IsNotNull()
               && descriptor.ImplementationType.IsNull()
               && descriptor.ImplementationInstance.IsNull()
               && descriptor.ImplementationFactory.IsNull();

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A ServiceDescriptor extension method that queries whether the descriptor is a conventional
        ///     (non-keyed) registration.
        /// </summary>
        /// <param name="descriptor">The descriptor to act on.</param>
        /// <returns>
        ///     True when the descriptor is a non-keyed registration, false when it is keyed.
        /// </returns>
        /// =================================================================================================
        internal static bool SCIsNotKeyed(this ServiceDescriptor descriptor)
            => !descriptor.SCIsKeyed();
    }
}
