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

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor of the MainPageViewModel
        /// </summary>
        public MainPageViewModel()
        {
            notes = new ObservableCollection<BandNote>();
            connector = new BandConnector();

        }

        #endregion

        #region Methods         

        /// <summary>
        /// Sync the given notes to the Band asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task SyncNotesToBandAsync()
        {
            //Check if we have notes to sync
            if(Notes.Count == 0)
            {
                //Empty note. Just remove the Tile.
                await connector.RemoveTileFromBandAsync(new Guid(uniqueIDString));

                //write empty XML.
                await XMLHandler.Instance.SaveToXMLAsync(Notes.ConvertToList());

                return;
            }


            //Save it to XML
            Task t = Task.Run(async () => await XMLHandler.Instance.SaveToXMLAsync(Notes.ConvertToList()));

            //If we reach here. Then we have some notes to sync
            //Make the BandTile
            var myBandTile = await BandNoteTileGenerator.Instance.GenerateBandTileAsync(CurrentBandVersion);

            //Sync the band tile.
            await connector.ConnectToBandAsync();
            await connector.AddTileToBandAsync(myBandTile);

            //Generate the data from the notes.
            var listy = Notes.ConvertToList();
            listy.Reverse();

            //Generate the data.
            var pagesData = BandNoteTileGenerator.Instance.GenerateDataFromNotes(listy);

            //Sync the data over.
            await connector.SyncDataToBandAsync(myBandTile, pagesData);

            //After we are done. Wait for the t if it hasn't done already
            await t;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task LoadPreviousSyncedNotesAsync()
        {
            //Try to load.
            var previousSyncedNotes = await XMLHandler.Instance.LoadFromXMLAsync();

            //And save that to the list.
            foreach(var item in previousSyncedNotes)
            {
                Notes.Add(item);
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
