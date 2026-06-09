// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2025-06-17 22:42
// 
//  Last Modified By : RzR
//  Last Modified On : 2025-06-17 22:48
// ***********************************************************************
//  <copyright file="InternalCollectionExtensions.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Collections.Generic;
using System.Linq;

#endregion

namespace RzR.Extensions.UniqueServiceCollection.Extensions
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     A data collection extensions.
    /// </summary>
    /// =================================================================================================
    internal static class InternalCollectionExtensions
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IEnumerable&lt;T&gt; extension method that query if 'collection' has any.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        /// <returns>
        ///     True if any, false if not.
        /// </returns>
        /// =================================================================================================
        internal static bool HasAnyInCollection<T>(this IEnumerable<T> collection)
            => collection.IsNotNull() && collection.Any();

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IEnumerable&lt;T&gt; extension method that query if 'collection' has no any.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        /// <returns>
        ///     True if no any, false if not.
        /// </returns>
        /// =================================================================================================
        internal static bool HasNoAnyInCollection<T>(this IEnumerable<T> collection)
            => !collection.HasAnyInCollection();
    }
}