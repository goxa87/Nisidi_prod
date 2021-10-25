using CommonServices.Infrastructure.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBTests
{
    [TestClass]
    public class StringHelperTest
    {
        [TestMethod]
        public void ToDateStringmonth_FormattedForCard()
        {
            var input = new DateTime(2021, 11, 10, 14, 45, 55);
            string output =  "10 ноября";

            var rezult = input.ToDateStringmonth(false," ", false);

            Assert.AreEqual(output, rezult);
        }

        [TestMethod]
        public void ToDateStringmonth_FormattedForCard2()
        {
            var input = new DateTime(2021, 11, 10);
            string output = "10.11.2021";

            var rezult = input.ToDateStringmonth(true, ".", true);

            Assert.AreEqual(output, rezult);
        }
    }
}
