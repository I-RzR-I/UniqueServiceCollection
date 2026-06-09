// ***********************************************************************
//  Assembly         : RzR.Shared.Services.ExtensionsTest
//  Author           : RzR
//  Created On       : 2026-06-08 22:05
//
//  Last Modified By : RzR
//  Last Modified On : 2026-06-09 21:05
// ***********************************************************************
//  <copyright file="AddUniqueSingletonExtensionsTests.cs" company="RzR SOFT & TECH">
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
    public class AddUniqueSingletonExtensionsTests
    {
        [TestMethod]
        public void AddUnique_Generic_Instance_UsesProvidedInstance()
        {
            var instance = new ServiceInvoke();
            var collection = new ServiceCollection();

            collection.AddUnique<IServiceInvoke>(instance);

            var descriptor = collection.Single(x => x.ServiceType == typeof(IServiceInvoke));
            Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.AreSame(instance, descriptor.ImplementationInstance,
                "The passed instance must be registered, not a new container-constructed one.");
        }

        [TestMethod]
        public void AddUnique_Generic_Instance_ReplacesExistingRegistration()
        {
            var firstInstance = new ServiceInvoke();
            var secondInstance = new ServiceInvoke();
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke>(firstInstance);

            collection.AddUnique<IServiceInvoke>(secondInstance);

            var descriptor = collection.Single(x => x.ServiceType == typeof(IServiceInvoke));
            Assert.AreSame(secondInstance, descriptor.ImplementationInstance);
        }

        [TestMethod]
        public void AddUnique_Generic_Instance_NullCollection_ThrowsArgumentNullException()
        {
            IServiceCollection collection = null;
            var instance = new ServiceInvoke();

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUnique<IServiceInvoke>(instance));
        }

        [TestMethod]
        public void AddUnique_Generic_Instance_NullInstance_ThrowsArgumentNullException()
        {
            var collection = new ServiceCollection();

            Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUnique<IServiceInvoke>(null));
        }

        [TestMethod]
        public void AddUnique_Generic_Instance_ReturnsServiceCollection_ForFluentChaining()
        {
            var instance = new ServiceInvoke();
            var collection = new ServiceCollection();

            var result = collection.AddUnique<IServiceInvoke>(instance);

            Assert.AreSame(collection, result);
        }

        [TestMethod]
        public void AddUnique_Type_Instance_UsesProvidedInstance()
        {
            var instance = new ServiceInvoke();
            var collection = new ServiceCollection();

            collection.AddUnique(typeof(IServiceInvoke), (object)instance);

            var descriptor = collection.Single(x => x.ServiceType == typeof(IServiceInvoke));
            Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.AreSame(instance, descriptor.ImplementationInstance);
        }

        [TestMethod]
        public void AddUnique_Type_Instance_ReplacesExistingRegistration()
        {
            var first = new ServiceInvoke();
            var second = new ServiceInvoke();
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke>(first);

            collection.AddUnique(typeof(IServiceInvoke), (object)second);

            var descriptor = collection.Single(x => x.ServiceType == typeof(IServiceInvoke));
            Assert.AreSame(second, descriptor.ImplementationInstance);
        }

        [TestMethod]
        public void AddUnique_Type_Instance_NullCollection_ThrowsArgumentNullException()
        {
            IServiceCollection collection = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUnique(typeof(IServiceInvoke), new ServiceInvoke()));
        }

        [TestMethod]
        public void AddUnique_Type_Instance_NullServiceType_ThrowsArgumentNullException()
        {
            var collection = new ServiceCollection();

            Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUnique(null, new ServiceInvoke()));
        }

        [TestMethod]
        public void AddUnique_Type_Instance_NullInstance_ThrowsArgumentNullException()
        {
            var collection = new ServiceCollection();

            Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUnique(typeof(IServiceInvoke), (object)null));
        }

        [TestMethod]
        public void AddUnique_Type_Instance_ReturnsServiceCollection_ForFluentChaining()
        {
            var instance = new ServiceInvoke();
            var collection = new ServiceCollection();

            var result = collection.AddUnique(typeof(IServiceInvoke), (object)instance);

            Assert.AreSame(collection, result);
        }

        [TestMethod]
        public void AddUnique_Generic_NoArgs_RegistersAsSingleton()
        {
            var collection = new ServiceCollection();

            collection.AddUnique<ServiceInvoke>();

            var descriptor = collection.Single(x => x.ServiceType == typeof(ServiceInvoke));
            Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime);
        }

        [TestMethod]
        public void AddUnique_Generic_NoArgs_ReturnsServiceCollection_ForFluentChaining()
        {
            var collection = new ServiceCollection();

            var result = collection.AddUnique<ServiceInvoke>();

            Assert.AreSame(collection, result);
        }

        [TestMethod]
        public void AddUnique_Generic_Instance_Resolves_SameReference_FromServiceProvider()
        {
            var instance = new ServiceInvoke();
            var collection = new ServiceCollection();
            collection.AddUnique<IServiceInvoke>(instance);

            var provider = collection.BuildServiceProvider();
            var resolved = provider.GetRequiredService<IServiceInvoke>();

            Assert.AreSame(instance, resolved,
                "The container must return the exact registered instance, not a new one.");
        }

        [TestMethod]
        public void AddUnique_Type_Instance_Resolves_SameReference_FromServiceProvider()
        {
            var instance = new ServiceInvoke();
            var collection = new ServiceCollection();
            collection.AddUnique(typeof(IServiceInvoke), (object)instance);

            var provider = collection.BuildServiceProvider();
            var resolved = provider.GetRequiredService<IServiceInvoke>();

            Assert.AreSame(instance, resolved,
                "The container must return the exact registered instance, not a new one.");
        }
    }
}
