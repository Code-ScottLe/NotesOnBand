﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;
using NotesOnBand.Models;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage.Streams;

namespace NotesOnBand.ViewModels
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
            //Because the Band can store up to 8 individual pages only. We set the list of strings to have only 8 values.
            for(int i = 0; i < 8; i++)
            {
                notesList.Add(string.Empty);
            }

            //Open up the XML document to load back on the previously saved notes.

        }

        #endregion

        #region Methods

        /// <summary>
        /// Load up previously synced notes to the band.
        /// </summary>
        /// <returns></returns>
        private async Task LoadPreviousSyncedNotes()
        {
            //Open up the in-app XML Documents that we saves all the notes.
            Windows.Storage.StorageFile savedNotesXMLStorageFile = 
                await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///SavedNotes/PreviousSyncedNotes.xml"));
            
            

            //Open up for both read and write.
            using (Windows.Storage.Streams.IRandomAccessStream fileStream = await savedNotesXMLStorageFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
            {
                //Get an input stream from the RandomAccessStream
                var readStream = fileStream.GetInputStreamAt(0);

                //Using a data reader to read off an input stream
                using (DataReader reader = new DataReader(readStream))
                {
                    //Read the entire stream
                    var fileSize = await reader.LoadAsync((uint)fileStream.Size);

                    //Ask the reader to parse the stream and return it as string, or XML in string form.
                    string xml = reader.ReadString(fileSize);

                    //Load it up as XElement (LINQ with XML)
                    previousSyncedNotes = XElement.Parse(xml);                  
                }
                
            }


            //check if we actually loaded it up successfully
            if(previousSyncedNotes == null)
            {
                ErrorMessage = "Can't load up previous synced notes!";
                return;
            }
           
            //Populate back the List of messages.
            foreach(XElement element in previousSyncedNotes.Descendants("note"))
            {
                notesList[(int)element.Attribute("index")] = element.Value;
            }
            
            
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
