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
        public async Task SaveToXMLAsync(List<string> notes)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> LoadFromXMLAsync()
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


            List<string> notes = new List<string>();

            foreach(XElement element in myDoc.Descendants().First().Descendants())
            {
                notes.Add(element.Value);
            }

            return notes;
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

            for(int i = 0; i < 8; i++)
            {
                //create a new element
                XElement noteElement = new XElement("Note");
                noteElement.SetAttributeValue("index", i);
                noteElement.SetValue($"Notes #{i+1}");

                //Add that to the root node
                rootElement.Add(noteElement);
            }

            //Done adding basics, return it.
            return myDocument;
        }
        #endregion

    }
}
