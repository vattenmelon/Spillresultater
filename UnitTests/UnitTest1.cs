using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Tipperesultater.Data;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task skal_hente_ned_lotto_resultater()
        {
            var resultatdata = await WebDataSource.GetGroupAsync("lotto", false);
            Assert.IsNotNull(resultatdata);
            string[] vinnertallArray = resultatdata.Vinnertall.Split(',');
            Assert.AreEqual(7, vinnertallArray.Length);
        }

        [TestMethod]
        public async Task skal_hente_ned_vikinglotto_resultater()
        {
            var resultatdata = await WebDataSource.GetGroupAsync("vikinglotto", false);
            Assert.IsNotNull(resultatdata);
            string[] vinnertallArray = resultatdata.Vinnertall.Split(',');
            Assert.AreEqual(6, vinnertallArray.Length);
        }
    }
}
