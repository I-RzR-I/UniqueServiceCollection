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
    public class ValidationBeforeMutationTests
    {
        private const ServiceLifetime InvalidLifetime = (ServiceLifetime)77;

        #region Invalid lifetime must be rejected before the collection is touched

        [TestMethod]
        public void AddUnique_ServiceAndImplementation_InvalidLifetime_ThrowsAndLeavesCollectionUnchanged()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            var original = collection.Single();

            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.AddUnique<IServiceInvoke, ServiceInvokeAlt>(InvalidLifetime));

            Assert.AreEqual("lifetime", ex.ParamName);
            AssertSoleRegistrationSurvived(collection, original, "AddUnique<TService, TImplementing>");
        }

        [TestMethod]
        public void AddUnique_ServiceOnly_InvalidLifetime_ThrowsAndLeavesCollectionUnchanged()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<ServiceInvoke>();
            var original = collection.Single();

            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.AddUnique<ServiceInvoke>(InvalidLifetime));

            Assert.AreEqual("lifetime", ex.ParamName);
            AssertSoleRegistrationSurvived(collection, original, "AddUnique<TService>(lifetime)");
        }

        [TestMethod]
        public void AddUnique_Factory_InvalidLifetime_ThrowsAndLeavesCollectionUnchanged()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            var original = collection.Single();

            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.AddUnique<IServiceInvoke>(_ => new ServiceInvokeAlt(), InvalidLifetime));

            Assert.AreEqual("lifetime", ex.ParamName);
            AssertSoleRegistrationSurvived(collection, original, "AddUnique<TService>(factory, lifetime)");
        }

        [TestMethod]
        public void AddUnique_ServiceType_InvalidLifetime_ThrowsAndLeavesCollectionUnchanged()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<ServiceInvoke>();
            var original = collection.Single();

            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.AddUnique(typeof(ServiceInvoke), InvalidLifetime));

            Assert.AreEqual("lifetime", ex.ParamName);
            AssertSoleRegistrationSurvived(collection, original, "AddUnique(Type, lifetime)");
        }

        [TestMethod]
        public void ReplaceUnique_InvalidLifetime_ThrowsAndLeavesCollectionUnchanged()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            var original = collection.Single();

            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.ReplaceUnique<IServiceInvoke, ServiceInvokeAlt>(InvalidLifetime));

            Assert.AreEqual("lifetime", ex.ParamName);
            AssertSoleRegistrationSurvived(collection, original, "ReplaceUnique<TService, TImplementing>");
        }

        [TestMethod]
        public void RegisterIfNotExist_ServiceAndImplementation_InvalidLifetime_ThrowsAndLeavesCollectionUnchanged()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            var original = collection.Single();

            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.RegisterIfNotExist<IServiceInvoke, ServiceInvokeAlt>(InvalidLifetime));

            Assert.AreEqual("lifetime", ex.ParamName);
            AssertSoleRegistrationSurvived(collection, original, "RegisterIfNotExist<TService, TImplementing>");
        }

        [TestMethod]
        public void RegisterIfNotExist_ServiceOnly_InvalidLifetime_ThrowsAndLeavesCollectionUnchanged()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<ServiceInvoke>();
            var original = collection.Single();

            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.RegisterIfNotExist<ServiceInvoke>(InvalidLifetime));

            Assert.AreEqual("lifetime", ex.ParamName);
            AssertSoleRegistrationSurvived(collection, original, "RegisterIfNotExist<TService>(lifetime)");
        }

        [TestMethod]
        public void RegisterIfNotExist_Factory_InvalidLifetime_ThrowsAndLeavesCollectionUnchanged()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            var original = collection.Single();

            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.RegisterIfNotExist<IServiceInvoke>(_ => new ServiceInvokeAlt(), InvalidLifetime));

            Assert.AreEqual("lifetime", ex.ParamName);
            AssertSoleRegistrationSurvived(collection, original, "RegisterIfNotExist<TService>(factory, lifetime)");
        }

        [TestMethod]
        public void AddUnique_Factory_InvalidLifetime_IsRejectedAtRegistrationTimeNotAtBuildTime()
        {
            var collection = new ServiceCollection();

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => collection.AddUnique<IServiceInvoke>(_ => new ServiceInvokeAlt(), InvalidLifetime));

            Assert.AreEqual(0, collection.Count);
        }

        #endregion

        #region Non-assignable instance must be rejected before the collection is touched

        [TestMethod]
        public void AddUnique_Instance_NotAssignableToServiceType_ThrowsArgumentException()
        {
            var collection = new ServiceCollection();
            object unrelatedInstance = new ServiceInvokeOne();

            var ex = Assert.ThrowsException<ArgumentException>(
                () => collection.AddUnique(typeof(IServiceInvoke), unrelatedInstance));

            Assert.AreEqual("instance", ex.ParamName);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddUnique_Instance_NotAssignableToServiceType_LeavesExistingRegistrationIntact()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IServiceInvoke, ServiceInvoke>();
            var original = collection.Single();
            object unrelatedInstance = new ServiceInvokeOne();

            var ex = Assert.ThrowsException<ArgumentException>(
                () => collection.AddUnique(typeof(IServiceInvoke), unrelatedInstance));

            Assert.AreEqual("instance", ex.ParamName);
            AssertSoleRegistrationSurvived(collection, original, "AddUnique(Type, instance)");
        }

        #endregion

        #region Helpers

        private static void AssertSoleRegistrationSurvived(IServiceCollection collection,
            ServiceDescriptor original, string method)
        {
            Assert.AreEqual(1, collection.Count);

            var survivor = collection.Single();

            Assert.AreSame(original, survivor);
            Assert.AreEqual(original.ServiceType, survivor.ServiceType);
            Assert.AreEqual(original.ImplementationType, survivor.ImplementationType);
            Assert.AreEqual(original.Lifetime, survivor.Lifetime);
        }

        #endregion
    }
}
