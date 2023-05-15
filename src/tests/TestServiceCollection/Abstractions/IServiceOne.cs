// ***********************************************************************
//  Assembly         : RzR.Shared.Services.TestServiceCollection
//  Author           : RzR
//  Created On       : 2023-05-12 19:05
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-05-12 19:05
// ***********************************************************************
//  <copyright file="IServiceOne.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

using System.Threading.Tasks;

namespace TestServiceCollection.Abstractions
{
    public interface IServiceOne
    {
        Task DoTask();
    }
}