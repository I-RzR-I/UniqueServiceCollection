#if NET8_0_OR_GREATER

#region U S A G E S

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

            Assert.IsTrue(added, "A keyed registration must not block a non-keyed TryAddUnique.");
            Assert.AreEqual(1,
                collection.Count(x => x.ServiceType == typeof(IServiceInvoke) && !x.IsKeyedService),
                "The non-keyed slot must be filled.");
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
    }
}

#endif
