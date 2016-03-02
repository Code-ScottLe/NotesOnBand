using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using NotesOnBandEngine.Models;
using NotesOnBandEngine.Events;
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
        private string saveFileName = "PreviousSyncedNotes.xml";
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

        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
                OnPropertyChanged("Message");
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
                notesList.Add("Note #" + (i+1).ToString());
            }

            //Initalize Band
            currentBand = new Band();

        }

        #endregion

        #region Methods

        /// <summary>
        /// Sync the notes to Band
        /// </summary>
        /// <param name="notesCount"> number of notes to sync</param>
        /// <returns></returns>
        public async Task SyncNotesToBandAsync(int notesCount)
        {
            //Saves the notes first.
            //await SaveNotesToXML();

            //Reverse List.
            List<string> actualNote = NotesListHandler(notesCount);

            //Done adding reverse stuffs.
            //Connect to Band.
            bool connectStatus = false;

            await CurrentBand.ConnectToBandAsync();

            if(connectStatus == false)
            {
                //something is wrong.
                
                return;
            }

            await CurrentBand.SyncToBandAsync(actualNote);
               
        }
        
        /// <summary>
        ///    Handling the note list for the band. It reverse the original note list due to the nature of the band displaying stuffs
        ///    in backward order.
        /// </summary>
        /// <returns></returns>
        private List<string> NotesListHandler(int notesCount)
        {
            List<string> actualNote = new List<string>();
            //Reorder the list, as the Band sync them in the reverse order.

            for (int i = notesCount - 1; i >= 0; i --)
            {
                actualNote.Add(notesList[i]);
            }


            //for (int i = notesList.Count - 1; i >= 0; i--)
            //{
            //    if (string.IsNullOrEmpty(notesList[i]) == false)
            //    {
            //        if (notesList[i].Contains("Note #") == false)
            //        {
            //            actualNote.Add(notesList[i]);
            //        }

            //        else if (notesList[i].Trim().Count() > 7)
            //        {
            //            actualNote.Add(notesList[i]);
            //        }
            //    }
            //}

            return actualNote;
        }

        /// <summary>
        /// Load up previously synced notes to the band asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task LoadNotesFromXML()
        {
            //Open up the in-app XML Documents that we saves all the notes.
            StorageFile savedNotesXMLStorageFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(saveFileName);

            //Load up the stream
            string xml = await FileIO.ReadTextAsync(savedNotesXMLStorageFile);

            //Load it up as XElement (LINQ with XML)
            previousSyncedNotes = XElement.Parse(xml);

            //check if we actually loaded it up successfully
            if (previousSyncedNotes == null)
            {
                
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
                savedNotesXMLStorageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(saveFileName);
            }
            
            catch (System.IO.FileNotFoundException e)
            {
                //Not found/not created file.
                savedNotesXMLStorageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(saveFileName);
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
