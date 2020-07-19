using Microsoft.VisualStudio.TestTools.UnitTesting;
using EventB.Services;

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
            string[] output = { "AAA", "BBB", "CCC" };

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
    }
}
