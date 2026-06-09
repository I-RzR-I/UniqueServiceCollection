// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2025-06-18 23:54
// 
//  Last Modified By : RzR
//  Last Modified On : 2025-06-18 23:55
// ***********************************************************************
//  <copyright file="InternalObjectExceptionExtensions.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;

#endregion

namespace RzR.Extensions.UniqueServiceCollection.Extensions
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     An internal object exception extensions.
    /// </summary>
    /// =================================================================================================
    internal static class InternalObjectExceptionExtensions
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An object extension method that if null throw argument null exception.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <param name="sourceObj">The sourceObj to act on.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// =================================================================================================
        internal static void IfNullThrowArgumentNullException(this object sourceObj, string paramName)
        {
            if (sourceObj.IsNull())
                throw new ArgumentNullException(paramName);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An object extension method that if null throw argument null exception.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <param name="sourceObj">The sourceObj to act on.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="exception">The exception.</param>
        /// =================================================================================================
        internal static void IfNullThrowArgumentNullException(this object sourceObj, string paramName, Exception exception)
        {
            if (sourceObj.IsNull())
                throw new ArgumentNullException(paramName, exception);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An object extension method that throw argument out of range exception.
        /// </summary>
        /// <param name="sourceObj">The sourceObj to act on.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// =================================================================================================
        internal static void ThrowArgumentOutOfRangeException(this object sourceObj, string paramName) 
            => throw new ArgumentOutOfRangeException(paramName, sourceObj, "Supplied parameter is not supported(is out of accepted range)!");

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An object extension method that throw argument out of range exception.
        /// </summary>
        /// <param name="sourceObj">The sourceObj to act on.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="exceptionMessage">Message describing the exception.</param>
        /// =================================================================================================
        internal static void ThrowArgumentOutOfRangeException(this object sourceObj, string paramName, string exceptionMessage) 
            => throw new ArgumentOutOfRangeException(paramName, sourceObj, exceptionMessage);
    }
}