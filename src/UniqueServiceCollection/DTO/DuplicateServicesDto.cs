// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2023-05-13 17:54
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-05-14 23:30
// ***********************************************************************
//  <copyright file="DuplicateServicesDto.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.Extensions.DependencyInjection;

#endregion

namespace UniqueServiceCollection.DTO
{
    /// <summary>
    ///     Duplicate service DTO
    /// </summary>
    public class DuplicateServicesDto
    {
        /// <summary>
        ///     Number of duplicate services
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     Service instance
        /// </summary>
        public ServiceDescriptor ServiceDescriptor { get; set; }
    }
}