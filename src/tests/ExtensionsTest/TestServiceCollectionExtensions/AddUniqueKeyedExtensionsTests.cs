#if NET8_0_OR_GREATER

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
    public class AddUniqueKeyedExtensionsTests
    {
        private enum Tenant
        {
            First,
            Second
        }

        [TestMethod]
        public void AddUniqueKeyed_FirstRegistration_RegistersWithKeyAndImplementation()
        {
            var collection = new ServiceCollection();

            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA");

            Assert.AreEqual(1, collection.Count);
            Assert.IsTrue(collection[0].IsKeyedService);
            Assert.AreEqual("tenantA", collection[0].ServiceKey);
            Assert.AreEqual(typeof(ServiceInvoke), collection[0].KeyedImplementationType);
            Assert.AreEqual(ServiceLifetime.Singleton, collection[0].Lifetime);
        }

        [TestMethod]
        public void AddUniqueKeyed_SameKey_LastWins()
        {
            var collection = new ServiceCollection();
            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA");

            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>("tenantA");

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(typeof(ServiceInvokeAlt), collection[0].KeyedImplementationType);
        }

        [TestMethod]
        public void AddUniqueKeyed_SiblingKey_IsPreserved()
        {
            var collection = new ServiceCollection();
            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantB");

            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>("tenantA");

            Assert.AreEqual(2, collection.Count);
            Assert.IsTrue(collection.Any(x => Equals(x.ServiceKey, "tenantB")
                                              && x.KeyedImplementationType == typeof(ServiceInvoke)));
        }

        [TestMethod]
        public void AddUniqueKeyed_NonKeyedRegistration_IsPreserved()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>("tenantA");

            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(1, collection.Count(x => !x.IsKeyedService),
                "The non-keyed registration must survive a keyed add.");
        }

        [TestMethod]
        public void AddUniqueKeyed_CollapsesMultiplePriorRegistrationsUnderSameKey()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");
            collection.AddKeyedScoped<IServiceInvoke, ServiceInvokeAlt>("tenantA");

            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA", ServiceLifetime.Transient);

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(ServiceLifetime.Transient, collection[0].Lifetime);
        }

        [TestMethod]
        public void AddUniqueKeyed_KeyComparisonIsByValueNotReference()
        {
            var collection = new ServiceCollection();
            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA");

            var equalButDistinct = new string("tenantA".ToCharArray());
            Assert.IsFalse(ReferenceEquals("tenantA", equalButDistinct));

            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>(equalButDistinct);

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(typeof(ServiceInvokeAlt), collection[0].KeyedImplementationType);
        }

        [TestMethod]
        public void AddUniqueKeyed_SupportsEnumAndNumericKeys()
        {
            var collection = new ServiceCollection();

            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>(Tenant.First);
            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>(Tenant.Second);
            collection.AddUniqueKeyed<IServiceInvokeOne, ServiceInvokeOne>(7);

            Assert.AreEqual(3, collection.Count);

            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>(Tenant.Second);
            Assert.AreEqual(3, collection.Count);
        }

        [TestMethod]
        public void AddUniqueKeyed_SelfRegistration_RegistersConcreteType()
        {
            var collection = new ServiceCollection();

            collection.AddUniqueKeyed<ServiceInvoke>("tenantA", ServiceLifetime.Scoped);

            Assert.AreEqual(typeof(ServiceInvoke), collection[0].ServiceType);
            Assert.AreEqual(typeof(ServiceInvoke), collection[0].KeyedImplementationType);
            Assert.AreEqual(ServiceLifetime.Scoped, collection[0].Lifetime);
        }

        [TestMethod]
        public void AddUniqueKeyed_TypeBasedOverload_Registers()
        {
            var collection = new ServiceCollection();

            collection.AddUniqueKeyed(typeof(IServiceInvoke), "tenantA", typeof(ServiceInvoke), ServiceLifetime.Scoped);

            Assert.IsTrue(collection[0].IsKeyedService);
            Assert.AreEqual(typeof(ServiceInvoke), collection[0].KeyedImplementationType);
            Assert.AreEqual(ServiceLifetime.Scoped, collection[0].Lifetime);
        }

        [TestMethod]
        public void AddUniqueKeyed_Factory_ReceivesResolvedKey()
        {
            var collection = new ServiceCollection();
            object observedKey = null;

            collection.AddUniqueKeyed<IServiceInvoke>("tenantA", (_, key) =>
            {
                observedKey = key;
                return new ServiceInvoke();
            });

            var provider = collection.BuildServiceProvider();
            var resolved = provider.GetRequiredKeyedService<IServiceInvoke>("tenantA");

            Assert.IsInstanceOfType(resolved, typeof(ServiceInvoke));
            Assert.AreEqual("tenantA", observedKey);
        }

        [TestMethod]
        public void AddUniqueKeyed_ResolvesEachKeyIndependently()
        {
            var collection = new ServiceCollection();
            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA");
            collection.AddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>("tenantB");
            collection.AddUnique<IServiceInvoke, ServiceInvoke>();

            var provider = collection.BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredKeyedService<IServiceInvoke>("tenantA"), typeof(ServiceInvoke));
            Assert.IsInstanceOfType(provider.GetRequiredKeyedService<IServiceInvoke>("tenantB"), typeof(ServiceInvokeAlt));
            Assert.IsInstanceOfType(provider.GetRequiredService<IServiceInvoke>(), typeof(ServiceInvoke));
        }

        [TestMethod]
        public void AddUniqueKeyed_ReturnsServiceCollection_ForFluentChaining()
        {
            var collection = new ServiceCollection();

            var result = collection.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA");

            Assert.AreSame(collection, result);
        }

        [TestMethod]
        public void AddUniqueKeyed_NullKey_ThrowsArgumentNullException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>(null));

            Assert.AreEqual("serviceKey", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddUniqueKeyed_AnyKey_ThrowsArgumentException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentException>(
                () => collection.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>(KeyedService.AnyKey));

            Assert.AreEqual("serviceKey", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddUniqueKeyed_NullCollection_ThrowsArgumentNullException()
        {
            IServiceCollection collection = null;

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA"));

            Assert.AreEqual("serviceCollection", ex.ParamName);
        }

        [TestMethod]
        public void AddUniqueKeyed_InvalidLifetime_ThrowsArgumentOutOfRangeException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA", (ServiceLifetime)99));

            Assert.AreEqual("lifetime", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddUniqueKeyed_Factory_NullFactory_ThrowsArgumentNullException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUniqueKeyed<IServiceInvoke>(
                    "tenantA", (Func<IServiceProvider, object, IServiceInvoke>)null));

            Assert.AreEqual("factory", ex.ParamName);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_FirstRegistration_ReturnsTrue()
        {
            var collection = new ServiceCollection();

            var added = collection.TryAddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA");

            Assert.IsTrue(added);
            Assert.AreEqual(1, collection.Count);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_SameKeyAgain_ReturnsFalseAndKeepsOriginal()
        {
            var collection = new ServiceCollection();
            collection.TryAddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA");

            var added = collection.TryAddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>("tenantA");

            Assert.IsFalse(added);
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(typeof(ServiceInvoke), collection[0].KeyedImplementationType);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_DifferentKey_ReturnsTrue()
        {
            var collection = new ServiceCollection();
            collection.TryAddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA");

            var added = collection.TryAddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantB");

            Assert.IsTrue(added);
            Assert.AreEqual(2, collection.Count);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_NonKeyedRegistrationPresent_ReturnsTrue()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            var added = collection.TryAddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>("tenantA");

            Assert.IsTrue(added);
            Assert.AreEqual(2, collection.Count);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_SelfRegistrationAndFactoryOverloads_Work()
        {
            var collection = new ServiceCollection();

            Assert.IsTrue(collection.TryAddUniqueKeyed<ServiceInvoke>("tenantA", ServiceLifetime.Scoped));
            Assert.IsFalse(collection.TryAddUniqueKeyed<ServiceInvoke>("tenantA"));

            Assert.IsTrue(collection.TryAddUniqueKeyed<IServiceInvoke>("tenantB", (_, _) => new ServiceInvoke()));
            Assert.IsFalse(collection.TryAddUniqueKeyed<IServiceInvoke>("tenantB", (_, _) => new ServiceInvokeAlt()));

            Assert.AreEqual(2, collection.Count);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_NullKey_ThrowsArgumentNullException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => collection.TryAddUniqueKeyed<IServiceInvoke, ServiceInvoke>(null));

            Assert.AreEqual("serviceKey", ex.ParamName);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_AnyKey_ThrowsArgumentException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentException>(
                () => collection.TryAddUniqueKeyed<IServiceInvoke, ServiceInvoke>(KeyedService.AnyKey));

            Assert.AreEqual("serviceKey", ex.ParamName);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_NeverRemovesExistingRegistrations()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvokeAlt>("tenantB");
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            collection.TryAddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>("tenantA");

            Assert.AreEqual(3, collection.Count);
        }
    }
}

#endif
