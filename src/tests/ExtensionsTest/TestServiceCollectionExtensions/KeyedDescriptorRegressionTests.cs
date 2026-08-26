#if NET8_0_OR_GREATER

#region U S A G E S

using System;
using System.Linq;
using ExtensionsTest.Modules.Abstractions;
using ExtensionsTest.Modules.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzR.Extensions.UniqueServiceCollection.Extensions;
using RzR.Extensions.UniqueServiceCollection.ServiceCollectionExtensions;

#endregion

namespace ExtensionsTest.TestServiceCollectionExtensions
{
    [TestClass]
    public class KeyedDescriptorRegressionTests
    {
        [TestMethod]
        public void CheckAndCleanUpAllDuplicates_DistinctKeyedRegistrations_BothPreserved()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvokeAlt>("tenantB");

            collection.CheckAndCleanUpAllDuplicates();

            Assert.AreEqual(2, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
        }

        [TestMethod]
        public void CheckAndCleanUpAllDuplicates_SameImplementationDifferentKeys_BothPreserved()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantB");

            collection.CheckAndCleanUpAllDuplicates();

            Assert.AreEqual(2, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
        }

        [TestMethod]
        public void FindExactDuplicates_DistinctKeyedRegistrations_NotReported()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvokeAlt>("tenantB");

            var reports = collection.FindExactDuplicates().ToList();

            Assert.AreEqual(0, reports.Count);
        }

        [TestMethod]
        public void ValidateNoDuplicates_KeyedRegistrationsPresent_DoesNotThrow()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvokeAlt>("tenantB");

            var result = collection.ValidateNoDuplicates();

            Assert.AreSame(collection, result);
        }

        [TestMethod]
        public void CheckAndCleanUpAllDuplicates_MixedKeyedAndNonKeyed_OnlyNonKeyedCollapsed()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantB");

            collection.CheckAndCleanUpAllDuplicates();

            Assert.AreEqual(3, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
        }

        [TestMethod]
        public void AddUnique_WithKeyedRegistrationPresent_StillRegistersNonKeyedService()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");

            collection.AddUnique<IServiceInvoke, ServiceInvokeAlt>();

            var nonKeyed = collection
                .Where(x => x.ServiceType == typeof(IServiceInvoke) && !x.IsKeyedService)
                .ToList();

            Assert.AreEqual(1, nonKeyed.Count);
            Assert.AreEqual(typeof(ServiceInvokeAlt), nonKeyed[0].ImplementationType);
        }

        [TestMethod]
        public void AddUnique_WithKeyedRegistrationPresent_LeavesKeyedRegistrationUntouched()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");

            collection.AddUnique<IServiceInvoke, ServiceInvokeAlt>();

            var keyed = collection
                .Where(x => x.ServiceType == typeof(IServiceInvoke) && x.IsKeyedService)
                .ToList();

            Assert.AreEqual(1, keyed.Count);
            Assert.AreEqual("tenantA", keyed[0].ServiceKey);
            Assert.AreEqual(typeof(ServiceInvoke), keyed[0].KeyedImplementationType);
        }

        [TestMethod]
        public void AddUnique_WithKeyedRegistrationPresent_ResolvesBothIndependently()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");
            collection.AddUnique<IServiceInvoke, ServiceInvokeAlt>();

            var provider = collection.BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IServiceInvoke>(), typeof(ServiceInvokeAlt));
            Assert.IsInstanceOfType(provider.GetRequiredKeyedService<IServiceInvoke>("tenantA"), typeof(ServiceInvoke));
        }

        [TestMethod]
        public void RegisterIfNotExist_WithOnlyKeyedRegistrationPresent_StillRegisters()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");

            collection.RegisterIfNotExist<IServiceInvoke, ServiceInvokeAlt>();

            Assert.AreEqual(1,
                collection.Count(x => x.ServiceType == typeof(IServiceInvoke) && !x.IsKeyedService));
        }

        [TestMethod]
        public void TryAddUnique_WithOnlyKeyedRegistrationPresent_StillRegisters()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");

            var added = collection.TryAddUnique<IServiceInvoke, ServiceInvokeAlt>();

            Assert.IsTrue(added);
            Assert.AreEqual(1,
                collection.Count(x => x.ServiceType == typeof(IServiceInvoke) && !x.IsKeyedService));
            Assert.AreEqual(1,
                collection.Count(x => x.ServiceType == typeof(IServiceInvoke) && x.IsKeyedService));
        }

        [TestMethod]
        public void SCIsKeyed_MatchesIsKeyedService_ForEveryDescriptorShape()
        {
            var instance = new ServiceInvoke();
            IServiceCollection collection = new ServiceCollection();

            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddScoped<IServiceInvoke, ServiceInvoke>();
            collection.AddTransient<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke>(instance);
            collection.AddSingleton(typeof(IServiceInvoke), instance);
            collection.AddSingleton<IServiceInvoke>(_ => new ServiceInvoke());
            collection.AddSingleton<ServiceInvoke>();
            collection.Add(ServiceDescriptor.Describe(typeof(IServiceInvoke), typeof(ServiceInvoke), ServiceLifetime.Scoped));

            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("k");
            collection.AddKeyedScoped<IServiceInvoke, ServiceInvoke>("k");
            collection.AddKeyedTransient<IServiceInvoke, ServiceInvoke>("k");
            collection.AddKeyedSingleton<IServiceInvoke>("k", instance);
            collection.AddKeyedSingleton<IServiceInvoke>("k", (_, _) => new ServiceInvoke());
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>(KeyedService.AnyKey);

            foreach (var descriptor in collection)
                Assert.AreEqual(descriptor.IsKeyedService, descriptor.SCIsKeyed());
        }

        [TestMethod]
        public void SCIsKeyed_NullKeyIsTreatedAsNonKeyed()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>(null);

            Assert.IsFalse(collection[0].IsKeyedService);
            Assert.IsFalse(collection[0].SCIsKeyed());
        }

        [TestMethod]
        public void ReplaceUnique_WithOnlyKeyedRegistrationPresent_ReturnsFalse()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");

            var replaced = collection.ReplaceUnique<IServiceInvoke, ServiceInvokeAlt>();

            Assert.IsFalse(replaced);
        }

        [TestMethod]
        public void ReplaceUnique_WithOnlyKeyedRegistrationPresent_LeavesKeyedRegistrationUntouched()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");
            var original = collection.Single();

            collection.ReplaceUnique<IServiceInvoke, ServiceInvokeAlt>();

            var keyed = collection
                .Where(x => x.ServiceType == typeof(IServiceInvoke) && x.IsKeyedService)
                .ToList();

            Assert.AreEqual(1, keyed.Count);
            Assert.AreSame(original, keyed[0]);
            Assert.AreEqual("tenantA", keyed[0].ServiceKey);
            Assert.AreEqual(typeof(ServiceInvoke), keyed[0].KeyedImplementationType);
            Assert.AreEqual(ServiceLifetime.Singleton, keyed[0].Lifetime);
        }

        [TestMethod]
        public void ReplaceUnique_WithOnlyKeyedRegistrationPresent_RegistersNonKeyedService()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");

            collection.ReplaceUnique<IServiceInvoke, ServiceInvokeAlt>();

            var nonKeyed = collection
                .Where(x => x.ServiceType == typeof(IServiceInvoke) && !x.IsKeyedService)
                .ToList();

            Assert.AreEqual(1, nonKeyed.Count);
            Assert.AreEqual(typeof(ServiceInvokeAlt), nonKeyed[0].ImplementationType);
        }

        [TestMethod]
        public void ReplaceUnique_WithNonKeyedRegistrationPresent_ReturnsTrue()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            var replaced = collection.ReplaceUnique<IServiceInvoke, ServiceInvokeAlt>();

            Assert.IsTrue(replaced);
            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
            Assert.AreEqual(typeof(ServiceInvokeAlt),
                collection.Single(x => x.ServiceType == typeof(IServiceInvoke)).ImplementationType);
        }

        [TestMethod]
        public void AddUniqueKeyed_ImplementationTypeNotAssignableToServiceType_ThrowsArgumentException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentException>(
                () => collection.AddUniqueKeyed(
                    typeof(IServiceInvoke), "k", typeof(ServiceInvokeOne), ServiceLifetime.Singleton));

            Assert.AreEqual("implementationType", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddUniqueKeyed_ImplementationTypeNotAssignable_LeavesExistingKeyedRegistrationIntact()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("k");
            var original = collection.Single();

            var ex = Assert.ThrowsException<ArgumentException>(
                () => collection.AddUniqueKeyed(
                    typeof(IServiceInvoke), "k", typeof(ServiceInvokeOne), ServiceLifetime.Singleton));

            Assert.AreEqual("implementationType", ex.ParamName);
            Assert.AreEqual(1, collection.Count);
            Assert.AreSame(original, collection.Single());
            Assert.AreEqual(typeof(ServiceInvoke), collection.Single().KeyedImplementationType);
            Assert.AreEqual(ServiceLifetime.Singleton, collection.Single().Lifetime);
        }

        #region Keyed detection must never read a throwing implementation accessor

        [TestMethod]
        public void SCIsKeyed_DelegateIsBoundOnThisRuntime_ReportsKeyedAndNonKeyedExactly()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");
            collection.AddSingleton<IServiceInvoke, ServiceInvokeAlt>();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>(null);

            Assert.AreEqual(3, collection.Count);
            Assert.IsTrue(collection[0].SCIsKeyed());
            Assert.IsFalse(collection[1].SCIsKeyed());
            Assert.IsFalse(collection[2].IsKeyedService);
            Assert.IsFalse(collection[2].SCIsKeyed());
        }

        private static ServiceCollection SeedKeyedDescriptorOfEveryShape()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("byType");
            collection.AddKeyedSingleton<IServiceInvoke>("byInstance", new ServiceInvoke());
            collection.AddKeyedSingleton<IServiceInvoke>("byFactory", (_, _) => new ServiceInvokeAlt());

            return collection;
        }

        private static void AssertKeyedShapesSurvived(IServiceCollection collection)
        {
            var keyed = collection
                .Where(x => x.ServiceType == typeof(IServiceInvoke) && x.IsKeyedService)
                .ToList();

            Assert.AreEqual(3, keyed.Count);
            CollectionAssert.AreEquivalent(
                new object[] { "byType", "byInstance", "byFactory" },
                keyed.Select(x => x.ServiceKey).ToArray());
        }

        [TestMethod]
        public void AddUnique_WithKeyedDescriptorOfEveryShapePresent_RegistersWithoutThrowing()
        {
            var collection = SeedKeyedDescriptorOfEveryShape();

            collection.AddUnique<IServiceInvoke, ServiceInvokeAlt>();

            var nonKeyed = collection
                .Where(x => x.ServiceType == typeof(IServiceInvoke) && !x.IsKeyedService)
                .ToList();

            Assert.AreEqual(1, nonKeyed.Count);
            Assert.AreEqual(typeof(ServiceInvokeAlt), nonKeyed[0].ImplementationType);
            AssertKeyedShapesSurvived(collection);
        }

        [TestMethod]
        public void TryAddUnique_WithKeyedDescriptorOfEveryShapePresent_RegistersWithoutThrowing()
        {
            var collection = SeedKeyedDescriptorOfEveryShape();

            var added = collection.TryAddUnique<IServiceInvoke, ServiceInvokeAlt>();

            Assert.IsTrue(added);
            Assert.AreEqual(1,
                collection.Count(x => x.ServiceType == typeof(IServiceInvoke) && !x.IsKeyedService));
            AssertKeyedShapesSurvived(collection);
        }

        [TestMethod]
        public void ReplaceUnique_WithKeyedDescriptorOfEveryShapePresent_RegistersWithoutThrowing()
        {
            var collection = SeedKeyedDescriptorOfEveryShape();

            var replaced = collection.ReplaceUnique<IServiceInvoke, ServiceInvokeAlt>();

            Assert.IsFalse(replaced);
            Assert.AreEqual(1,
                collection.Count(x => x.ServiceType == typeof(IServiceInvoke) && !x.IsKeyedService));
            AssertKeyedShapesSurvived(collection);
        }

        [TestMethod]
        public void RegisterIfNotExist_WithKeyedDescriptorOfEveryShapePresent_RegistersWithoutThrowing()
        {
            var collection = SeedKeyedDescriptorOfEveryShape();

            collection.RegisterIfNotExist<IServiceInvoke, ServiceInvokeAlt>();

            Assert.AreEqual(1,
                collection.Count(x => x.ServiceType == typeof(IServiceInvoke) && !x.IsKeyedService));
            AssertKeyedShapesSurvived(collection);
        }

        [TestMethod]
        public void CheckAndCleanUpAllDuplicates_WithKeyedDescriptorOfEveryShapePresent_DoesNotThrow()
        {
            var collection = SeedKeyedDescriptorOfEveryShape();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            collection.CheckAndCleanUpAllDuplicates();

            Assert.AreEqual(1,
                collection.Count(x => x.ServiceType == typeof(IServiceInvoke) && !x.IsKeyedService));
            AssertKeyedShapesSurvived(collection);
        }

        [TestMethod]
        public void FindExactDuplicates_WithKeyedDescriptorOfEveryShapePresent_ReportsOnlyTheNonKeyedDuplicate()
        {
            var collection = SeedKeyedDescriptorOfEveryShape();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            var reports = collection.FindExactDuplicates().ToList();

            Assert.AreEqual(1, reports.Count);
            Assert.AreEqual(typeof(IServiceInvoke), reports[0].RetainedDescriptor.ServiceType);
            Assert.AreEqual(1, reports[0].DuplicateRegistrations.Count);
        }

        [TestMethod]
        public void FindExactDuplicates_WithOnlyKeyedDescriptorsOfEveryShape_ReportsNothing()
        {
            var collection = SeedKeyedDescriptorOfEveryShape();

            var reports = collection.FindExactDuplicates().ToList();

            Assert.AreEqual(0, reports.Count);
            AssertKeyedShapesSurvived(collection);
        }

        [TestMethod]
        public void ValidateNoDuplicates_WithKeyedDescriptorOfEveryShapePresent_DoesNotThrow()
        {
            var collection = SeedKeyedDescriptorOfEveryShape();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            var result = collection.ValidateNoDuplicates();

            Assert.AreSame(collection, result);
            AssertKeyedShapesSurvived(collection);
        }

        [TestMethod]
        public void NonKeyedApis_WithKeyedDescriptorOfEveryShapePresent_ResolveIndependently()
        {
            var collection = SeedKeyedDescriptorOfEveryShape();
            collection.AddUnique<IServiceInvoke, ServiceInvokeAlt>();

            var provider = collection.BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IServiceInvoke>(), typeof(ServiceInvokeAlt));
            Assert.IsInstanceOfType(provider.GetRequiredKeyedService<IServiceInvoke>("byType"), typeof(ServiceInvoke));
            Assert.IsInstanceOfType(provider.GetRequiredKeyedService<IServiceInvoke>("byInstance"), typeof(ServiceInvoke));
            Assert.IsInstanceOfType(provider.GetRequiredKeyedService<IServiceInvoke>("byFactory"), typeof(ServiceInvokeAlt));
        }

        #endregion
    }
}

#endif
