// ***********************************************************************
//  Assembly         : RzR.Shared.Services.ExtensionsTest
//  Author           : RzR
//  Created On       : 2026-06-08 00:00
//
//  Last Modified By : RzR
//  Last Modified On : 2026-06-08 00:00
// ***********************************************************************
//  <copyright file="ServiceInvokeAlt.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
//
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Threading.Tasks;
using ExtensionsTest.Modules.Abstractions;

#endregion

namespace ExtensionsTest.Modules.Services
{
    /// <summary>
    ///     Alternate implementation of IServiceInvoke used to test that distinct multi-registrations
    ///     are preserved by the duplicate-cleanup logic.
    /// </summary>
    public class ServiceInvokeAlt : IServiceInvoke
    {
        /// <inheritdoc />
        public async Task DoTask1()
        {
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task DoTask2()
        {
            await Task.CompletedTask;
        }
    }
}
