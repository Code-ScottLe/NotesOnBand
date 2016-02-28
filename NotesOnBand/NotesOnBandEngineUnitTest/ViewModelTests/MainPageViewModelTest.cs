using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using NotesOnBandEngine.ViewModels;

namespace NotesOnBandEngineUnitTest.ViewModelTests
{
    [TestClass]
    public class MainPageViewModelTest
    {

        [TestMethod]
        public void FailOnPurpose_Test()
        {
            Assert.Fail("Fail me 2");
        }

        [TestMethod]
        public async Task LoadNotesFromXMLTest_Success()
        {
            //Create the instance.
            MainPageViewModel viewModel = new MainPageViewModel();

            //Load.
            await viewModel.LoadNotesFromXML();

            //Check.

            Assert.AreEqual(viewModel.Note1, "Test Note 1");
        }
    }
}
