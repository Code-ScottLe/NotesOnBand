using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using NotesOnBandEngine.Models;
using Windows.Storage.Streams;
using Windows.Storage;

namespace NotesOnBandEngine.ViewModels
{
    /// <summary>
    /// ViewModel for the MainPage (MainPage.xaml)
    /// </summary>
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region Fields

        private Band currentBand;
        private List<string> notesList;
        private XElement previousSyncedNotes;
        private string errorMessage;
        private string message;
        #endregion

        #region events

        /// <summary>
        /// Implement the INotifyPropertyChanged Interface. Use this to notify the View about the property that was changed to perform updates.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        #endregion

        #region Properties

        public Band CurrentBand
        {
            get
            {
                return currentBand;
            }

            protected set
            {
                currentBand = value;
                OnPropertyChanged("CurrentBand");
            }
        }

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

        #region Notes Properties
        public string Note1
        {
            get
            {
                return notesList[0];
            }

            set
            {
                notesList[0] = value;
                OnPropertyChanged("Note1");

            }
        }

        public string Note2
        {
            get
            {
                return notesList[1];
            }

            set
            {
                notesList[1] = value;
                OnPropertyChanged("Note2");

            }
        }

        public string Note3
        {
            get
            {
                return notesList[2];
            }

            set
            {
                notesList[2] = value;
                OnPropertyChanged("Note3");

            }
        }

        public string Note4
        {
            get
            {
                return notesList[3];
            }

            set
            {
                notesList[3] = value;
                OnPropertyChanged("Note4");

            }
        }

        public string Note5
        {
            get
            {
                return notesList[4];
            }

            set
            {
                notesList[4] = value;
                OnPropertyChanged("Note5");

            }
        }

        public string Note6
        {
            get
            {
                return notesList[5];
            }

            set
            {
                notesList[5] = value;
                OnPropertyChanged("Note6");

            }
        }

        public string Note7
        {
            get
            {
                return notesList[6];
            }

            set
            {
                notesList[6] = value;
                OnPropertyChanged("Note7");

            }
        }

        public string Note8
        {
            get
            {
                return notesList[7];
            }

            set
            {
                notesList[7] = value;
                OnPropertyChanged("Note8");

            }
        }
        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor of the MainPageViewModel
        /// </summary>
        public MainPageViewModel()
        {
            //Initialze List.
            notesList = new List<string>();

            //Because the Band can store up to 8 individual pages only. We set the list of strings to have only 8 values.
            for (int i = 0; i < 8; i++)
            {
                notesList.Add(string.Empty);
            }

            //Open up the XML document to load back on the previously saved notes.

        }

        #endregion

        #region Methods

        /// <summary>
        /// Load up previously synced notes to the band asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task LoadNotesFromXML()
        {
            //Open up the in-app XML Documents that we saves all the notes.
            StorageFile savedNotesXMLStorageFile = await AppFileHandler.Instance.GetFileFromLocalFolder("PreviousSyncedNotes.xml");
          
            //Load up the stream
            System.IO.Stream actualFileStream = await AppFileHandler.Instance.GetReadStreamFromStorageFile(savedNotesXMLStorageFile);

            //Load it up as XElement (LINQ with XML)
            previousSyncedNotes = XElement.Load(actualFileStream);                                
          

            //check if we actually loaded it up successfully
            if (previousSyncedNotes == null)
            {
                ErrorMessage = "Can't load up previous synced notes!";
                return;
            }

            //Populate back the List of messages.
            foreach (XElement element in previousSyncedNotes.Descendants("Note"))
            {
                notesList[(int)element.Attribute("index")] = element.Value.Trim();

                //Because we are saving that to the list itself, not through the Property, have to manually ring the event.
                //Index in XML and list are 0 based,  have to add 1 first.
                string propertyName = "Note" + ((int)element.Attribute("index") + 1).ToString();

                //Manually ring the event
                OnPropertyChanged(propertyName);
            }


        }

        /// <summary>
        /// Save the current notes in the note list to the PreviousSyncedNotes.xml just in case. This will be called everytime the user syncs the note to the band
        /// </summary>
        /// <returns></returns>
        public async Task SaveNotesToXML()
        {

            //Get the list of notes and put it back to the XElement.
            for (int i = 0; i < notesList.Count; i++)
            {
                XElement currentNode = previousSyncedNotes.Descendants("Note").Where(n => (int)n.Attribute("index") == (i)).Select(n => n).FirstOrDefault();
                currentNode.SetValue(notesList[i]);
            }


            //Open up the in-app XML Documents that we saves all the notes.
            //StorageFile savedNotesXMLStorageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///SavedNotes/PreviousSyncedNotes.xml"));

            //Get the current folder.
            StorageFile savedNotesXMLStorageFile = null;

            try
            {
                //Try to get the file.
                savedNotesXMLStorageFile = await AppFileHandler.Instance.GetFileFromLocalFolder("PreviousSyncedNotes.xml");
            }
            
            catch (System.IO.FileNotFoundException e)
            {
                //Not found/not created file.
                savedNotesXMLStorageFile = await AppFileHandler.Instance.CreateFileFromLocalFolder("PreviousSyncedNotes.xml");
            }

            //write.

            string xml = previousSyncedNotes.ToString();
            await FileIO.WriteTextAsync(savedNotesXMLStorageFile, xml);
                     

        }


        /// <summary>
        /// Fire up the PropertyChanged event and notify all the listener about the changed property.
        /// </summary>
        /// <param name="propertyName">Name of the property that was changed.</param>
        public void OnPropertyChanged(string propertyName)
        {

            //Make sure we do have a listener.
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        #endregion

    }
}
