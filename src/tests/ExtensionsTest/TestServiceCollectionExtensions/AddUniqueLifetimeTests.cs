// ***********************************************************************
//  Assembly         : RzR.Shared.Services.ExtensionsTest
//  Author           : RzR
//  Created On       : 2026-06-08 22:05
//
//  Last Modified By : RzR
//  Last Modified On : 2026-06-09 21:05
// ***********************************************************************
//  <copyright file="AddUniqueLifetimeTests.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
//
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;
using System.Linq;
using ExtensionsTest.Modules.Abstractions;
using ExtensionsTest.Modules.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzR.Extensions.UniqueServiceCollection.ServiceCollectionExtensions;

#endregion

namespace ExtensionsTest.TestServiceCollectionExtensions
{
    [TestClass]
    public class AddUniqueLifetimeTests
    {
        [TestMethod]
        public void AddUnique_Generic_Lifetime_Transient_RegistersCorrectly()
        {
            var collection = new ServiceCollection();

            collection.AddUnique<ServiceInvoke>(ServiceLifetime.Transient);

            var descriptor = collection.Single(x => x.ServiceType == typeof(ServiceInvoke));
            Assert.AreEqual(ServiceLifetime.Transient, descriptor.Lifetime);
        }

        [TestMethod]
        public void AddUnique_Generic_Lifetime_Scoped_RegistersCorrectly()
        {
            var collection = new ServiceCollection();

            collection.AddUnique<ServiceInvoke>(ServiceLifetime.Scoped);

            var descriptor = collection.Single(x => x.ServiceType == typeof(ServiceInvoke));
            Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
        }

        [TestMethod]
        public void AddUnique_Generic_Lifetime_ReplacesExistingRegistration()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<ServiceInvoke>();
            collection.AddSingleton<ServiceInvoke>();

            collection.AddUnique<ServiceInvoke>(ServiceLifetime.Transient);

            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(ServiceInvoke)));
            Assert.AreEqual(ServiceLifetime.Transient,
                collection.First(x => x.ServiceType == typeof(ServiceInvoke)).Lifetime);
        }

        [TestMethod]
        public void AddUnique_Generic_Lifetime_NullCollection_ThrowsArgumentNullException()
        {
            IServiceCollection collection = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUnique<ServiceInvoke>(ServiceLifetime.Singleton));
        }

        [TestMethod]
        public void AddUnique_Generic_Lifetime_ReturnsServiceCollection_ForFluentChaining()
        {
            var collection = new ServiceCollection();

            var result = collection.AddUnique<ServiceInvoke>(ServiceLifetime.Singleton);

            Assert.AreSame(collection, result);
        }

        [TestMethod]
        public void RegisterIfNotExist_Factory_RegistersWhenNotPresent()
        {
            var collection = new ServiceCollection();

            collection.RegisterIfNotExist<IServiceInvoke>(
                _ => new ServiceInvoke(), ServiceLifetime.Singleton);

            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
            var descriptor = collection.Single(x => x.ServiceType == typeof(IServiceInvoke));
            Assert.IsNotNull(descriptor.ImplementationFactory);
        }

        [TestMethod]
        public void RegisterIfNotExist_Factory_DoesNotRegisterWhenAlreadyPresent()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            collection.RegisterIfNotExist<IServiceInvoke>(
                _ => new ServiceInvokeAlt(), ServiceLifetime.Singleton);

            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
            var descriptor = collection.Single(x => x.ServiceType == typeof(IServiceInvoke));
            Assert.AreEqual(typeof(ServiceInvoke), descriptor.ImplementationType,
                "Existing registration must not be replaced.");
        }

        [TestMethod]
        public void RegisterIfNotExist_Factory_NullCollection_ThrowsArgumentNullException()
        {
            IServiceCollection collection = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(
                () => collection.RegisterIfNotExist<IServiceInvoke>(_ => new ServiceInvoke()));
        }

        [TestMethod]
        public void RegisterIfNotExist_Factory_NullFactory_ThrowsArgumentNullException()
        {
            var collection = new ServiceCollection();

            Assert.ThrowsException<ArgumentNullException>(
                () => collection.RegisterIfNotExist<IServiceInvoke>(null));
        }

        [TestMethod]
        public void RegisterIfNotExist_Factory_ReturnsServiceCollection_ForFluentChaining()
        {
            var collection = new ServiceCollection();

            var result = collection.RegisterIfNotExist<IServiceInvoke>(_ => new ServiceInvoke());

            Assert.AreSame(collection, result);
        }

        [TestMethod]
        public void AddUnique_FluentChaining_MultipleCallsWork()
        {
            var collection = new ServiceCollection();

            collection
                .AddUnique<IServiceInvoke, ServiceInvoke>()
                .AddUnique<IServiceInvokeOne, ServiceInvokeOne>()
                .AddUnique<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvokeOne)));
            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvokeTwo)));
        }
    }
}
