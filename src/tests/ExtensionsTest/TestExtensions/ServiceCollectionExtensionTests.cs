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

namespace ExtensionsTest.TestExtensions
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void SCHasAny_Null_Test_Should_Pass()
        {
            ServiceCollection collection = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            var hasAny = collection.SCHasAny(typeof(IServiceInvokeOne));

            Assert.IsNotNull(hasAny);
            Assert.IsFalse(hasAny);
        }

        [TestMethod]
        public void SCHasAny_Empty_Test_Should_Pass()
        {
            var collection = new ServiceCollection();

            var hasAny = collection.SCHasAny(typeof(IServiceInvokeOne));

            Assert.IsNotNull(hasAny);
            Assert.IsFalse(hasAny);
        }

        [TestMethod]
        public void SCHasAny_Test_Should_Pass()
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
        public void SCHasNoAny_Empty_Test_Should_Pass()
        {
            var collection = new ServiceCollection();

            var hasAny = collection.SCHasNoAny(typeof(IServiceInvokeOne));

            Assert.IsNotNull(hasAny);
            Assert.IsTrue(hasAny);
        }

        [TestMethod]
        public void SCHasNoAny_Null_Test_Should_Pass()
        {
            ServiceCollection collection = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            var hasAny = collection.SCHasNoAny(typeof(IServiceInvokeOne));

            Assert.IsNotNull(hasAny);
            Assert.IsFalse(hasAny);
        }

        [TestMethod]
        public void SCHasAny_ArgumentNullException_CollectionType_Test_Should_Pass()
        {
            var collection = new ServiceCollection();
            Type collectionType = null;

            Assert.IsNotNull(collection);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(() => collection.SCHasAny(collectionType));
        }

        [TestMethod]
        public void SCHasAny_T_Test_Should_Pass()
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
        public void SCIfHasAny_Test_Should_Pass()
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
        public void SCIfHasAny_Null_Collection_Test_Should_Pass()
        {
            ServiceCollection collection = null;

            Assert.IsNull(collection);
            Assert.ThrowsException<ArgumentNullException>(() =>
                // ReSharper disable once ExpressionIsAlwaysNull
                collection.SCIfHasAny(typeof(IServiceInvokeOne),
                    // ReSharper disable once ExpressionIsAlwaysNull
                    // ReSharper disable once AssignNullToNotNullAttribute
                    () => collection.RemoveAll(typeof(IServiceInvokeOne))));
        }

        [TestMethod]
        public void SCIfHasAny_Null_CollectionType_Test_Should_Pass()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddTransient<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.ThrowsException<ArgumentNullException>(() =>
                // ReSharper disable once ExpressionIsAlwaysNull
                collection.SCIfHasAny(null,
                    () => collection.RemoveAll(typeof(IServiceInvokeOne))));
        }

        [TestMethod]
        public void SCIfHasAny_Null_Action_Test_Should_Pass()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddTransient<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.ThrowsException<ArgumentNullException>(() =>
                collection.SCIfHasAny(typeof(IServiceInvokeOne),
                    // ReSharper disable once AssignNullToNotNullAttribute
                    () => collection.RemoveAll(null)));
        }

        [TestMethod]
        public void SCIfHasAny_T_Test_Should_Pass()
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
        public void SCRemoveAllIfHasAny_Test_Should_Pass()
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
        public void SCRemoveAllIfHasAny_T_Test_Should_Pass()
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

        [TestMethod]
        public void SCRemoveAllIfHasAny_T_ThrowArgumentNullException_Test_Should_Pass()
        {
            ServiceCollection collection = null;

            Assert.IsNull(collection);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(() => collection.SCRemoveAllIfHasAny<IServiceInvokeOne>());
        }

        [TestMethod]
        public void SCHasNoAny_T_Test_Should_Pass()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.AreEqual(2, collection.Count);

            var check = collection.SCHasNoAny<IServiceInvoke>();

            Assert.IsNotNull(collection);
            Assert.IsTrue(check);
        }

        [TestMethod]
        public void SCCountByType_Test_Should_Pass()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.AreEqual(3, collection.Count);

            var count = collection.SCCountByType(typeof(IServiceInvokeTwo));

            Assert.IsNotNull(collection);
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void SCAddIfHasNoAny_Type_Test_Should_Pass()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.AreEqual(2, collection.Count);

            collection.SCAddIfHasNoAny(typeof(IServiceInvoke), typeof(ServiceInvoke),
                ServiceLifetime.Scoped);

            Assert.IsNotNull(collection);
            Assert.AreEqual(3, collection.Count);
        }

        [TestMethod]
        public void SCAddIfHasNoAny_Type_Exist_Test_Should_Pass()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.AreEqual(3, collection.Count);

            collection.SCAddIfHasNoAny(typeof(IServiceInvoke), typeof(ServiceInvoke),
                ServiceLifetime.Scoped);

            Assert.IsNotNull(collection);
            Assert.AreEqual(3, collection.Count);
        }

        [TestMethod]
        public void SCAddIfHasNoAny_Type_2_Test_Should_Pass()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.AreEqual(2, collection.Count);

            collection.SCAddIfHasNoAny(typeof(ServiceInvoke),
                ServiceLifetime.Scoped);

            Assert.IsNotNull(collection);
            Assert.AreEqual(3, collection.Count);
        }

        [TestMethod]
        public void SCAddIfHasNoAny_Type_3_Test_Should_Pass()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.AreEqual(3, collection.Count);

            collection.SCAddIfHasNoAny(typeof(ServiceInvoke),
                ServiceLifetime.Scoped);

            Assert.IsNotNull(collection);
            Assert.AreEqual(4, collection.Count);
        }

        [TestMethod]
        public void SCAddToServiceCollection_Type_Test_Should_Pass()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.AreEqual(3, collection.Count);

            collection.SCAddToServiceCollection(typeof(ServiceInvoke), ServiceLifetime.Scoped);
            collection.SCAddToServiceCollection(typeof(ServiceInvokeOne), ServiceLifetime.Transient);

            Assert.IsNotNull(collection);
            Assert.AreEqual(5, collection.Count);
        }

        [TestMethod]
        public void SCAddToServiceCollection_Type_x2_Test_Should_Pass()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.AreEqual(3, collection.Count);

            collection.SCAddToServiceCollection(typeof(IServiceInvoke), typeof(ServiceInvoke), ServiceLifetime.Scoped);
            collection.SCAddToServiceCollection(typeof(IServiceInvokeOne),typeof(ServiceInvokeOne), ServiceLifetime.Transient);

            Assert.IsNotNull(collection);
            Assert.AreEqual(5, collection.Count);
        }
    }
}