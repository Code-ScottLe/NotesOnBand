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
using System.Collections.ObjectModel;

namespace NotesOnBandEngine.ViewModels
{
    /// <summary>
    /// ViewModel for the MainPage (MainPage.xaml)
    /// </summary>
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region Fields
        private BandConnector connector;
        private ObservableCollection<BandNote> notes;
        private BandVersion currentBandVersion;
        private string uniqueIDString = "b40d28db-a774-4b6f-a97a-76272146a174";
        private double completionPercentage = 0.0;
        private string completionStatus;
        #endregion

        #region events

        /// <summary>
        /// Implement the INotifyPropertyChanged Interface. Use this to notify the View about the property that was changed to perform updates.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Get the List of notes.
        /// </summary>
        public ObservableCollection<BandNote> Notes
        {
            get
            {
                return this.notes;
            }
        }
        
        public BandVersion CurrentBandVersion
        {
            get
            {
                return currentBandVersion;
            }

            set
            {
                currentBandVersion = value;
                OnPropertyChanged("CurrentBandVersion");
            }
        }

        public double CompletionPercentage
        {
            get
            {
                return completionPercentage;
            }

            set
            {
                completionPercentage = value;
                OnPropertyChanged("CompletionPercentage");
            }
        }

        public string CompletionStatus
        {
            get
            {
                return completionStatus;
            }

            set
            {
                completionStatus = value;
                OnPropertyChanged("CompletionStatus");
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor of the MainPageViewModel
        /// </summary>
        public MainPageViewModel()
        {
            notes = new ObservableCollection<BandNote>();
            connector = new BandConnector();
            completionStatus = string.Empty;

        }

        #endregion

        #region Methods         

        /// <summary>
        /// Sync the given notes to the Band asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task SyncNotesToBandAsync()
        {
            //Starting.
            CompletionPercentage = 0;
            //Check if we have notes to sync
            if(Notes.Count == 0)
            {
                //Empty note. Just remove the Tile.
                await connector.RemoveTileFromBandAsync(new Guid(uniqueIDString));

                //write empty XML.
                await XMLHandler.Instance.SaveToXMLAsync(Notes.ConvertToList());

                return;
            }

            //We have notes to sync.
            CompletionPercentage = 5;

            //Save it to XML
            Task t = Task.Run(async () => await XMLHandler.Instance.SaveToXMLAsync(Notes.ConvertToList()));

            //Make the BandTile
            CompletionStatus = "Generating Band Tile...";
            var myBandTile = await BandNoteTileGenerator.Instance.GenerateBandTileAsync(CurrentBandVersion);
            CompletionPercentage = 30;


            //Sync the band tile.
            CompletionStatus = "Syncing...";
           

            //Connect to Band
            await connector.ConnectToBandAsync();
            CompletionPercentage = 50;

            //Try to add Tile
            await connector.AddTileToBandAsync(myBandTile);
            CompletionPercentage = 75;

            //Generate the data from the notes.
            var listy = Notes.ConvertToList();
            listy.Reverse();

            //Generate the data.
            var pagesData = BandNoteTileGenerator.Instance.GenerateDataFromNotes(listy);

            //Sync the data over.
            await connector.SyncDataToBandAsync(myBandTile, pagesData);
            CompletionPercentage = 95;

            //After we are done. Wait for the t if it hasn't done already
            await t;
            CompletionPercentage = 100;
            CompletionStatus = "Done.";

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task LoadPreviousSyncedNotesAsync()
        {
            //Try to load.
            CompletionStatus = "Loading Previous Notes...";
            CompletionPercentage = 0;

            var previousSyncedNotes = await XMLHandler.Instance.LoadFromXMLAsync();

            //Half way.
            CompletionPercentage = 50;

            //And save that to the list.
            foreach(var item in previousSyncedNotes)
            {
                Notes.Add(item);
            }

            //Done
            CompletionPercentage = 100;
            CompletionStatus = "Done.";
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
