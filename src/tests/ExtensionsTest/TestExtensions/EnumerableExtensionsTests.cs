#region U S A G E S

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzR.Extensions.UniqueServiceCollection.Extensions;

#endregion

namespace ExtensionsTest.TestExtensions
{
    [TestClass]
    public class EnumerableExtensionsTests
    {
        [TestMethod]
        public void HasAny_Test_Should_Pass()
        {
            var list = new List<int> { 1 };
            var any = list.HasAnyInCollection();

            Assert.IsNotNull(list);
            Assert.IsNotNull(any);
            Assert.IsTrue(any);
        }

        [TestMethod]
        public void HasAny_Null_Test_Should_Pass()
        {
            List<int> list = null;
            var any = list.HasAnyInCollection();

            Assert.IsNull(list);
            Assert.IsNotNull(any);
            Assert.IsFalse(any);
        }

        [TestMethod]
        public void HasAny_Empty_Test_Should_Pass()
        {
            var list = new List<int>();
            var any = list.HasAnyInCollection();

            Assert.IsNotNull(list);
            Assert.IsNotNull(any);
            Assert.IsFalse(any);
        }

        [TestMethod]
        public void HasNoAny_Empty_Test_Should_Pass()
        {
            var list = new List<int>();
            var any = list.HasNoAnyInCollection();

            Assert.IsNotNull(list);
            Assert.IsNotNull(any);
            Assert.IsTrue(any);
        }

        [TestMethod]
        public void HasNoAny_WithData_Test_Should_Pass()
        {
            var list = new List<int>() { 1 };
            var any = list.HasNoAnyInCollection();

            Assert.IsNotNull(list);
            Assert.IsNotNull(any);
            Assert.IsFalse(any);
        }

        [TestMethod]
        public void HasNoAny_Null_Test_Should_Pass()
        {
            List<int> list = null;
            var any = list.HasNoAnyInCollection();

            Assert.IsNull(list);
            Assert.IsNotNull(any);
            Assert.IsTrue(any);
        }
    }
}
