#if NET8_0_OR_GREATER

#region U S A G E S

using System;
using System.Linq;
using ExtensionsTest.Modules.Abstractions;
using ExtensionsTest.Modules.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzR.Extensions.UniqueServiceCollection.Helpers;
using RzR.Extensions.UniqueServiceCollection.ServiceCollectionExtensions;

#endregion

namespace ExtensionsTest.TestServiceCollectionExtensions
{
    [TestClass]
    public class KeyedMonitoringGuardTests
    {
        private static ServiceCollection KeyedScopedPlusDuplicateNonKeyedSingleton()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedScoped<IServiceInvoke, ServiceInvoke>("k");
            collection.AddSingleton<IServiceInvoke, ServiceInvokeAlt>();
            collection.AddSingleton<IServiceInvoke, ServiceInvokeAlt>();

            return collection;
        }

        [TestMethod]
        public void FindExactDuplicatesT_KeyedDescriptorPresent_ReportsSingleNonKeyedDuplicate()
        {
            var collection = KeyedScopedPlusDuplicateNonKeyedSingleton();

            var report = collection.FindExactDuplicates<IServiceInvoke>();

            Assert.IsNotNull(report);
            Assert.AreEqual(1, report.DuplicateRegistrations.Count);
            Assert.IsFalse(report.DuplicateRegistrations[0].IsKeyedService);
            Assert.AreEqual(typeof(ServiceInvokeAlt), report.DuplicateRegistrations[0].ImplementationType);
        }

        [TestMethod]
        public void FindExactDuplicatesT_KeyedDescriptorPresent_RetainedDescriptorIsTheNonKeyedSingleton()
        {
            var collection = KeyedScopedPlusDuplicateNonKeyedSingleton();

            var report = collection.FindExactDuplicates<IServiceInvoke>();

            Assert.IsNotNull(report);
            Assert.IsFalse(report.RetainedDescriptor.IsKeyedService);
            Assert.AreEqual(typeof(ServiceInvokeAlt), report.RetainedDescriptor.ImplementationType);
            Assert.AreEqual(ServiceLifetime.Singleton, report.RetainedDescriptor.Lifetime);
        }

        [TestMethod]
        public void FindExactDuplicatesT_KeyedDescriptorPresent_AllRegistrationsExcludesTheKeyedDescriptor()
        {
            var collection = KeyedScopedPlusDuplicateNonKeyedSingleton();

            var report = collection.FindExactDuplicates<IServiceInvoke>();

            Assert.IsNotNull(report);
            Assert.AreEqual(2, report.AllRegistrations.Count);
            Assert.AreEqual(0, report.AllRegistrations.Count(x => x.IsKeyedService));
        }

        [TestMethod]
        public void FindExactDuplicates_KeyedDescriptorPresent_ReportsOnlyTheNonKeyedSingletonPair()
        {
            var collection = KeyedScopedPlusDuplicateNonKeyedSingleton();

            var reports = collection.FindExactDuplicates().ToList();

            Assert.AreEqual(1, reports.Count);
            Assert.AreEqual(1, reports[0].DuplicateRegistrations.Count);
            Assert.IsFalse(reports[0].RetainedDescriptor.IsKeyedService);
            Assert.AreEqual(ServiceLifetime.Singleton, reports[0].RetainedDescriptor.Lifetime);
        }

        [TestMethod]
        public void ValidateNoDuplicates_KeyedScopedPlusDuplicateNonKeyedSingleton_Throws()
        {
            var collection = KeyedScopedPlusDuplicateNonKeyedSingleton();

            Assert.ThrowsException<InvalidOperationException>(() => collection.ValidateNoDuplicates());
        }

        [TestMethod]
        public void ValidateNoDuplicates_KeyedScopedPlusDuplicateNonKeyedSingleton_MessageNamesSingletonNotScoped()
        {
            var collection = KeyedScopedPlusDuplicateNonKeyedSingleton();

            var ex = Assert.ThrowsException<InvalidOperationException>(() => collection.ValidateNoDuplicates());

            StringAssert.Contains(ex.Message, nameof(IServiceInvoke));
            StringAssert.Contains(ex.Message, ServiceLifetime.Singleton.ToString());
            Assert.IsFalse(ex.Message.Contains(ServiceLifetime.Scoped.ToString()));
        }

        [TestMethod]
        public void CheckAndCleanUpDuplicateServiceT_KeyedDescriptorPresent_CollapsesOnlyTheNonKeyedDuplicate()
        {
            var collection = KeyedScopedPlusDuplicateNonKeyedSingleton();

            collection.CheckAndCleanUpDuplicateService<IServiceInvoke>();

            Assert.AreEqual(1, collection.Count(x => !x.IsKeyedService));
            Assert.AreEqual(typeof(ServiceInvokeAlt),
                collection.Single(x => !x.IsKeyedService).ImplementationType);
        }

        [TestMethod]
        public void CheckAndCleanUpDuplicateServiceT_KeyedDescriptorPresent_LeavesKeyedDescriptorIntact()
        {
            var collection = KeyedScopedPlusDuplicateNonKeyedSingleton();
            var keyedBefore = collection.Single(x => x.IsKeyedService);

            collection.CheckAndCleanUpDuplicateService<IServiceInvoke>();

            var keyedAfter = collection.Single(x => x.IsKeyedService);

            Assert.AreSame(keyedBefore, keyedAfter);
            Assert.AreEqual("k", keyedAfter.ServiceKey);
            Assert.AreEqual(typeof(ServiceInvoke), keyedAfter.KeyedImplementationType);
            Assert.AreEqual(ServiceLifetime.Scoped, keyedAfter.Lifetime);
        }

        [TestMethod]
        public void CheckAndCleanUpAllDuplicates_KeyedDescriptorPresent_CollapsesOnlyTheNonKeyedDuplicate()
        {
            var collection = KeyedScopedPlusDuplicateNonKeyedSingleton();

            collection.CheckAndCleanUpAllDuplicates();

            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(1, collection.Count(x => !x.IsKeyedService));
            Assert.AreEqual(1, collection.Count(x => x.IsKeyedService));
        }

        [TestMethod]
        public void CheckAndCleanUpAllDuplicates_KeyedDescriptorPresent_LeavesKeyedDescriptorIntact()
        {
            var collection = KeyedScopedPlusDuplicateNonKeyedSingleton();
            var keyedBefore = collection.Single(x => x.IsKeyedService);

            collection.CheckAndCleanUpAllDuplicates();

            var keyedAfter = collection.Single(x => x.IsKeyedService);

            Assert.AreSame(keyedBefore, keyedAfter);
            Assert.AreEqual("k", keyedAfter.ServiceKey);
            Assert.AreEqual(ServiceLifetime.Scoped, keyedAfter.Lifetime);
        }

        [TestMethod]
        public void FindServiceDuplicate_NonKeyedPlusKeyedOfSameServiceType_ReturnsEmpty()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvokeAlt>("k");

            var duplicates = collection.FindServiceDuplicate().ToList();

            Assert.AreEqual(0, duplicates.Count);
        }

        [TestMethod]
        public void FindServiceDuplicateT_NonKeyedPlusKeyedOfSameServiceType_ReturnsEmpty()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvokeAlt>("k");

            var duplicates = collection.FindServiceDuplicate<IServiceInvoke>().ToList();

            Assert.AreEqual(0, duplicates.Count);
        }

        [TestMethod]
        public void FindServiceDuplicate_GenuineNonKeyedPairAlongsideKeyed_StillReportsThePair()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvokeAlt>();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvokeAlt>("k");

            var duplicates = collection.FindServiceDuplicate().ToList();

            Assert.AreEqual(1, duplicates.Count);
            Assert.AreEqual(2, duplicates[0].Count);
            Assert.AreEqual(typeof(IServiceInvoke), duplicates[0].ServiceDescriptor.ServiceType);
        }

        [TestMethod]
        public void FindServiceDuplicateT_GenuineNonKeyedPairAlongsideKeyed_StillReportsThePair()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvokeAlt>();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvokeAlt>("k");

            var duplicates = collection.FindServiceDuplicate<IServiceInvoke>().ToList();

            Assert.AreEqual(1, duplicates.Count);
            Assert.AreEqual(2, duplicates[0].Count);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_TypeMapped_AnyKeyRegistrationPresent_ReturnsFalse()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>(KeyedService.AnyKey);

            var added = collection.TryAddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>("k");

            Assert.IsFalse(added);
            Assert.AreEqual(1, collection.Count);
            Assert.AreSame(KeyedService.AnyKey, collection.Single().ServiceKey);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_TypeMapped_AnyKeyRegistrationPresent_KeyStillResolvesToWildcardImplementation()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>(KeyedService.AnyKey);

            collection.TryAddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>("k");

            var provider = collection.BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredKeyedService<IServiceInvoke>("k"), typeof(ServiceInvoke));
        }

        [TestMethod]
        public void TryAddUniqueKeyed_SelfRegistration_AnyKeyRegistrationPresent_ReturnsFalse()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<ServiceInvoke>(KeyedService.AnyKey);

            var added = collection.TryAddUniqueKeyed<ServiceInvoke>("k");

            Assert.IsFalse(added);
            Assert.AreEqual(1, collection.Count);
            Assert.AreSame(KeyedService.AnyKey, collection.Single().ServiceKey);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_Factory_AnyKeyRegistrationPresent_ReturnsFalse()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>(KeyedService.AnyKey);

            var added = collection.TryAddUniqueKeyed<IServiceInvoke>("k", (_, _) => new ServiceInvokeAlt());

            Assert.IsFalse(added);
            Assert.AreEqual(1, collection.Count);
            Assert.AreSame(KeyedService.AnyKey, collection.Single().ServiceKey);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_Factory_AnyKeyRegistrationPresent_KeyStillResolvesToWildcardImplementation()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>(KeyedService.AnyKey);

            collection.TryAddUniqueKeyed<IServiceInvoke>("k", (_, _) => new ServiceInvokeAlt());

            var provider = collection.BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredKeyedService<IServiceInvoke>("k"), typeof(ServiceInvoke));
        }

        [TestMethod]
        public void TryAddUniqueKeyed_TypeMapped_NoAnyKeyRegistration_ReturnsTrue()
        {
            var collection = new ServiceCollection();

            var added = collection.TryAddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>("k");

            Assert.IsTrue(added);
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual("k", collection.Single().ServiceKey);
            Assert.AreEqual(typeof(ServiceInvokeAlt), collection.Single().KeyedImplementationType);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_SelfRegistration_NoAnyKeyRegistration_ReturnsTrue()
        {
            var collection = new ServiceCollection();

            var added = collection.TryAddUniqueKeyed<ServiceInvoke>("k");

            Assert.IsTrue(added);
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual("k", collection.Single().ServiceKey);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_Factory_NoAnyKeyRegistration_ReturnsTrue()
        {
            var collection = new ServiceCollection();

            var added = collection.TryAddUniqueKeyed<IServiceInvoke>("k", (_, _) => new ServiceInvokeAlt());

            Assert.IsTrue(added);
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual("k", collection.Single().ServiceKey);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_AnyKeyRegistrationOfDifferentServiceType_DoesNotBlock()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvokeOne, ServiceInvokeOne>(KeyedService.AnyKey);

            var added = collection.TryAddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>("k");

            Assert.IsTrue(added);
            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(1,
                collection.Count(x => x.ServiceType == typeof(IServiceInvoke) && Equals(x.ServiceKey, "k")));
        }

        [TestMethod]
        public void TryAddUniqueKeyed_ExactKeyRegistrationPresent_ReturnsFalse()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("k");

            var added = collection.TryAddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>("k");

            Assert.IsFalse(added);
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(typeof(ServiceInvoke), collection.Single().KeyedImplementationType);
        }

        [TestMethod]
        public void AddUniqueKeyed_AnyKeyRegistrationPresent_WildcardDescriptorSurvivesByDesign()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>(KeyedService.AnyKey);
            var wildcard = collection.Single();

            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>("k");

            var survivors = collection
                .Where(x => x.IsKeyedService && ReferenceEquals(x.ServiceKey, KeyedService.AnyKey))
                .ToList();

            Assert.AreEqual(1, survivors.Count);
            Assert.AreSame(wildcard, survivors[0]);
            Assert.AreEqual(typeof(ServiceInvoke), survivors[0].KeyedImplementationType);
        }

        [TestMethod]
        public void AddUniqueKeyed_AnyKeyRegistrationPresent_ExactKeyDescriptorIsAdded()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>(KeyedService.AnyKey);

            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>("k");

            var exact = collection
                .Where(x => x.IsKeyedService && Equals(x.ServiceKey, "k"))
                .ToList();

            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(1, exact.Count);
            Assert.AreEqual(typeof(ServiceInvokeAlt), exact[0].KeyedImplementationType);
        }

        [TestMethod]
        public void AddUniqueKeyed_AnyKeyRegistrationPresent_ExactKeyWinsAndOtherKeysStillResolveThroughWildcard()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>(KeyedService.AnyKey);
            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>("k");

            var provider = collection.BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredKeyedService<IServiceInvoke>("k"), typeof(ServiceInvokeAlt));
            Assert.IsInstanceOfType(provider.GetRequiredKeyedService<IServiceInvoke>("other"), typeof(ServiceInvoke));
        }

        [TestMethod]
        public void IsSupported_OnThisRuntime_IsTrue()
        {
            Assert.IsTrue(InternalKeyedSupport.IsSupported);
        }

        [TestMethod]
        public void SCIsKeyedDescriptor_KeyedDescriptor_ReturnsTrue()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("k");

            Assert.IsTrue(InternalKeyedSupport.SCIsKeyedDescriptor(collection.Single()));
        }

        [TestMethod]
        public void SCIsKeyedDescriptor_NonKeyedDescriptor_ReturnsFalse()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            Assert.IsFalse(InternalKeyedSupport.SCIsKeyedDescriptor(collection.Single()));
        }

        [TestMethod]
        public void SCIsKeyedDescriptor_NullKeyRegistration_ReturnsFalse()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>(null);

            Assert.IsFalse(collection.Single().IsKeyedService);
            Assert.IsFalse(InternalKeyedSupport.SCIsKeyedDescriptor(collection.Single()));
        }

        [TestMethod]
        public void AnyKey_OnThisRuntime_IsTheContainerWildcard()
        {
            Assert.AreSame(KeyedService.AnyKey, InternalKeyedSupport.AnyKey);
        }
    }
}

#endif
