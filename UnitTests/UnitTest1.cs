﻿using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Tipperesultater.Data;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var sampleDataGroup = WebDataSource.GetGroupAsync("Group-1", false);
            Assert.IsNotNull(sampleDataGroup);
        }
    }
}