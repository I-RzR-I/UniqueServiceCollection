// ***********************************************************************
//  Assembly         : RzR.Shared.Services.ExtensionsTest
//  Author           : RzR
//  Created On       : 2025-06-17 17:57
// 
//  Last Modified By : RzR
//  Last Modified On : 2025-06-17 18:04
// ***********************************************************************
//  <copyright file="ObjectExtensionTests.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniqueServiceCollection.Extensions;

#endregion

namespace ExtensionsTest.ExtensionsTest
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