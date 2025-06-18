// ***********************************************************************
//  Assembly         : RzR.Shared.Services.ExtensionsTest
//  Author           : RzR
//  Created On       : 2025-06-17 23:35
// 
//  Last Modified By : RzR
//  Last Modified On : 2025-06-17 23:38
// ***********************************************************************
//  <copyright file="EnumerableExtensionsTests.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniqueServiceCollection.Extensions;

// ReSharper disable ExpressionIsAlwaysNull

#endregion

namespace ExtensionsTest.ExtensionsTest
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