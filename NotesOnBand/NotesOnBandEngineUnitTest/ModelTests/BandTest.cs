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
    public class BandTest
    {
        [TestMethod]
        public void BandInitTest_Success()
        {
            Band myBand = new Band();

            Assert.IsNotNull(myBand);
        }


        [TestMethod]
        public async Task ConnectToBandAsyncTest_Success()
        {
            Band myBand = new Band();
            
            bool status = await myBand.ConnectToBandAsync();

            Assert.IsTrue(status);
        }
    
    }
}
