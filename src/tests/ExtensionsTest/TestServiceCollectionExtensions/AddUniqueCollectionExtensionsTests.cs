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

using System.Linq;
using ExtensionsTest.Modules.Abstractions;
using ExtensionsTest.Modules.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniqueServiceCollection.Extensions;
using UniqueServiceCollection.ServiceCollectionExtensions;

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
    }
}