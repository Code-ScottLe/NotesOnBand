using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using NotesOnBandEngine.ViewModels;

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

        [TestMethod]
        public void InitTest_Success()
        {
            var viewModel = new MainPageViewModel();

            Assert.IsTrue(true);
        }
    }
}
