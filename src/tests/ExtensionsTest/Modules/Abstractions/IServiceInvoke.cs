// ***********************************************************************
//  Assembly         : RzR.Shared.Services.ExtensionsTest
//  Author           : RzR
//  Created On       : 2025-06-17 18:34
// 
//  Last Modified By : RzR
//  Last Modified On : 2025-06-17 18:38
// ***********************************************************************
//  <copyright file="IServiceInvoke.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Threading.Tasks;

#endregion

namespace ExtensionsTest.Modules.Abstractions
{
    public interface IServiceInvoke
    {
        Task DoTask1();
        Task DoTask2();
    }
}