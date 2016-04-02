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
        public async void LoadTest()
        {
            var instance = XMLHandler.Instance;

            var returned = await instance.LoadFromXMLAsync();

            Assert.Equals(returned.Count, 8);
        }
    }
}
