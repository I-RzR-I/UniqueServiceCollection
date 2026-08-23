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
    public class TryAddUniqueExtensionsTests
    {
        [TestMethod]
        public void TryAddUnique_FirstRegistration_ReturnsTrueAndRegisters()
        {
            var collection = new ServiceCollection();

            var added = collection.TryAddUnique<IServiceInvoke, ServiceInvoke>();

            Assert.IsTrue(added);
            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
            Assert.AreEqual(typeof(ServiceInvoke), collection[0].ImplementationType);
            Assert.AreEqual(ServiceLifetime.Singleton, collection[0].Lifetime);
        }

        [TestMethod]
        public void TryAddUnique_SecondRegistration_ReturnsFalseAndDoesNotAdd()
        {
            var collection = new ServiceCollection();
            collection.TryAddUnique<IServiceInvoke, ServiceInvoke>();

            var added = collection.TryAddUnique<IServiceInvoke, ServiceInvoke>();

            Assert.IsFalse(added);
            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
        }

        [TestMethod]
        public void TryAddUnique_DifferentImplementation_StillFirstWins()
        {
            var collection = new ServiceCollection();
            collection.TryAddUnique<IServiceInvoke, ServiceInvoke>();

            var added = collection.TryAddUnique<IServiceInvoke, ServiceInvokeAlt>();

            Assert.IsFalse(added);
            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
            Assert.AreEqual(typeof(ServiceInvoke), collection[0].ImplementationType);
        }

        [TestMethod]
        public void TryAddUnique_NeverRemovesExistingRegistrations()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            collection.AddSingleton<IServiceInvoke, ServiceInvokeAlt>();

            var added = collection.TryAddUnique<IServiceInvoke, ServiceInvoke>();

            Assert.IsFalse(added);
            Assert.AreEqual(2, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
        }

        [TestMethod]
        public void TryAddUnique_Idempotent_AcrossRepeatedCalls()
        {
            var collection = new ServiceCollection();

            var results = Enumerable.Range(0, 5)
                .Select(_ => collection.TryAddUnique<IServiceInvoke, ServiceInvoke>())
                .ToList();

            Assert.IsTrue(results[0]);
            Assert.IsTrue(results.Skip(1).All(x => x == false));
            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
        }

        [TestMethod]
        public void TryAddUnique_RespectsLifetime()
        {
            var collection = new ServiceCollection();

            collection.TryAddUnique<IServiceInvoke, ServiceInvoke>(ServiceLifetime.Scoped);

            Assert.AreEqual(ServiceLifetime.Scoped, collection[0].Lifetime);
        }

        [TestMethod]
        public void TryAddUnique_SelfRegistration_RegistersConcreteType()
        {
            var collection = new ServiceCollection();

            var added = collection.TryAddUnique<ServiceInvoke>(ServiceLifetime.Transient);

            Assert.IsTrue(added);
            Assert.AreEqual(typeof(ServiceInvoke), collection[0].ServiceType);
            Assert.AreEqual(ServiceLifetime.Transient, collection[0].Lifetime);
        }

        [TestMethod]
        public void TryAddUnique_Factory_FirstWins()
        {
            var collection = new ServiceCollection();

            var first = collection.TryAddUnique<IServiceInvoke>(_ => new ServiceInvoke());
            var second = collection.TryAddUnique<IServiceInvoke>(_ => new ServiceInvokeAlt());

            Assert.IsTrue(first);
            Assert.IsFalse(second);
            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvoke)));
        }

        [TestMethod]
        public void TryAddUnique_Factory_ResolvesThroughProvider()
        {
            var collection = new ServiceCollection();
            collection.TryAddUnique<IServiceInvoke>(_ => new ServiceInvokeAlt());

            var provider = collection.BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IServiceInvoke>(), typeof(ServiceInvokeAlt));
        }

        [TestMethod]
        public void TryAddUnique_NullCollection_ThrowsArgumentNullException()
        {
            IServiceCollection collection = null;

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => collection.TryAddUnique<IServiceInvoke, ServiceInvoke>());

            Assert.AreEqual("serviceCollection", ex.ParamName);
        }

        [TestMethod]
        public void TryAddUnique_Factory_NullFactory_ThrowsArgumentNullException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => collection.TryAddUnique<IServiceInvoke>((Func<IServiceProvider, IServiceInvoke>)null));

            Assert.AreEqual("factory", ex.ParamName);
        }

        [TestMethod]
        public void TryAddUnique_InvalidLifetime_ThrowsArgumentOutOfRangeExceptionAndAddsNothing()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.TryAddUnique<IServiceInvoke, ServiceInvoke>((ServiceLifetime)99));

            Assert.AreEqual("lifetime", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void TryAddUnique_Factory_InvalidLifetime_ThrowsArgumentOutOfRangeException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.TryAddUnique<IServiceInvoke>(_ => new ServiceInvoke(), (ServiceLifetime)99));

            Assert.AreEqual("lifetime", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void TryAddUnique_DoesNotAffectUnrelatedServiceTypes()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvokeOne, ServiceInvokeOne>();
            collection.AddSingleton<IServiceInvokeTwo, ServiceInvokeTwo>();

            collection.TryAddUnique<IServiceInvoke, ServiceInvoke>();

            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvokeOne)));
            Assert.AreEqual(1, collection.Count(x => x.ServiceType == typeof(IServiceInvokeTwo)));
        }
    }
}
