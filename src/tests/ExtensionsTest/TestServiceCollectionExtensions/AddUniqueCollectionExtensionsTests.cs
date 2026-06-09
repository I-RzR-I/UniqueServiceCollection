// ***********************************************************************
//  Assembly         : RzR.Shared.Services.ExtensionsTest
//  Author           : RzR
//  Created On       : 2025-06-18 14:39
// 
//  Last Modified By : RzR
//  Last Modified On : 2025-06-18 14:42
// ***********************************************************************
//  <copyright file="AddUniqueCollectionExtensionsTests.cs" company="RzR SOFT & TECH">
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
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzR.Extensions.UniqueServiceCollection.Extensions;
using RzR.Extensions.UniqueServiceCollection.ServiceCollectionExtensions;

#endregion

namespace ExtensionsTest.TestServiceCollectionExtensions
{
    [TestClass]
    public class AddUniqueCollectionExtensionsTests
    {
        private static ServiceCollection InitServiceCollection()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<ILoggerFactory, LoggerFactory>();
            serviceCollection.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            serviceCollection.AddLogging(loggingBuilder => loggingBuilder
                .AddConsole()
                .AddDebug()
                .SetMinimumLevel(LogLevel.Debug));

            return serviceCollection;
        }

        private static IServiceCollection InitIServiceCollection()
            => InitServiceCollection();

        [TestMethod]
        public void IServiceCollection_AddUnique_Scoped_Test_Should_Pass()
        {
            var collection = InitIServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddTransient<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.IsTrue(collection.Count >= 4);

            var countServiceOne = collection.SCCountByType<IServiceInvokeOne>();
            Assert.AreEqual(2, countServiceOne);

            collection.AddUnique<IServiceInvokeOne, ServiceInvokeOne>(ServiceLifetime.Scoped);

            var countServiceOneUnique = collection.SCCountByType<IServiceInvokeOne>();
            Assert.AreEqual(1, countServiceOneUnique);

            var serviceOne = collection.FirstOrDefault(x => x.ServiceType == typeof(IServiceInvokeOne));
            Assert.IsNotNull(serviceOne);
            Assert.AreEqual(ServiceLifetime.Scoped, serviceOne.Lifetime);
        }

        [TestMethod]
        public void IServiceCollection_AddUnique_Singleton_Test_Should_Pass()
        {
            var collection = InitIServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddTransient<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.IsTrue(collection.Count >= 4);

            var countServiceOne = collection.SCCountByType<IServiceInvokeOne>();
            Assert.AreEqual(2, countServiceOne);

            collection.AddUnique<IServiceInvokeOne, ServiceInvokeOne>();

            var countServiceOneUnique = collection.SCCountByType<IServiceInvokeOne>();
            Assert.AreEqual(1, countServiceOneUnique);

            var serviceOne = collection.FirstOrDefault(x => x.ServiceType == typeof(IServiceInvokeOne));
            Assert.IsNotNull(serviceOne);
            Assert.AreEqual(ServiceLifetime.Singleton, serviceOne.Lifetime);
        }

        [TestMethod]
        public void ServiceCollection_AddUnique_Scoped_Test_Should_Pass()
        {
            var collection = InitServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddTransient<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.IsTrue(collection.Count >= 4);

            var countServiceOne = collection.SCCountByType<IServiceInvokeOne>();
            Assert.AreEqual(2, countServiceOne);

            collection.AddUnique<IServiceInvokeOne, ServiceInvokeOne>(ServiceLifetime.Scoped);

            var countServiceOneUnique = collection.SCCountByType<IServiceInvokeOne>();
            Assert.AreEqual(1, countServiceOneUnique);

            var serviceOne = collection.FirstOrDefault(x => x.ServiceType == typeof(IServiceInvokeOne));
            Assert.IsNotNull(serviceOne);
            Assert.AreEqual(ServiceLifetime.Scoped, serviceOne.Lifetime);
        }

        [TestMethod]
        public void ServiceCollection_AddUnique_Singleton_Test_Should_Pass()
        {
            var collection = InitServiceCollection();

            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddTransient<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.IsTrue(collection.Count >= 4);

            var countServiceOne = collection.SCCountByType<IServiceInvokeOne>();
            Assert.AreEqual(2, countServiceOne);

            collection.AddUnique<IServiceInvokeOne, ServiceInvokeOne>();

            var countServiceOneUnique = collection.SCCountByType<IServiceInvokeOne>();
            Assert.AreEqual(1, countServiceOneUnique);

            var serviceOne = collection.FirstOrDefault(x => x.ServiceType == typeof(IServiceInvokeOne));
            Assert.IsNotNull(serviceOne);
            Assert.AreEqual(ServiceLifetime.Singleton, serviceOne.Lifetime);
        }

        [TestMethod]
        public void AddUnique_Factory_RegistersWithCorrectLifetime_Singleton()
        {
            var collection = new ServiceCollection();

            collection.AddUnique<IServiceInvoke>(_ => new ServiceInvoke(), ServiceLifetime.Singleton);

            var descriptor = collection.Single(x => x.ServiceType == typeof(IServiceInvoke));
            Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.IsNotNull(descriptor.ImplementationFactory);
        }

        [TestMethod]
        public void AddUnique_Factory_RegistersWithCorrectLifetime_Scoped()
        {
            var collection = new ServiceCollection();

            collection.AddUnique<IServiceInvoke>(_ => new ServiceInvoke(), ServiceLifetime.Scoped);

            var descriptor = collection.Single(x => x.ServiceType == typeof(IServiceInvoke));
            Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
        }

        [TestMethod]
        public void AddUnique_Factory_RegistersWithCorrectLifetime_Transient()
        {
            var collection = new ServiceCollection();

            collection.AddUnique<IServiceInvoke>(_ => new ServiceInvoke(), ServiceLifetime.Transient);

            var descriptor = collection.Single(x => x.ServiceType == typeof(IServiceInvoke));
            Assert.AreEqual(ServiceLifetime.Transient, descriptor.Lifetime);
        }

        [TestMethod]
        public void AddUnique_Factory_ReplacesExistingRegistration()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvokeAlt>();

            collection.AddUnique<IServiceInvoke>(_ => new ServiceInvoke());

            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
            var descriptor = collection.Single(x => x.ServiceType == typeof(IServiceInvoke));
            Assert.IsNotNull(descriptor.ImplementationFactory,
                "The factory overload must register via factory, not type.");
        }

        [TestMethod]
        public void AddUnique_Factory_NullFactory_ThrowsArgumentNullException()
        {
            var collection = new ServiceCollection();

            Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUnique<IServiceInvoke>((Func<IServiceProvider, IServiceInvoke>)null));
        }

        [TestMethod]
        public void AddUnique_Factory_NullCollection_ThrowsArgumentNullException()
        {
            IServiceCollection collection = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUnique<IServiceInvoke>(_ => new ServiceInvoke()));
        }

        [TestMethod]
        public void AddUnique_Factory_ReturnsServiceCollection_ForFluentChaining()
        {
            var collection = new ServiceCollection();

            var result = collection.AddUnique<IServiceInvoke>(_ => new ServiceInvoke());

            Assert.AreSame(collection, result);
        }
    }
}