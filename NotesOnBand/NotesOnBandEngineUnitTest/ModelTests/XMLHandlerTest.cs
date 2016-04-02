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

namespace NotesOnBandEngineUnitTest.ModelTests
{
    [TestClass]
    public class XMLHandlerTest
    {
        [TestMethod]
        public void LoadTest()
        {
            var instance = XMLHandler.Instance;

            var returned = instance.LoadFromXMLAsync().Result;

            Assert.AreEqual(returned.Count, 8);
        }

        [TestMethod]
        public void SaveTest()
        {
            var instance = XMLHandler.Instance;

            List<string> notes = new List<string>() { "What the hell", "this sucks", "gives me some good stuffs", "Nope", "Dope", "Yep", "Bruh", "Dep" };

            instance.SaveToXMLAsync(notes).Wait();

            var lol = instance.LoadFromXMLAsync().Result;

            for(int i = 0; i < lol.Count; i++)
            {
                Assert.AreEqual(notes[i], lol[i]);
            }
        }
    }
}
