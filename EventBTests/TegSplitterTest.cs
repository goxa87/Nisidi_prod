using Microsoft.VisualStudio.TestTools.UnitTesting;
using EventB.Services;
using System.Linq;

namespace EventBTests
{
    [TestClass]
    public class TegSplitter_Test
    {
        [TestMethod]
        public void GetEnumerable_normal_string()
        {
            var input = "aaa bbb ccc";
            string[] output = { "AAA", "BBB", "CCC" };

            var splitter = new TegSplitter();
            var rezult = splitter.GetEnumerable(input);

            Assert.AreEqual(output.Length, rezult.Count);
        }

        [TestMethod]
        public void GetEnumerable_withEmpties()
        {
            var input = "aaa .,bbb/+ccc";
            string[] output = { "AAA", "BBBCCC" };

            var splitter = new TegSplitter();
            var rezult = splitter.GetEnumerable(input);

            Assert.AreEqual(output.Length, rezult.Count);
        }

        [TestMethod]
        public void GetEnumerable_void()
        {
            var input = "";
            string[] output = {  };

            var splitter = new TegSplitter();
            var rezult = splitter.GetEnumerable(input);

            Assert.AreEqual(null, rezult);
        }
        [TestMethod]
        public void GetEnumerable_separatorsOnly()
        {
            var input = "  ..!?///+=*-@";
            string[] output = { };

            var splitter = new TegSplitter();
            var rezult = splitter.GetEnumerable(input);

            Assert.AreEqual(0, rezult.Count);
        }

        [TestMethod]
        public void GetEnumerable_withUpper_toLower()
        {
            var input = " aaa BBB";
            string[] output = { };

            var splitter = new TegSplitter();
            var rezult = splitter.GetEnumerable(input);

            Assert.AreEqual("BBB", rezult[1]);
        }


        [TestMethod]
        public void GetEnumerable_withUpper_toLowerAndRepeat()
        {
            var input = " aaa BBB aaa";
            string[] output = { };

            var splitter = new TegSplitter();
            var rezult = splitter.GetEnumerable(input);

            Assert.AreEqual(2, rezult.Count);
        }

        [TestMethod]
        public void GetEnumerable_withUpper_toLowerAndRepeat2()
        {
            var input = " aaa BBB@aaa";
            string[] output = { "AAA", "BBB"};

            var splitter = new TegSplitter();
            var rezult = splitter.GetEnumerable(input);

            Assert.AreEqual(output.Length, rezult.Count);
        }

        [TestMethod]
        public void GetEnumerable_withUpper_toLowerAndRepeat3()
        {
            var input = " aaa BBB @aaaBBB";
            string[] output = { "AAA", "BBB", "aaaBBB" };

            var splitter = new TegSplitter();
            var rezult = splitter.GetEnumerable(input);

            Assert.AreEqual(output.Length, rezult.Count);
        }

        [TestMethod]
        public void GetEnumerable_withUpper_toLowerAndRepeat4()
        {
            var input = " aaa BBB @aaaBBB";
            string[] output = { "AAA", "BBB", "AAABBB" };

            var splitter = new TegSplitter();
            var rezult = splitter.GetEnumerable(input);
            
            if(rezult.Count != output.Count())
            {
                Assert.Fail();
            }

            foreach(var e in rezult)
            {
                if(!output.Any(x => e == x))
                {
                    Assert.Fail();
                }
            }

            foreach (var e in output)
            {
                if (!rezult.Any(x => e == x))
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void GetEnumerable_withUpper_toLowerAndRepeat5()
        {
            var input = " aaa BBB @aaa?/'__123BBB";
            string[] output = { "AAA", "BBB", "AAA123BBB" };

            var splitter = new TegSplitter();
            var rezult = splitter.GetEnumerable(input);

            if (rezult.Count != output.Count())
            {
                Assert.Fail();
            }

            foreach (var e in rezult)
            {
                if (!output.Any(x => e == x))
                {
                    Assert.Fail();
                }
            }

            foreach (var e in output)
            {
                if (!rezult.Any(x => e == x))
                {
                    Assert.Fail();
                }
            }
        }
    }
}
