#region U S A G E S

using System;
using System.Linq;
using System.Threading.Tasks;
using ExtensionsTest.Modules.Abstractions;
using ExtensionsTest.Modules.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzR.Extensions.UniqueServiceCollection.ServiceCollectionExtensions;

#endregion

namespace ExtensionsTest.TestServiceCollectionExtensions
{
    [TestClass]
    public class GuardEdgeCaseTests
    {
        #region Open generics - the assignability guard must not break them

        [TestMethod]
        public void AddUnique_TypeOverload_OpenGenericSelfRegistration_IsAcceptedAndResolves()
        {
            var collection = new ServiceCollection();

            collection.AddUnique(typeof(Repo<>), ServiceLifetime.Transient);

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(typeof(Repo<>), collection[0].ServiceType);
            Assert.AreEqual(ServiceLifetime.Transient, collection[0].Lifetime);

            var provider = collection.BuildServiceProvider();
            Assert.IsInstanceOfType(provider.GetRequiredService<Repo<string>>(), typeof(Repo<string>));
        }

#if NET8_0_OR_GREATER

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_OpenGenericPair_IsAcceptedDespiteIsAssignableFromBeingFalse()
        {
            Assert.IsFalse(typeof(IRepo<>).IsAssignableFrom(typeof(Repo<>)));

            var collection = new ServiceCollection();

            collection.AddUniqueKeyed(typeof(IRepo<>), "tenantA", typeof(Repo<>), ServiceLifetime.Transient);

            Assert.AreEqual(1, collection.Count);
            Assert.IsTrue(collection[0].IsKeyedService);
            Assert.AreEqual("tenantA", collection[0].ServiceKey);
            Assert.AreEqual(typeof(IRepo<>), collection[0].ServiceType);
            Assert.AreEqual(typeof(Repo<>), collection[0].KeyedImplementationType);
            Assert.AreEqual(ServiceLifetime.Transient, collection[0].Lifetime);
        }

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_OpenGenericPair_ResolvesAsClosedKeyedService()
        {
            var collection = new ServiceCollection();
            collection.AddUniqueKeyed(typeof(IRepo<>), "tenantA", typeof(Repo<>), ServiceLifetime.Transient);

            var provider = collection.BuildServiceProvider();
            var resolved = provider.GetRequiredKeyedService<IRepo<string>>("tenantA");

            Assert.IsInstanceOfType(resolved, typeof(Repo<string>));
        }

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_ClosedGenericMismatch_ThrowsArgumentException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentException>(
                () => collection.AddUniqueKeyed(typeof(IRepo<string>), "tenantA", typeof(Repo<int>)));

            Assert.AreEqual("implementationType", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_OpenServiceWithClosedImplementation_ThrowsArgumentException()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedTransient(typeof(IRepo<>), "tenantA", typeof(Repo<>));
            var seeded = collection.Single();

            var ex = Assert.ThrowsException<ArgumentException>(
                () => collection.AddUniqueKeyed(typeof(IRepo<>), "tenantA", typeof(ServiceInvoke)));

            Assert.AreEqual("implementationType", ex.ParamName);
            Assert.AreEqual(1, collection.Count);
            Assert.AreSame(seeded, collection.Single());
            Assert.AreEqual(typeof(IRepo<>), collection.Single().ServiceType);
            Assert.AreEqual("tenantA", collection.Single().ServiceKey);
            Assert.AreEqual(typeof(Repo<>), collection.Single().KeyedImplementationType);
            Assert.AreEqual(ServiceLifetime.Transient, collection.Single().Lifetime);
        }

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_ClosedServiceWithOpenImplementation_ThrowsArgumentException()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton(typeof(IRepo<string>), "tenantA", typeof(Repo<string>));
            var seeded = collection.Single();

            var ex = Assert.ThrowsException<ArgumentException>(
                () => collection.AddUniqueKeyed(typeof(IRepo<string>), "tenantA", typeof(Repo<>)));

            Assert.AreEqual("implementationType", ex.ParamName);
            Assert.AreEqual(1, collection.Count);
            Assert.AreSame(seeded, collection.Single());
            Assert.AreEqual(typeof(IRepo<string>), collection.Single().ServiceType);
            Assert.AreEqual("tenantA", collection.Single().ServiceKey);
            Assert.AreEqual(typeof(Repo<string>), collection.Single().KeyedImplementationType);
            Assert.AreEqual(ServiceLifetime.Singleton, collection.Single().Lifetime);
        }

#endif

        #endregion

        #region Valid lifetimes still accepted by every guarded method

        [DataTestMethod]
        [DataRow(ServiceLifetime.Singleton)]
        [DataRow(ServiceLifetime.Scoped)]
        [DataRow(ServiceLifetime.Transient)]
        public void AddUnique_AllValidLifetimes_AreAccepted(ServiceLifetime lifetime)
        {
            AssertSingleRegistration(
                c => c.AddUnique<IServiceInvoke, ServiceInvoke>(lifetime), typeof(IServiceInvoke), lifetime);
            AssertSingleRegistration(
                c => c.AddUnique<IServiceInvoke>(_ => new ServiceInvoke(), lifetime), typeof(IServiceInvoke), lifetime);
            AssertSingleRegistration(
                c => c.AddUnique<ServiceInvoke>(lifetime), typeof(ServiceInvoke), lifetime);
            AssertSingleRegistration(
                c => c.AddUnique(typeof(ServiceInvoke), lifetime), typeof(ServiceInvoke), lifetime);
        }

        [DataTestMethod]
        [DataRow(ServiceLifetime.Singleton)]
        [DataRow(ServiceLifetime.Scoped)]
        [DataRow(ServiceLifetime.Transient)]
        public void TryAddUnique_AllValidLifetimes_AreAccepted(ServiceLifetime lifetime)
        {
            AssertSingleRegistration(
                c => Assert.IsTrue(c.TryAddUnique<IServiceInvoke, ServiceInvoke>(lifetime)),
                typeof(IServiceInvoke), lifetime);
            AssertSingleRegistration(
                c => Assert.IsTrue(c.TryAddUnique<ServiceInvoke>(lifetime)),
                typeof(ServiceInvoke), lifetime);
            AssertSingleRegistration(
                c => Assert.IsTrue(c.TryAddUnique<IServiceInvoke>(_ => new ServiceInvoke(), lifetime)),
                typeof(IServiceInvoke), lifetime);
        }

        [DataTestMethod]
        [DataRow(ServiceLifetime.Singleton)]
        [DataRow(ServiceLifetime.Scoped)]
        [DataRow(ServiceLifetime.Transient)]
        public void RegisterIfNotExistAndReplaceUnique_AllValidLifetimes_AreAccepted(ServiceLifetime lifetime)
        {
            AssertSingleRegistration(
                c => c.RegisterIfNotExist<IServiceInvoke, ServiceInvoke>(lifetime), typeof(IServiceInvoke), lifetime);
            AssertSingleRegistration(
                c => c.RegisterIfNotExist<ServiceInvoke>(lifetime), typeof(ServiceInvoke), lifetime);
            AssertSingleRegistration(
                c => c.RegisterIfNotExist<IServiceInvoke>(_ => new ServiceInvoke(), lifetime),
                typeof(IServiceInvoke), lifetime);
            AssertSingleRegistration(
                c => c.ReplaceUnique<IServiceInvoke, ServiceInvoke>(lifetime), typeof(IServiceInvoke), lifetime);
        }

#if NET8_0_OR_GREATER

        [DataTestMethod]
        [DataRow(ServiceLifetime.Singleton)]
        [DataRow(ServiceLifetime.Scoped)]
        [DataRow(ServiceLifetime.Transient)]
        public void AddUniqueKeyedAndTryAddUniqueKeyed_AllValidLifetimes_AreAccepted(ServiceLifetime lifetime)
        {
            AssertSingleRegistration(
                c => c.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA", lifetime),
                typeof(IServiceInvoke), lifetime);
            AssertSingleRegistration(
                c => c.AddUniqueKeyed<ServiceInvoke>("tenantA", lifetime), typeof(ServiceInvoke), lifetime);
            AssertSingleRegistration(
                c => c.AddUniqueKeyed<IServiceInvoke>("tenantA", (_, _) => new ServiceInvoke(), lifetime),
                typeof(IServiceInvoke), lifetime);
            AssertSingleRegistration(
                c => c.AddUniqueKeyed(typeof(IServiceInvoke), "tenantA", typeof(ServiceInvoke), lifetime),
                typeof(IServiceInvoke), lifetime);

            AssertSingleRegistration(
                c => Assert.IsTrue(c.TryAddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA", lifetime)),
                typeof(IServiceInvoke), lifetime);
            AssertSingleRegistration(
                c => Assert.IsTrue(c.TryAddUniqueKeyed<ServiceInvoke>("tenantA", lifetime)),
                typeof(ServiceInvoke), lifetime);
            AssertSingleRegistration(
                c => Assert.IsTrue(c.TryAddUniqueKeyed<IServiceInvoke>("tenantA", (_, _) => new ServiceInvoke(), lifetime)),
                typeof(IServiceInvoke), lifetime);
        }

#endif

        #endregion

        #region Boundary enum values

        [TestMethod]
        public void ServiceLifetimeZero_IsSingleton_AndIsAccepted()
        {
            Assert.AreEqual(ServiceLifetime.Singleton, (ServiceLifetime)0);

            var collection = new ServiceCollection();

            collection.AddUnique<IServiceInvoke, ServiceInvoke>((ServiceLifetime)0);

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(ServiceLifetime.Singleton, collection[0].Lifetime);
        }

#if NET8_0_OR_GREATER

        [TestMethod]
        public void ServiceLifetimeZero_IsAcceptedByKeyedTypeOverload()
        {
            var collection = new ServiceCollection();

            collection.AddUniqueKeyed(typeof(IServiceInvoke), "tenantA", typeof(ServiceInvoke), (ServiceLifetime)0);

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(ServiceLifetime.Singleton, collection[0].Lifetime);
        }

#endif

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(3)]
        [DataRow(99)]
        public void AddUnique_OutOfRangeLifetime_ThrowsArgumentOutOfRangeException(int rawLifetime)
        {
            var collection = new ServiceCollection();
            var lifetime = (ServiceLifetime)rawLifetime;

            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.AddUnique<IServiceInvoke, ServiceInvoke>(lifetime));

            Assert.AreEqual("lifetime", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(3)]
        public void TryAddUniqueAndReplaceUnique_OutOfRangeLifetime_ThrowsArgumentOutOfRangeException(int rawLifetime)
        {
            var lifetime = (ServiceLifetime)rawLifetime;

            AssertOutOfRange(c => c.TryAddUnique<IServiceInvoke, ServiceInvoke>(lifetime));
            AssertOutOfRange(c => c.TryAddUnique<ServiceInvoke>(lifetime));
            AssertOutOfRange(c => c.TryAddUnique<IServiceInvoke>(_ => new ServiceInvoke(), lifetime));
            AssertOutOfRange(c => c.ReplaceUnique<IServiceInvoke, ServiceInvoke>(lifetime));
            AssertOutOfRange(c => c.RegisterIfNotExist<IServiceInvoke, ServiceInvoke>(lifetime));
            AssertOutOfRange(c => c.AddUnique(typeof(ServiceInvoke), lifetime));
        }

#if NET8_0_OR_GREATER

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(3)]
        public void KeyedOverloads_OutOfRangeLifetime_ThrowsArgumentOutOfRangeException(int rawLifetime)
        {
            var lifetime = (ServiceLifetime)rawLifetime;

            AssertOutOfRange(c => c.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA", lifetime));
            AssertOutOfRange(c => c.AddUniqueKeyed<ServiceInvoke>("tenantA", lifetime));
            AssertOutOfRange(c => c.AddUniqueKeyed<IServiceInvoke>("tenantA", (_, _) => new ServiceInvoke(), lifetime));
            AssertOutOfRange(c => c.AddUniqueKeyed(typeof(IServiceInvoke), "tenantA", typeof(ServiceInvoke), lifetime));
            AssertOutOfRange(c => c.TryAddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA", lifetime));
            AssertOutOfRange(c => c.TryAddUniqueKeyed<ServiceInvoke>("tenantA", lifetime));
            AssertOutOfRange(c => c.TryAddUniqueKeyed<IServiceInvoke>("tenantA", (_, _) => new ServiceInvoke(), lifetime));
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(3)]
        public void AddUniqueKeyed_OutOfRangeLifetime_DoesNotRemovePreexistingRegistrations(int rawLifetime)
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.AddUniqueKeyed<IServiceInvoke, ServiceInvokeAlt>(
                    "tenantA", (ServiceLifetime)rawLifetime));

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(typeof(ServiceInvoke), collection[0].KeyedImplementationType);
        }

#endif

        #endregion

        #region Assignability shapes that must stay accepted

        [TestMethod]
        public void AddUnique_Instance_SelfRegistrationOfConcreteType_IsAccepted()
        {
            var collection = new ServiceCollection();
            var instance = new ServiceInvoke();

            collection.AddUnique(typeof(ServiceInvoke), instance);

            Assert.AreEqual(1, collection.Count);
            Assert.AreSame(instance, collection[0].ImplementationInstance);
        }

        [TestMethod]
        public void AddUnique_Instance_DerivedFromAbstractBase_IsAccepted()
        {
            var collection = new ServiceCollection();
            var instance = new DerivedFromAbstractService();

            collection.AddUnique(typeof(AbstractServiceBase), instance);

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(typeof(AbstractServiceBase), collection[0].ServiceType);
            Assert.AreSame(instance, collection[0].ImplementationInstance);
        }

        [TestMethod]
        public void AddUnique_Instance_ImplementsInterfaceThroughIntermediateBase_IsAccepted()
        {
            var collection = new ServiceCollection();
            var instance = new LeafViaIntermediate();

            collection.AddUnique(typeof(IServiceInvoke), instance);

            Assert.AreEqual(1, collection.Count);
            Assert.AreSame(instance, collection[0].ImplementationInstance);
        }

        [TestMethod]
        public void AddUnique_Instance_ValueTypeServiceWithMatchingBoxedInstance_IsAccepted()
        {
            var collection = new ServiceCollection();

            collection.AddUnique(typeof(int), 42);

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(42, (int)collection[0].ImplementationInstance);
        }

#if NET8_0_OR_GREATER

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_SelfRegistrationOfConcreteType_IsAccepted()
        {
            var collection = new ServiceCollection();

            collection.AddUniqueKeyed(typeof(ServiceInvoke), "tenantA", typeof(ServiceInvoke));

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(typeof(ServiceInvoke), collection[0].KeyedImplementationType);
        }

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_DerivedFromAbstractBase_IsAccepted()
        {
            var collection = new ServiceCollection();

            collection.AddUniqueKeyed(typeof(AbstractServiceBase), "tenantA", typeof(DerivedFromAbstractService));

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(typeof(DerivedFromAbstractService), collection[0].KeyedImplementationType);
        }

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_ImplementsInterfaceThroughIntermediateBase_IsAccepted()
        {
            var collection = new ServiceCollection();

            collection.AddUniqueKeyed(typeof(IServiceInvoke), "tenantA", typeof(LeafViaIntermediate));

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(typeof(LeafViaIntermediate), collection[0].KeyedImplementationType);
        }

#endif

        #endregion

        #region Assignability shapes that must be rejected

        [TestMethod]
        public void AddUnique_Instance_UnrelatedType_ThrowsArgumentException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentException>(
                () => collection.AddUnique(typeof(IServiceInvoke), new UnrelatedService()));

            Assert.AreEqual("instance", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddUnique_Instance_ImplementingADifferentInterface_ThrowsArgumentException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentException>(
                () => collection.AddUnique(typeof(IServiceInvoke), new ServiceInvokeOne()));

            Assert.AreEqual("instance", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddUnique_Instance_ValueTypeWhereReferenceTypeExpected_ThrowsArgumentException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentException>(
                () => collection.AddUnique(typeof(IServiceInvoke), 42));

            Assert.AreEqual("instance", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddUnique_Instance_RejectedRegistration_LeavesPreviousRegistrationIntact()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();

            Assert.ThrowsException<ArgumentException>(
                () => collection.AddUnique(typeof(IServiceInvoke), new UnrelatedService()));

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(typeof(ServiceInvoke), collection[0].ImplementationType);
        }

#if NET8_0_OR_GREATER

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_UnrelatedType_ThrowsArgumentException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentException>(
                () => collection.AddUniqueKeyed(typeof(IServiceInvoke), "tenantA", typeof(UnrelatedService)));

            Assert.AreEqual("implementationType", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_ImplementingADifferentInterface_ThrowsArgumentException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentException>(
                () => collection.AddUniqueKeyed(typeof(IServiceInvoke), "tenantA", typeof(ServiceInvokeOne)));

            Assert.AreEqual("implementationType", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_RejectedRegistration_LeavesPreviousKeyedRegistrationIntact()
        {
            var collection = new ServiceCollection();
            collection.AddKeyedSingleton<IServiceInvoke, ServiceInvoke>("tenantA");

            Assert.ThrowsException<ArgumentException>(
                () => collection.AddUniqueKeyed(typeof(IServiceInvoke), "tenantA", typeof(UnrelatedService)));

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(typeof(ServiceInvoke), collection[0].KeyedImplementationType);
        }

#endif

        #endregion

        #region Guard ordering / precedence

#if NET8_0_OR_GREATER

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_InvalidLifetimeAndInvalidAssignability_LifetimeWins()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.AddUniqueKeyed(
                    typeof(IServiceInvoke), "tenantA", typeof(UnrelatedService), (ServiceLifetime)99));

            Assert.AreEqual("lifetime", ex.ParamName);
        }

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_InvalidKeyAndInvalidLifetime_KeyWins()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUniqueKeyed(
                    typeof(IServiceInvoke), null, typeof(ServiceInvoke), (ServiceLifetime)99));

            Assert.AreEqual("serviceKey", ex.ParamName);
        }

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_NullCollection_TakesPrecedenceOverEveryOtherGuard()
        {
            IServiceCollection collection = null;

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUniqueKeyed(null, null, null, (ServiceLifetime)99));

            Assert.AreEqual("serviceCollection", ex.ParamName);
        }

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_NullServiceType_TakesPrecedenceOverLifetimeAndAssignability()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUniqueKeyed(null, "tenantA", typeof(ServiceInvoke), (ServiceLifetime)99));

            Assert.AreEqual("serviceType", ex.ParamName);
        }

        [TestMethod]
        public void AddUniqueKeyed_TypeOverload_NullImplementationType_TakesPrecedenceOverLifetimeAndAssignability()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUniqueKeyed(typeof(IServiceInvoke), "tenantA", null, (ServiceLifetime)99));

            Assert.AreEqual("implementationType", ex.ParamName);
        }

#endif

        [TestMethod]
        public void AddUnique_Instance_NullServiceType_TakesPrecedenceOverNullInstance()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUnique(null, (object)null));

            Assert.AreEqual("serviceType", ex.ParamName);
        }

        [TestMethod]
        public void AddUnique_Instance_NullInstance_TakesPrecedenceOverAssignabilityGuard()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUnique(typeof(IServiceInvoke), (object)null));

            Assert.AreEqual("instance", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddUnique_Instance_NullCollection_TakesPrecedenceOverEveryOtherGuard()
        {
            IServiceCollection collection = null;

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUnique(null, (object)null));

            Assert.AreEqual("serviceCollection", ex.ParamName);
        }

        [TestMethod]
        public void AddUnique_TypeOverload_NullServiceType_ThrowsArgumentNullException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUnique((Type)null, ServiceLifetime.Scoped));

            Assert.AreEqual("collectionType", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddUnique_TypeOverload_NullServiceTypeAndInvalidLifetime_LifetimeWins()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.AddUnique((Type)null, (ServiceLifetime)99));

            Assert.AreEqual("lifetime", ex.ParamName);
        }

        #endregion

        #region Helpers

        private static void AssertSingleRegistration(Action<IServiceCollection> register,
            Type expectedServiceType, ServiceLifetime expectedLifetime)
        {
            var collection = new ServiceCollection();

            register(collection);

            Assert.AreEqual(1, collection.Count);
            var descriptor = collection.Single();
            Assert.AreEqual(expectedServiceType, descriptor.ServiceType);
            Assert.AreEqual(expectedLifetime, descriptor.Lifetime);
        }

        private static void AssertOutOfRange(Action<IServiceCollection> register)
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(() => register(collection));

            Assert.AreEqual("lifetime", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        #endregion
    }
}
