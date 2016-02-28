using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;
using NotesOnBandEngine.ViewModels;

namespace NotesOnBandUnitTest.ViewModelsUnitTests
{
    [TestClass]
    public class MainPageViewModelUnitTest
    {
        public async Task LoadNotesFromXMLTest_Success()
        {
            //crate a viewModel.
            MainPageViewModel myView = new MainPageViewModel();

            await myView.LoadNotesFromXML();

            //Check.
            Assert.AreEqual(myView.Note1, "Test Note 1");
        }
    }
}
