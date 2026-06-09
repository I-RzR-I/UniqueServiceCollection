// ***********************************************************************
//  Assembly         : RzR.Shared.Services.ExtensionsTest
//  Author           : RzR
//  Created On       : 2026-06-08 22:05
//
//  Last Modified By : RzR
//  Last Modified On : 2026-06-09 21:05
// ***********************************************************************
//  <copyright file="ReplaceUniqueExtensionsTests.cs" company="RzR SOFT & TECH">
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
    public class ReplaceUniqueExtensionsTests
    {
        [TestMethod]
        public void ReplaceUnique_FirstRegistration_ReturnsFalse()
        {
            var collection = new ServiceCollection();

            var replaced = collection.ReplaceUnique<IServiceInvoke, ServiceInvoke>();

            Assert.IsFalse(replaced, "No prior registration — should return false.");
            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
        }

        [TestMethod]
        public void ReplaceUnique_ExistingRegistration_ReturnsTrue()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            var replaced = collection.ReplaceUnique<IServiceInvoke, ServiceInvokeAlt>();

            Assert.IsTrue(replaced, "Prior registration existed — should return true.");
            var descriptor = collection.Single(x => x.ServiceType == typeof(IServiceInvoke));
            Assert.AreEqual(typeof(ServiceInvokeAlt), descriptor.ImplementationType);
        }

        [TestMethod]
        public void ReplaceUnique_ExistingMultipleRegistrations_AllReplacedReturnTrue()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvoke, ServiceInvokeAlt>();

            var replaced = collection.ReplaceUnique<IServiceInvoke, ServiceInvoke>(ServiceLifetime.Transient);

            Assert.IsTrue(replaced);
            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
        }

        [TestMethod]
        public void ReplaceUnique_RespectsLifetimeParameter()
        {
            var collection = new ServiceCollection();

            collection.ReplaceUnique<IServiceInvoke, ServiceInvoke>(ServiceLifetime.Scoped);

            var descriptor = collection.Single(x => x.ServiceType == typeof(IServiceInvoke));
            Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
        }

        [TestMethod]
        public void ReplaceUnique_NullCollection_ThrowsArgumentNullException()
        {
            IServiceCollection collection = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(
                () => collection.ReplaceUnique<IServiceInvoke, ServiceInvoke>());
        }
    }
}
