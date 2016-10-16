using System;
using System.IO;
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
            decimal i = sv.SolarIntensity(40, -105);
            Assert.AreEqual(6.06m, i);

        }
        [TestMethod]
        public void TestSunshineNoData()
        {
            Project3.Service1 sv = new Project3.Service1();
            decimal i = sv.SolarIntensity(40, 50);
            Assert.AreEqual(-1, i);

        }

        [TestMethod]
        public void TestWords()
        {
            Project3.Service1 sv = new Project3.Service1();
            Assert.AreEqual("&<", sv.WordFilter("<html><head></head><body>&amp;&lt;</body></html>"));
            Assert.AreEqual("", sv.WordFilter("<html><head></head><body>is</body></html>"));
            Assert.AreEqual("isare", sv.WordFilter("<html><head></head><body>isare</body></html>"));
            Assert.AreEqual("", sv.WordFilter("<html><head></head><script>isare</script></html>"));
            Assert.AreEqual("", sv.WordFilter("<html><head></head><!--test--></html>"));
            Assert.AreEqual("test2", sv.WordFilter("<html><head></head><style>test1</style>test2</html>"));
            Assert.AreEqual("fish taco", sv.WordFilter("<html><head></head><p>fish</p><div>taco</div></html>"));
            Assert.AreEqual("one < 2", sv.WordFilter("one is < 2"));
        }

        [TestMethod]
        public void TestHtml()
        {
            Project3.Service1 sv = new Project3.Service1();
            string text = File.ReadAllText(@"..\..\full.htm", System.Text.Encoding.UTF8);
            string actual = sv.WordFilter(text);
            File.WriteAllText("actual.txt", actual);
            string expected = File.ReadAllText(@"..\..\fullresult.txt", System.Text.Encoding.UTF8);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGit()
        {
            Project3.Service1 sv = new Project3.Service1();
            string[] actual = sv.EcoFriendlySoftware(5);
            Assert.AreEqual(5, actual.Length);
        }

        [TestMethod]
        public void TestAmazon()
        {
            Project3.Service1 sv = new Project3.Service1();
            string[] actual = sv.EcoFriendlyProducts(5);
        }

    }
}
