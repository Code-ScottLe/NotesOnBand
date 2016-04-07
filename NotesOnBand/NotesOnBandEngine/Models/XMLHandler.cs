using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NotesOnBandEngine.Models
{
    public class XMLHandler
    {
        #region Fields
        private static XMLHandler currentInstance;


        /// <summary>
        /// The XML Document that hold all the data 
        /// </summary>
        private XDocument syncedNotesXDocument;


        private string savedFileName;

        #endregion

        #region Properties

        /// <summary>
        /// Get the singleton instance
        /// </summary>
        public static XMLHandler Instance
        {
            get
            {
                if (currentInstance == null)
                {
                    currentInstance = new XMLHandler();
                }

                return currentInstance;
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor, hidden
        /// </summary>
        protected XMLHandler()
        {
            syncedNotesXDocument = new XDocument();
            savedFileName = "NotesOnBandSyncedNotes.xml";
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        public async Task SaveToXMLAsync(List<BandNote> notes)
        {
            //Get the XDoc from List
            XDocument myDoc = CreateNewXML(notes);

            //Trying to load from local folder
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile myXMLStorageFile = await localFolder.TryGetItemAsync(savedFileName) as Windows.Storage.StorageFile;
          
            if (myXMLStorageFile == null)
            {
                //not exists. Create them.
                myXMLStorageFile = await localFolder.CreateFileAsync(savedFileName);
            }

            //get xml without any formatting
            string xml = myDoc.ToString(SaveOptions.DisableFormatting);

            await Windows.Storage.FileIO.WriteTextAsync(myXMLStorageFile, xml);


        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<BandNote>> LoadFromXMLAsync()
        {
            //Trying to load from local folder
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile myXMLStorageFile = await localFolder.TryGetItemAsync(savedFileName) as Windows.Storage.StorageFile;

            XDocument myDoc = null;

            if (myXMLStorageFile == null)
            {
                //not exists. Create them.
                myDoc = CreateNewXML();
            }

            else
            {
                //File exist, load it.
                string xml = await Windows.Storage.FileIO.ReadTextAsync(myXMLStorageFile);
                myDoc = XDocument.Parse(xml);
            }

            try
            {
                List<BandNote> notes = new List<BandNote>();

                foreach (XElement element in myDoc.Descendants().First().Descendants())
                {
                    int noteNum = 0;
                    int.TryParse(element.FirstAttribute.Value, out noteNum);
                    notes.Add(new BandNote() { Content = element.Value, Title = $"Note #{noteNum + 1}" });
                }

                return notes;
            }

            catch
            {
                //Remove the old file as we have some corruption.
                await myXMLStorageFile.DeleteAsync();
                return new List<BandNote>();
            }
            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        private XDocument CreateNewXML(List<BandNote> notes)
        {
            XDocument myDocument = new XDocument();

            //Add the root notes.
            myDocument.Add(new XElement("Notes"));

            //get the root element.
            var rootElement = myDocument.Descendants().First();

            for (int i = 0; i < notes.Count; i++)
            {
                //create a new element
                XElement noteElement = new XElement("Note");
                noteElement.SetAttributeValue("index", i);
                noteElement.SetValue(notes[i].Content);

                //Add that to the root node
                rootElement.Add(noteElement);
            }

            //Done adding basics, return it.
            return myDocument;
        }


        /// <summary>
        /// Create new XDocument that can be saved as new XML for storing notes. 
        /// </summary>
        /// <returns></returns>
        private XDocument CreateNewXML()
        {
            XDocument myDocument = new XDocument();

            //Add the root notes.
            myDocument.Add(new XElement("Notes"));

            //get the root element.
            var rootElement = myDocument.Descendants().First();

            //for(int i = 0; i < 8; i++)
            //{
            //    //create a new element
            //    XElement noteElement = new XElement("Note");
            //    noteElement.SetAttributeValue("index", i);
            //    noteElement.SetValue($"Notes #{i+1}");

            //    //Add that to the root node
            //    rootElement.Add(noteElement);
            //}

            //Done adding basics, return it.
            return myDocument;
        }
        #endregion

    }
}
