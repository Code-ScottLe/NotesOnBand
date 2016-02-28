using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace NotesOnBandEngineUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void FailMeTest()
        {
            Assert.Fail("fail on purpose");
        }
    }
}
