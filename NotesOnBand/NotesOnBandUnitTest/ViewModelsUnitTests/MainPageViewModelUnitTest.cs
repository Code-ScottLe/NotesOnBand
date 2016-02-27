using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using NotesOnBand.ViewModels;

namespace NotesOnBandUnitTest.ViewModelsUnitTests
{
    [TestClass]
    public class MainPageViewModelUnitTest
    {
        [TestMethod]
        public async Task LoadNotesFromXMLUnitTest()
        {
            //Create the instance of the view model

            MainPageViewModel viewModel = new MainPageViewModel();

            //Try to load.
            await viewModel.LoadNotesFromXML();

            //Test.
            Assert.AreEqual(viewModel.Note1, "Test Note 1");
        }
    }
}
