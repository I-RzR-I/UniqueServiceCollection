#if !NET8_0_OR_GREATER

#region U S A G E S

using System;
using ExtensionsTest.Modules.Abstractions;
using ExtensionsTest.Modules.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzR.Extensions.UniqueServiceCollection.ServiceCollectionExtensions;

#endregion

namespace ExtensionsTest.TestServiceCollectionExtensions
{
    [TestClass]
    public class AddUniqueKeyedDownlevelTests
    {
        [TestMethod]
        public void AddUniqueKeyed_OnUnsupportedRuntime_ThrowsPlatformNotSupportedException()
        {
            var collection = new ServiceCollection();

            var ex = Assert.ThrowsException<PlatformNotSupportedException>(
                () => collection.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA"));

            StringAssert.Contains(ex.Message, "8.0.0",
                "The message must name the version the caller needs.");
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void TryAddUniqueKeyed_OnUnsupportedRuntime_ThrowsPlatformNotSupportedException()
        {
            var collection = new ServiceCollection();

            Assert.ThrowsException<PlatformNotSupportedException>(
                () => collection.TryAddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA"));
        }

        [TestMethod]
        public void AddUniqueKeyed_NullCollection_StillThrowsArgumentNullExceptionFirst()
        {
            IServiceCollection collection = null;

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => collection.AddUniqueKeyed<IServiceInvoke, ServiceInvoke>("tenantA"));

            Assert.AreEqual("serviceCollection", ex.ParamName);
        }

        [TestMethod]
        public void NonKeyedSurface_IsUnaffectedOnUnsupportedRuntime()
        {
            var collection = new ServiceCollection();

            Assert.IsTrue(collection.TryAddUnique<IServiceInvoke, ServiceInvoke>());
            collection.AddUnique<IServiceInvokeOne, ServiceInvokeOne>();

            Assert.AreEqual(2, collection.Count);
        }
    }
}

#endif