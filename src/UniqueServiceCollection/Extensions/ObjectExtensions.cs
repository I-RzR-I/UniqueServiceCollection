// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2023-05-12 19:24
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-05-12 19:24
// ***********************************************************************
//  <copyright file="ObjectExtensions.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using CodeSource;

#endregion

namespace UniqueServiceCollection.Extensions
{
    /// <summary>
    ///     Object extensions
    /// </summary>
    /// <remarks></remarks>
    internal static class ObjectExtensions
    {
        /// <summary>
        ///     Is null
        /// </summary>
        /// <param name="obj">Source data</param>
        /// <returns></returns>
        [CodeSource("https://github.com/I-RzR-I/DomainCommonExtensions", "RzR",
            "DomainCommonExtensions.CommonExtensions.NullExtensions.IsNull", 1)]
        internal static bool IsNull(this object obj) => obj == null;
    }
}