// ***********************************************************************
//  Assembly         : RzR.Shared.Services.ExtensionsTest
//  Author           : RzR
//  Created On       : 2025-06-18 16:12
// 
//  Last Modified By : RzR
//  Last Modified On : 2025-06-18 16:12
// ***********************************************************************
//  <copyright file="RegisterIfNotExistCollectionExtensionsTests.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

using ExtensionsTest.Modules.Abstractions;
using ExtensionsTest.Modules.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using RzR.Extensions.UniqueServiceCollection.Extensions;
using RzR.Extensions.UniqueServiceCollection.ServiceCollectionExtensions;

namespace ExtensionsTest.TestServiceCollectionExtensions
{
    [TestClass]
    public class RegisterIfNotExistCollectionExtensionsTests
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
        public void IServiceCollection_RegisterIfNotExist_ArgumentNullException_Test_ShouldPass()
        {
            IServiceCollection collection = null;

            Assert.IsNull(collection);
            Assert.ThrowsException<ArgumentNullException>(
                // ReSharper disable once ExpressionIsAlwaysNull
                () => collection.RegisterIfNotExist<IServiceInvoke, ServiceInvoke>());
        }

        [TestMethod]
        public void IServiceCollection_RegisterIfNotExist_Test_ShouldPass()
        {
            var collection = InitIServiceCollection();
            
            collection.AddUnique<IServiceInvoke, ServiceInvoke>(ServiceLifetime.Scoped);
            collection.AddUnique<IServiceInvokeOne, ServiceInvokeOne>(ServiceLifetime.Scoped);
            collection.AddUnique<IServiceInvokeTwo, ServiceInvokeTwo>(ServiceLifetime.Scoped);

            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count >= 3);

            var countServiceByType = collection.SCCountByType<IServiceInvoke>();
            Assert.AreEqual(1, countServiceByType);

            collection.RegisterIfNotExist<IServiceInvoke, ServiceInvoke>();
            
            var serviceInvokeSingleton = collection.FirstOrDefault(
                x => x.ServiceType == typeof(IServiceInvoke)
                     && x.Lifetime == ServiceLifetime.Singleton);

            Assert.IsNull(serviceInvokeSingleton);
            var serviceInvoke = collection.FirstOrDefault(x => x.ServiceType == typeof(IServiceInvoke));

            Assert.IsNotNull(serviceInvoke);
            Assert.AreEqual(ServiceLifetime.Scoped, serviceInvoke.Lifetime);
        }

        [TestMethod]
        public void IServiceCollection_RegisterIfNotExist_MultiSameType_Test_ShouldPass()
        {
            var collection = InitIServiceCollection();
            
            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddTransient<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count >= 3);

            var countServiceByType = collection.SCCountByType<IServiceInvoke>();
            Assert.AreEqual(2, countServiceByType);

            collection.RegisterIfNotExist<IServiceInvoke, ServiceInvoke>();
            
            var serviceInvokeSingleton = collection.FirstOrDefault(
                x => x.ServiceType == typeof(IServiceInvoke)
                     && x.Lifetime == ServiceLifetime.Singleton);

            Assert.IsNull(serviceInvokeSingleton);
            var serviceInvoke = collection.Where(x => x.ServiceType == typeof(IServiceInvoke));

            Assert.IsNotNull(serviceInvoke);
            Assert.AreEqual(2, serviceInvoke.Count());
        }

        [TestMethod]
        public void IServiceCollection_RegisterIfNotExist_ServiceType_Test_ShouldPass()
        {
            var collection = InitIServiceCollection();
            
            collection.AddTransient<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.IsNotNull(collection);

            var countServiceByType = collection.SCCountByType<IServiceInvoke>();
            Assert.AreEqual(1, countServiceByType);

            var countServiceByType2 = collection.SCCountByType<ServiceInvoke>();
            Assert.AreEqual(0, countServiceByType2);

            collection.RegisterIfNotExist<ServiceInvoke>();
            
            var serviceInvokeSingleton = collection.FirstOrDefault(
                x => x.ServiceType == typeof(ServiceInvoke)
                     && x.Lifetime == ServiceLifetime.Singleton);

            Assert.IsNotNull(serviceInvokeSingleton);
            var serviceInvoke = collection.Where(x => x.ServiceType == typeof(IServiceInvoke));

            Assert.IsNotNull(serviceInvoke);
            Assert.AreEqual(1, serviceInvoke.Count());

            var serviceInvoke2 = collection.Where(x => x.ServiceType == typeof(ServiceInvoke));

            Assert.IsNotNull(serviceInvoke2);
            Assert.AreEqual(1, serviceInvoke2.Count());
        }

        [TestMethod]
        public void IServiceCollection_RegisterIfNotExist_ServiceType_Exist_Test_ShouldPass()
        {
            var collection = InitIServiceCollection();
            
            collection.AddScoped<ServiceInvoke>();
            collection.AddTransient<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddScoped<IServiceInvokeTwo, ServiceInvokeTwo>();

            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count >= 4);

            var countServiceByType = collection.SCCountByType<IServiceInvoke>();
            Assert.AreEqual(1, countServiceByType);

            var countServiceByType2 = collection.SCCountByType<ServiceInvoke>();
            Assert.AreEqual(1, countServiceByType2);

            collection.RegisterIfNotExist<ServiceInvoke>();
            
            var serviceInvokeSingleton = collection.FirstOrDefault(
                x => x.ServiceType == typeof(IServiceInvoke)
                     && x.Lifetime == ServiceLifetime.Singleton);

            Assert.IsNull(serviceInvokeSingleton);
            var serviceInvoke = collection.Where(x => x.ServiceType == typeof(IServiceInvoke));

            Assert.IsNotNull(serviceInvoke);
            Assert.AreEqual(1, serviceInvoke.Count());

            var serviceInvoke2 = collection.Where(x => x.ServiceType == typeof(ServiceInvoke));

            Assert.IsNotNull(serviceInvoke2);
            Assert.AreEqual(1, serviceInvoke2.Count());
        }

        [TestMethod]
        public void ServiceCollection_RegisterIfNotExist_ArgumentNullException_Test_ShouldPass()
        {
            ServiceCollection collection = null;

            Assert.IsNull(collection);
            Assert.ThrowsException<ArgumentNullException>(
                // ReSharper disable once ExpressionIsAlwaysNull
                () => collection.RegisterIfNotExist<IServiceInvoke, ServiceInvoke>());
        }

        [TestMethod]
        public void ServiceCollection_RegisterIfNotExist_Test_ShouldPass()
        {
            var collection = InitServiceCollection();
            
            collection.AddUnique<IServiceInvoke, ServiceInvoke>(ServiceLifetime.Scoped);
            collection.AddUnique<IServiceInvokeOne, ServiceInvokeOne>(ServiceLifetime.Scoped);
            collection.AddUnique<IServiceInvokeTwo, ServiceInvokeTwo>(ServiceLifetime.Scoped);

            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count >= 3);

            var countServiceByType = collection.SCCountByType<IServiceInvoke>();
            Assert.AreEqual(1, countServiceByType);

            collection.RegisterIfNotExist<IServiceInvoke, ServiceInvoke>();
            
            var serviceInvokeSingleton = collection.FirstOrDefault(x => x.ServiceType == typeof(IServiceInvoke)
            && x.Lifetime == ServiceLifetime.Singleton);

            Assert.IsNull(serviceInvokeSingleton);
            var serviceInvoke = collection.FirstOrDefault(x => x.ServiceType == typeof(IServiceInvoke));

            Assert.IsNotNull(serviceInvoke);
            Assert.AreEqual(ServiceLifetime.Scoped, serviceInvoke.Lifetime);
        }
    }
}