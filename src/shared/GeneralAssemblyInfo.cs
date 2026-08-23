// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2023-05-12 08:36
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-05-12 17:10
// ***********************************************************************
//  <copyright file="GeneralAssemblyInfo.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Reflection;
using System.Resources;

#endregion

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyCompany("RzR ®")]
[assembly: AssemblyProduct("UniqueServiceCollection")]
[assembly: AssemblyCopyright("Copyright © 2022-2026 RzR All rights reserved.")]
[assembly: AssemblyTrademark("® RzR™")]
[assembly: AssemblyDescription("A product that can add, check and remove/avoid multiple duplicate services injection in your project.")]

[assembly: AssemblyMetadata("TermsOfService", "")]
[assembly: AssemblyMetadata("ContactUrl", "")]
[assembly: AssemblyMetadata("ContactName", "RzR")]
[assembly: AssemblyMetadata("ContactEmail", "ddpRzR@hotmail.com")]

[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]

[assembly: AssemblyVersion("3.0.0.8109")]
[assembly: AssemblyFileVersion("3.0.0.8109")]
[assembly: AssemblyInformationalVersion("3.0.0.8109")]
