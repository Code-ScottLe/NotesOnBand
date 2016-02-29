using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using NotesOnBandEngine.ViewModels;
using NotesOnBandEngine.Models;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Xml;
using System.Xml.Linq;

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
            Assert.AreEqual(viewModel.Note2, "Test Note 2");
            Assert.AreEqual(viewModel.Note3, "Test Note 3");
            Assert.AreEqual(viewModel.Note4, "Test Note 4");
            Assert.AreEqual(viewModel.Note5, "Test Note 5");
            Assert.AreEqual(viewModel.Note6, "Test Note 6");
            Assert.AreEqual(viewModel.Note7, "Test Note 7");
            Assert.AreEqual(viewModel.Note8, "Test Note 8");
        }

        [TestMethod]
        public async Task SaveNotesToXMLTest_Success()
        {
            //Create the instance. 
            MainPageViewModel viewModel = new MainPageViewModel();

            //Load
            await viewModel.LoadNotesFromXML();

            //Make changes.
            viewModel.Note1 = "This is a new Note 1";

            viewModel.Note3 = "Hello World";

            viewModel.Note5 = "I love you to the moon and back";

            //saves.
            await viewModel.SaveNotesToXML();

            //Load up the thingy.
            StorageFile xmlStorageFile = await AppFileHandler.Instance.GetFileFromApplicationUrl("SavedNotes/PreviousSyncedNotes.xml");

            var readStream = await AppFileHandler.Instance.GetReadStreamToFile(xmlStorageFile);

            //Load up the XML and verify.
            XElement myXML = System.Xml.Linq.XElement.Load(readStream);

            XElement note1 = (from element in myXML.Descendants("Note")
                                 where (int)element.Attribute("index") == 0
                                 select element).First();

            XElement note3 = (from element in myXML.Descendants("Note")
                              where (int)element.Attribute("index") == 2
                              select element).First();

            XElement note5 = (from element in myXML.Descendants("Note")
                              where (int)element.Attribute("index") == 4
                              select element).First();

            //Checking

            Assert.AreEqual(note1.Value.Trim(), "This is a new Note 1");

            Assert.AreEqual(note3.Value.Trim(), "Hello World");

            Assert.AreEqual(note5.Value.Trim(), "I love you to the moon and back");


        }
    }
}
