// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2026-06-08 21:50
//
//  Last Modified By : RzR
//  Last Modified On : 2026-06-09 21:50
// ***********************************************************************
//  <copyright file="ReplaceUniqueExtensions.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
//
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.Extensions.DependencyInjection;
using RzR.Extensions.UniqueServiceCollection.Extensions;

#endregion

namespace RzR.Extensions.UniqueServiceCollection.ServiceCollectionExtensions
{
    /// <summary>
    ///     Replace-unique extensions that remove any prior registrations and register a new one (last-wins).
    /// </summary>
    /// <remarks>
    ///     Unlike the .NET <c>TryAdd*</c> convention (first-wins / add only if absent), these methods
    ///     always replace existing registrations. The returned <c>bool</c> indicates whether a prior
    ///     registration was present and replaced.
    /// </remarks>
    public static class ReplaceUniqueExtensions
    {
        /// <summary>
        ///     Removes all existing registrations for <typeparamref name="TService" /> and registers
        ///     <typeparamref name="TImplementing" /> as the sole implementation (last-wins / replace semantics).
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="lifetime" /> is outside the accepted range.
        /// </exception>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <typeparam name="TImplementing">Implementation type.</typeparam>
        /// <param name="serviceCollection">Service collection.</param>
        /// <param name="lifetime">(Optional) Defaults to Singleton.</param>
        /// <returns>
        ///     True when a previous registration existed and was replaced; false when this is the first
        ///     registration for <typeparamref name="TService" />.
        /// </returns>
        public static bool ReplaceUnique<TService, TImplementing>(this IServiceCollection serviceCollection,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
            where TImplementing : class, TService
        {
            serviceCollection.IfNullThrowArgumentNullException(nameof(serviceCollection));
            lifetime.SCValidateLifetime(nameof(lifetime));

            var replaced = serviceCollection.SCHasAny<TService>();

            serviceCollection.SCRemoveAllIfHasAny<TService>();
            serviceCollection.SCAddIfHasNoAny(typeof(TService), typeof(TImplementing), lifetime);

            return replaced;
        }
    }
}
