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
            string[] output = { "aaa", "bbb", "ccc" };

            var splitter = new TegSplitter();
            var rezult = splitter.GetEnumerable(input);

            Assert.AreEqual(output.Length, rezult.Count);
        }

        [TestMethod]
        public void GetEnumerable_withEmpties()
        {
            var input = "aaa .,bbb/+ccc";
            string[] output = { "aaa", "bbb", "ccc" };

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
    }
}
