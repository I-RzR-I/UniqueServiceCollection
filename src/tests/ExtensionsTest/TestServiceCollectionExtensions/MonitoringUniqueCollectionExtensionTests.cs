// ***********************************************************************
//  Assembly         : RzR.Shared.Services.ExtensionsTest
//  Author           : RzR
//  Created On       : 2026-06-08 22:05
//
//  Last Modified By : RzR
//  Last Modified On : 2026-06-09 21:05
// ***********************************************************************
//  <copyright file="MonitoringUniqueCollectionExtensionTests.cs" company="RzR SOFT & TECH">
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
    public class MonitoringUniqueCollectionExtensionTests
    {
        [TestMethod]
        public void CheckAndCleanUpDuplicateService_NullCollection_ThrowsArgumentNullException()
        {
            IServiceCollection collection = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(
                () => collection.CheckAndCleanUpDuplicateService<IServiceInvokeOne>());
        }

        [TestMethod]
        public void CheckAndCleanUpDuplicateService_NoDuplicates_LeavesCollectionUnchanged()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvokeOne, ServiceInvokeOne>();

            collection.CheckAndCleanUpDuplicateService<IServiceInvoke>();

            Assert.AreEqual(2, collection.Count);
        }

        [TestMethod]
        public void CheckAndCleanUpDuplicateService_ExactDuplicates_CollapseToOne()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvokeOne, ServiceInvokeOne>();

            Assert.AreEqual(2, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));

            collection.CheckAndCleanUpDuplicateService<IServiceInvoke>();

            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvokeOne)));
        }

        [TestMethod]
        public void CheckAndCleanUpDuplicateService_DistinctImplementations_BothPreserved()
        {
            // Intentional multi-registration — ServiceInvoke and ServiceInvokeAlt are different impls.
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvokeAlt>();

            collection.CheckAndCleanUpDuplicateService<IServiceInvoke>();

            Assert.AreEqual(2, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)),
                "Distinct implementations must not be removed by cleanup.");
        }

        [TestMethod]
        public void CheckAndCleanUpDuplicateService_DifferentLifetimes_BothPreserved()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvoke, ServiceInvoke>();

            collection.CheckAndCleanUpDuplicateService<IServiceInvoke>();

            Assert.AreEqual(2, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)),
                "Same implementation registered with different lifetimes are not duplicates.");
        }

        [TestMethod]
        public void CheckAndCleanUpDuplicateService_ReturnsServiceCollection_ForFluentChaining()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            var result = collection.CheckAndCleanUpDuplicateService<IServiceInvoke>();

            Assert.AreSame(collection, result);
        }

        [TestMethod]
        public void CheckAndCleanUpAllDuplicates_NullCollection_ThrowsArgumentNullException()
        {
            IServiceCollection collection = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(
                () => collection.CheckAndCleanUpAllDuplicates());
        }

        [TestMethod]
        public void CheckAndCleanUpAllDuplicates_ExactDuplicatesAcrossMultipleTypes_AllCollapsed()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddSingleton<IServiceInvokeOne, ServiceInvokeOne>();

            collection.CheckAndCleanUpAllDuplicates();

            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvokeOne)));
        }

        [TestMethod]
        public void CheckAndCleanUpAllDuplicates_DistinctImplementations_BothPreservedPerType()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvokeAlt>();

            collection.CheckAndCleanUpAllDuplicates();

            Assert.AreEqual(2, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)),
                "Distinct implementations must not be removed.");
        }

        [TestMethod]
        public void CheckAndCleanUpAllDuplicates_ReturnsServiceCollection_ForFluentChaining()
        {
            var collection = new ServiceCollection();

            var result = collection.CheckAndCleanUpAllDuplicates();

            Assert.AreSame(collection, result);
        }

        [TestMethod]
        public void FindServiceDuplicate_NoDuplicates_ReturnsEmpty()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            var duplicates = collection.FindServiceDuplicate().ToList();

            Assert.AreEqual(0, duplicates.Count);
        }

        [TestMethod]
        public void FindServiceDuplicate_SameTypeTwoRegistrations_ReturnsEntry()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvokeAlt>();

            var duplicates = collection.FindServiceDuplicate().ToList();

            Assert.AreEqual(1, duplicates.Count);
            Assert.AreEqual(2, duplicates[0].Count);
        }

        [TestMethod]
        public void FindServiceDuplicateT_ExactMatch_ReturnsEntry()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            var duplicates = collection.FindServiceDuplicate<IServiceInvoke>().ToList();

            Assert.AreEqual(1, duplicates.Count);
            Assert.AreEqual(2, duplicates[0].Count);
            Assert.AreEqual(typeof(IServiceInvoke), duplicates[0].ServiceDescriptor.ServiceType);
        }

        [TestMethod]
        public void FindServiceDuplicateT_NoDuplicatesForType_ReturnsEmpty()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddSingleton<IServiceInvokeOne, ServiceInvokeOne>();

            var duplicates = collection.FindServiceDuplicate<IServiceInvoke>().ToList();

            Assert.AreEqual(0, duplicates.Count);
        }

        [TestMethod]
        public void FindExactDuplicates_IdenticalRegistrations_ReturnsReport()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvokeOne, ServiceInvokeOne>();

            var reports = collection.FindExactDuplicates().ToList();

            Assert.AreEqual(1, reports.Count);
            Assert.AreEqual(1, reports[0].DuplicateRegistrations.Count);
            Assert.AreEqual(typeof(ServiceInvoke), reports[0].RetainedDescriptor.ImplementationType);
        }

        [TestMethod]
        public void FindExactDuplicates_DistinctImplementations_NotReported()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvokeAlt>();

            var reports = collection.FindExactDuplicates().ToList();

            Assert.AreEqual(0, reports.Count,
                "Distinct implementations must not be flagged as duplicates.");
        }

        [TestMethod]
        public void FindExactDuplicates_NullCollection_ThrowsArgumentNullException()
        {
            IServiceCollection collection = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(
                () => collection.FindExactDuplicates().ToList());
        }

        [TestMethod]
        public void FindExactDuplicatesT_IdenticalRegistrations_ReturnsReport()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            var report = collection.FindExactDuplicates<IServiceInvoke>();

            Assert.IsNotNull(report);
            Assert.AreEqual(1, report.DuplicateRegistrations.Count);
            Assert.AreEqual(2, report.AllRegistrations.Count);
        }

        [TestMethod]
        public void FindExactDuplicatesT_NoDuplicates_ReturnsNull()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            var report = collection.FindExactDuplicates<IServiceInvoke>();

            Assert.IsNull(report);
        }

        [TestMethod]
        public void FindExactDuplicatesT_DistinctImplementations_ReturnsNull()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvokeAlt>();

            var report = collection.FindExactDuplicates<IServiceInvoke>();

            Assert.IsNull(report,
                "Distinct implementations must not be flagged as duplicates.");
        }

        [TestMethod]
        public void CheckAndCleanUpDuplicateService_InstanceRegistration_ExactDuplicateRemoved()
        {
            var instance = new ServiceInvoke();
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke>(instance);
            collection.AddSingleton<IServiceInvoke>(instance);

            collection.CheckAndCleanUpDuplicateService<IServiceInvoke>();

            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
        }

        [TestMethod]
        public void CheckAndCleanUpDuplicateService_InstanceRegistration_DifferentInstances_BothPreserved()
        {
            var instance1 = new ServiceInvoke();
            var instance2 = new ServiceInvoke();
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke>(instance1);
            collection.AddSingleton<IServiceInvoke>(instance2);

            collection.CheckAndCleanUpDuplicateService<IServiceInvoke>();

            Assert.AreEqual(2, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)),
                "Different instances registered for the same type are not duplicates.");
        }

        [TestMethod]
        public void CheckAndCleanUpDuplicateService_SameFactoryDelegate_CollapseToOne()
        {
            // Same delegate reference → AreExactDuplicates matches via ReferenceEquals(ImplementationFactory).
            Func<IServiceProvider, IServiceInvoke> factory = _ => new ServiceInvoke();
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke>(factory);
            collection.AddSingleton<IServiceInvoke>(factory);

            Assert.AreEqual(2, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));

            collection.CheckAndCleanUpDuplicateService<IServiceInvoke>();

            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)),
                "Two registrations sharing the same factory delegate reference must collapse to one.");
        }

        [TestMethod]
        public void CheckAndCleanUpDuplicateService_DifferentFactoryDelegates_BothPreserved()
        {
            // Two distinct lambdas are different delegate objects → not duplicates.
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke>(_ => new ServiceInvoke());
            collection.AddSingleton<IServiceInvoke>(_ => new ServiceInvoke());

            collection.CheckAndCleanUpDuplicateService<IServiceInvoke>();

            Assert.AreEqual(2, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)),
                "Two registrations with distinct factory delegates must not be removed.");
        }

        [TestMethod]
        public void FindServiceDuplicate_VsExact_ContrastTest_DistinctImplementations()
        {
            // FindServiceDuplicate (count-based legacy) reports any type registered more than once.
            // FindExactDuplicates (identity-based) only reports genuinely identical registrations.
            // With two DISTINCT implementations, only the legacy API returns a result.
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvokeAlt>();

            var legacyDuplicates = collection.FindServiceDuplicate<IServiceInvoke>().ToList();
            var exactDuplicates = collection.FindExactDuplicates().ToList();

            Assert.AreEqual(1, legacyDuplicates.Count,
                "Legacy FindServiceDuplicate must flag any service type registered more than once.");
            Assert.AreEqual(0, exactDuplicates.Count,
                "FindExactDuplicates must return empty when all registrations have distinct implementations.");
        }

        [TestMethod]
        public void ValidateNoDuplicates_NullCollection_ThrowsArgumentNullException()
        {
            IServiceCollection collection = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(
                () => collection.ValidateNoDuplicates());
        }

        [TestMethod]
        public void ValidateNoDuplicates_NoDuplicates_ReturnsCollection()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvokeOne, ServiceInvokeOne>();

            var result = collection.ValidateNoDuplicates();

            Assert.AreSame(collection, result, "Must return the same collection instance for fluent chaining.");
        }

        [TestMethod]
        public void ValidateNoDuplicates_ExactDuplicate_ThrowsInvalidOperationException()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            Assert.ThrowsException<InvalidOperationException>(
                () => collection.ValidateNoDuplicates());
        }

        [TestMethod]
        public void ValidateNoDuplicates_DistinctImplementations_DoesNotThrow()
        {
            // Two distinct implementations of the same interface is intentional multi-registration,
            // not a duplicate — ValidateNoDuplicates must not throw.
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvokeAlt>();

            var result = collection.ValidateNoDuplicates();

            Assert.AreSame(collection, result);
        }

        [TestMethod]
        public void ValidateNoDuplicates_FluentChainingWorks()
        {
            var collection = new ServiceCollection();

            var result = collection
                .AddSingleton<IServiceInvoke, ServiceInvoke>()
                .AddSingleton<IServiceInvokeOne, ServiceInvokeOne>()
                .ValidateNoDuplicates();

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void ValidateNoDuplicates_ExceptionMessage_ContainsServiceTypeName()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            try
            {
                collection.ValidateNoDuplicates();
                Assert.Fail("Expected InvalidOperationException was not thrown.");
            }
            catch (InvalidOperationException ex)
            {
                StringAssert.Contains(ex.Message, nameof(IServiceInvoke),
                    "Exception message must include the affected service type name.");
            }
        }
    }
}
