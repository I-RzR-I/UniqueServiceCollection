#region U S A G E S

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzR.Extensions.UniqueServiceCollection.Extensions;

#endregion

namespace ExtensionsTest.TestExtensions
{
    [TestClass]
    public class ObjectExtensionTests
    {
        [DataRow(null)]
        [TestMethod]
        public void IsNull_Test_Should_Pass(object objValue)
        {
            var isTrue = objValue.IsNull();

            Assert.IsTrue(isTrue);
        }

        [DataRow(10)]
        [DataRow("10")]
        [TestMethod]
        public void IsNotNull_Test_Should_Pass(object objValue)
        {
            var isTrue = objValue.IsNotNull();

            Assert.IsTrue(isTrue);
        }
    }
}
