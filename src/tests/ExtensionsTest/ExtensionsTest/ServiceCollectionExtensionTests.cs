// ***********************************************************************
//  Assembly         : RzR.Shared.Services.ExtensionsTest
//  Author           : RzR
//  Created On       : 2025-06-17 18:16
// 
//  Last Modified By : RzR
//  Last Modified On : 2025-06-17 21:59
// ***********************************************************************
//  <copyright file="ServiceCollectionExtensionTests.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;
using ExtensionsTest.Modules.Abstractions;
using ExtensionsTest.Modules.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniqueServiceCollection.Extensions;

#endregion

namespace ExtensionsTest.ExtensionsTest
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void HasAny_Test()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            var hasAny = collection.SCHasAny(typeof(IServiceInvokeOne));

            Assert.IsNotNull(hasAny);
            Assert.IsTrue(hasAny);
        }

        [TestMethod]
        public void HasAny_ArgumentNullException_Collection_Test()
        {
            ServiceCollection collection = null;

            Assert.IsNull(collection);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(() => collection.SCHasAny(typeof(IServiceInvokeOne)));
        }

        [TestMethod]
        public void HasAny_ArgumentNullException_CollectionType_Test()
        {
            var collection = new ServiceCollection();
            Type collectionType = null;

            Assert.IsNotNull(collection);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(() => collection.SCHasAny(collectionType));
        }

        [TestMethod]
        public void HasAny_T_Test()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            var hasAny = collection.SCHasAny<IServiceInvokeOne>();

            Assert.IsNotNull(hasAny);
            Assert.IsTrue(hasAny);
        }

        [TestMethod]
        public void IfHasAny_Test()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddTransient<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.AreEqual(4, collection.Count);

            collection.SCIfHasAny(typeof(IServiceInvokeOne),
                () => collection.RemoveAll(typeof(IServiceInvokeOne)));

            Assert.IsNotNull(collection);
            Assert.AreEqual(2, collection.Count);
        }

        [TestMethod]
        public void IfHasAny_T_Test()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddTransient<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.AreEqual(4, collection.Count);

            collection.SCIfHasAny<IServiceInvokeOne>(() => collection.RemoveAll<IServiceInvokeOne>());

            Assert.IsNotNull(collection);
            Assert.AreEqual(2, collection.Count);
        }

        [TestMethod]
        public void RemoveAllIfHasAny_Test()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddTransient<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.AreEqual(4, collection.Count);

            collection.SCRemoveAllIfHasAny(typeof(IServiceInvokeOne));

            Assert.IsNotNull(collection);
            Assert.AreEqual(2, collection.Count);
        }

        [TestMethod]
        public void RemoveAllIfHasAny_T_Test()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddTransient<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.AreEqual(4, collection.Count);

            collection.SCRemoveAllIfHasAny<IServiceInvokeOne>();

            Assert.IsNotNull(collection);
            Assert.AreEqual(2, collection.Count);
        }
    }
}