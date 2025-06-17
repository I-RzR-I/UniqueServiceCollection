// ***********************************************************************
//  Assembly         : RzR.Shared.Services.ExtensionsTest
//  Author           : RzR
//  Created On       : 2025-06-17 18:36
// 
//  Last Modified By : RzR
//  Last Modified On : 2025-06-17 18:38
// ***********************************************************************
//  <copyright file="ServiceInvokeOne.cs" company="RzR SOFT & TECH">
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
    public class ServiceInvokeOne : IServiceInvokeOne
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