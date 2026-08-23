// ***********************************************************************
//  Assembly         : RzR.Shared.Services.UniqueServiceCollection
//  Author           : RzR
//  Created On       : 2026-08-14 15:20
//
//  Last Modified By : RzR
//  Last Modified On : 2026-08-23 23:53
// ***********************************************************************
//  <copyright file="InternalKeyedSupport.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
//
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.Extensions.DependencyInjection;
using RzR.Extensions.UniqueServiceCollection.Extensions;
using System;
using System.Linq;
using System.Reflection;

// ReSharper disable InconsistentNaming

#endregion

namespace RzR.Extensions.UniqueServiceCollection.Helpers
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Late-bound access to the keyed dependency injection API.
    /// </summary>
    /// =================================================================================================
    internal static class InternalKeyedSupport
    {
        private static readonly PropertyInfo ServiceKeyProperty;
        private static readonly PropertyInfo KeyedImplementationTypeProperty;
        private static readonly PropertyInfo KeyedImplementationInstanceProperty;
        private static readonly MethodInfo DescribeKeyedByTypeMethod;
        private static readonly MethodInfo DescribeKeyedByFactoryMethod;
        private static readonly object AnyKeyValue;

        static InternalKeyedSupport()
        {
            var descriptorType = typeof(ServiceDescriptor);

            ServiceKeyProperty = descriptorType.GetProperty("ServiceKey");
            KeyedImplementationTypeProperty = descriptorType.GetProperty("KeyedImplementationType");
            KeyedImplementationInstanceProperty = descriptorType.GetProperty("KeyedImplementationInstance");

            var describeKeyed = descriptorType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.Name == "DescribeKeyed" && x.GetParameters().Length == 4)
                .ToList();

            DescribeKeyedByTypeMethod = describeKeyed
                .FirstOrDefault(x => x.GetParameters()[2].ParameterType == typeof(Type));

            DescribeKeyedByFactoryMethod = describeKeyed
                .FirstOrDefault(x => x.GetParameters()[2].ParameterType == typeof(Func<IServiceProvider, object, object>));

            var keyedServiceType = descriptorType.Assembly
                .GetType("Microsoft.Extensions.DependencyInjection.KeyedService");

            if (keyedServiceType.IsNotNull())
                AnyKeyValue = keyedServiceType.GetProperty("AnyKey", BindingFlags.Public | BindingFlags.Static)?.GetValue(null)
                              ?? keyedServiceType.GetField("AnyKey", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Gets a value indicating whether the runtime supports keyed service registration.
        /// </summary>
        /// =================================================================================================
        internal static bool IsSupported
            => ServiceKeyProperty.IsNotNull()
               && DescribeKeyedByTypeMethod.IsNotNull()
               && DescribeKeyedByFactoryMethod.IsNotNull();

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Throws when the loaded runtime cannot support keyed service registration.
        /// </summary>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when the loaded DependencyInjection.Abstractions predates 8.0.
        /// </exception>
        /// =================================================================================================
        internal static void ThrowIfNotSupported()
        {
            if (IsSupported)
                return;

            throw new PlatformNotSupportedException(
                "Keyed service registration requires Microsoft.Extensions.DependencyInjection.Abstractions 8.0.0 " +
                "or later at runtime. Reference version 8.0.0 or later, or target .NET 8 or later, to use the " +
                "AddUniqueKeyed / TryAddUniqueKeyed methods.");
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Gets the wildcard key used by the container to match any key at resolution time, or null
        ///     when the runtime has no keyed support.
        /// </summary>
        /// =================================================================================================
        internal static object AnyKey => AnyKeyValue;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Reads the service key of a keyed descriptor.
        /// </summary>
        /// <param name="descriptor">The descriptor to act on.</param>
        /// <returns>
        ///     The registration key, or null when the descriptor is not keyed.
        /// </returns>
        /// =================================================================================================
        internal static object SCGetServiceKey(this ServiceDescriptor descriptor)
            => ServiceKeyProperty.IsNull() ? null : ServiceKeyProperty.GetValue(descriptor);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Reads the implementation type of a keyed descriptor.
        /// </summary>
        /// <param name="descriptor">The descriptor to act on.</param>
        /// <returns>
        ///     The keyed implementation type, or null when unavailable.
        /// </returns>
        /// =================================================================================================
        internal static Type SCGetKeyedImplementationType(this ServiceDescriptor descriptor)
            => KeyedImplementationTypeProperty.IsNull()
                ? null
                : KeyedImplementationTypeProperty.GetValue(descriptor) as Type;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Reads the implementation instance of a keyed descriptor.
        /// </summary>
        /// <param name="descriptor">The descriptor to act on.</param>
        /// <returns>
        ///     The keyed implementation instance, or null when unavailable.
        /// </returns>
        /// =================================================================================================
        internal static object SCGetKeyedImplementationInstance(this ServiceDescriptor descriptor)
            => KeyedImplementationInstanceProperty.IsNull()
                ? null
                : KeyedImplementationInstanceProperty.GetValue(descriptor);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Creates a keyed service descriptor mapping a service type to an implementation type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceKey">The registration key.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="lifetime">The lifetime.</param>
        /// =================================================================================================
        internal static ServiceDescriptor SCDescribeKeyed(
            Type serviceType, object serviceKey, Type implementationType, ServiceLifetime lifetime)
            => (ServiceDescriptor)DescribeKeyedByTypeMethod.Invoke(null,
                new[] { serviceType, serviceKey, implementationType, lifetime });

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Creates a keyed service descriptor backed by a factory.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceKey">The registration key.</param>
        /// <param name="factory">The factory, receiving the provider and the resolved key.</param>
        /// <param name="lifetime">The lifetime.</param>
        /// =================================================================================================
        internal static ServiceDescriptor SCDescribeKeyed(
            Type serviceType, object serviceKey, Func<IServiceProvider, object, object> factory, ServiceLifetime lifetime)
            => (ServiceDescriptor)DescribeKeyedByFactoryMethod.Invoke(null,
                new[] { serviceType, serviceKey, factory, lifetime });

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Validates a registration key, rejecting null and the resolution-time wildcard.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the key is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the key is the wildcard key.</exception>
        /// <param name="serviceKey">The registration key.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <remarks>
        ///     A null key is rejected rather than silently degraded. The container treats a null key as a
        ///     conventional non-keyed registration, so accepting one would let a keyed call quietly
        ///     replace the non-keyed registration of the same service type. The wildcard key is a
        ///     resolution-time construct that matches every key; registering under it would shadow every
        ///     keyed registration of that service type.
        /// </remarks>
        /// =================================================================================================
        internal static void SCValidateServiceKey(object serviceKey, string paramName)
        {
            serviceKey.IfNullThrowArgumentNullException(paramName);

            if (AnyKeyValue.IsNotNull() && ReferenceEquals(serviceKey, AnyKeyValue))
            {
                throw new ArgumentException(
                    "KeyedService.AnyKey is a resolution-time wildcard and cannot be used as a registration key.",
                    paramName);
            }
        }
    }
}
