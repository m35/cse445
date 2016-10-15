using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSunshineHasData()
        {
            Project3.Service1 sv = new Project3.Service1();
            decimal i = sv.AnnualAverageSunshineIndex(40, -105);
            Assert.AreEqual(6.06, i);

        }
        [TestMethod]
        public void TestSunshineNoData()
        {
            Project3.Service1 sv = new Project3.Service1();
            decimal i = sv.AnnualAverageSunshineIndex(40, 50);
            Assert.AreEqual(-1, i);

        }

        [TestMethod]
        public void TestWords()
        {
            Project3.Service1 sv = new Project3.Service1();
            Assert.AreEqual("&", sv.WordFilter("<html><head></head><body>&amp;</body></html>"));
            Assert.AreEqual("", sv.WordFilter("<html><head></head><body>is</body></html>"));
            Assert.AreEqual("isare", sv.WordFilter("<html><head></head><body>isare</body></html>"));
            Assert.AreEqual("", sv.WordFilter("<html><head></head><script>isare</script></html>"));
        }

    }
}
