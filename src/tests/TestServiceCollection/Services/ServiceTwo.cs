// ***********************************************************************
//  Assembly         : RzR.Shared.Services.TestServiceCollection
//  Author           : RzR
//  Created On       : 2023-05-12 19:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-05-12 19:04
// ***********************************************************************
//  <copyright file="ServiceOne.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

using System.Threading.Tasks;
using TestServiceCollection.Abstractions;

namespace TestServiceCollection.Services
{
    public class ServiceTwo : IServiceTwo
    {
        /// <inheritdoc />
        public async Task DoTask()
        {
            await Task.CompletedTask;
        }
    }
}