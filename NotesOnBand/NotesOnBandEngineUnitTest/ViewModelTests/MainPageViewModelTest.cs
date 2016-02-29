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

            //Load up the pre-defiend notes.
            StorageFile testXML = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///SavedNotes/PreviousSyncedNotes.xml"));

            //Copy to the local folder.
            await testXML.CopyAsync(Windows.Storage.ApplicationData.Current.LocalFolder);

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
            //Load up the pre-defiend notes.
            StorageFile testXML = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///SavedNotes/PreviousSyncedNotes.xml"));

            //Try to guess?
            try
            {
                StorageFile oldFIle = await ApplicationData.Current.LocalFolder.GetFileAsync("PreviousSyncedNotes.xml");

                //If reach here, mean we found the damn thing.
                await oldFIle.DeleteAsync();
            }

            catch
            {
                //All good.
            }

            //Copy to the local folder.
            await testXML.CopyAsync(Windows.Storage.ApplicationData.Current.LocalFolder);

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
            StorageFile xmlStorageFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("PreviousSyncedNotes.xml");

            string xml = await FileIO.ReadTextAsync(xmlStorageFile);

            //Load up the XML and verify.
            XElement myXML = XElement.Parse(xml);

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
