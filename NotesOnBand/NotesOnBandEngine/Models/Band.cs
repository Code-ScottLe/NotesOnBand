using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;
using System.ComponentModel;

namespace NotesOnBandEngine.Models
{
    /// <summary>
    /// Enum representing the version of band that we are dealing with.
    /// </summary>
    public enum BandVersion
    {
        MicrosoftBand1 = 1, MicrosoftBand2 = 2
    }

    /// <summary>
    /// Represent a Microsoft Band
    /// </summary>
    public class Band : INotifyPropertyChanged
    {
        #region Fields
        private IBandInfo currentBandInfo;

        private IBandClient currentBandClient;

        private BandVersion currentVersion = BandVersion.MicrosoftBand2;

        private BandTile currentTile;

        private string uniqueIDString = "b40d28db-a774-4b6f-a97a-76272146a174";
        #endregion

        #region events

        /// <summary>
        /// Implement the INotifyPropertyChanged Interface. Use this to notify the View and others about the property that was changed to perform updates.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Enum represent the current version of Microsoft Band that we are dealing with. Default is Band 2
        /// </summary>
        public BandVersion CurrentVersion
        {
            get
            {
                return currentVersion;
            }

            set
            {
                currentVersion = value;
                OnPropertyChanged("CurrentVersion");
            }
        }




        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor for the Band class.
        /// </summary>
        public Band()
        {

        }

        /// <summary>
        /// Constructor for the Band class.
        /// </summary>
        /// <param name="bandInfo">Paired Band</param>
        /// <param name="version">Version of Paired Band</param>
        public Band(IBandInfo bandInfo, BandVersion version) : this()
        {
            currentBandInfo = bandInfo;
            currentVersion = version;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Get an information about paired Band from Device asynchronously
        /// </summary>
        /// <returns>Boolean value. True indicate that Info of Band was returned successfully, False indicates failure</returns>
        public async Task<bool> GetBandInfoAsync()
        {
            //Do this only if we don't already have a band information.
            if (currentBandInfo != null)
            {
                return true;
            }

            //We don't have a band info yet, get it from the phone.
            var pairedBands = await BandClientManager.Instance.GetBandsAsync();     //BandClientManager is a singleton, return an array of IBandInfo

            if (pairedBands.Count() < 1)
            {
                //we don't have a band yet.
                return false;
            }

            //Get the band info. Default is the first one.
            currentBandInfo = pairedBands[0];

            return true;
        }

        /// <summary>
        /// Connect to the Microsoft Band asynchronously. 
        /// </summary>
        /// <returns>True indicate success. False indicate failure</returns>
        public async Task<bool> ConnectToBandAsync()
        {
            //Do this only if we haven't connected to a Band.
            if (currentBandClient != null)
            {
                return true;
            }


            //We haven't connected yet. Only connect if we already know which band we are connecting to.
            if (currentBandInfo == null)
            {
                //Have not acquire which band yet.
                return false;
            }

            try
            {
                //Try to connect to the paired band
                currentBandClient = await BandClientManager.Instance.ConnectAsync(currentBandInfo);

                if (currentBandClient == null)
                {
                    //something is wrong.
                    return false;
                }

                return true;
            }

            catch (BandException ex)
            {
                //Something is wrong.
                return false;
            }
        }


        /// <summary>
        /// Create the tile for notes
        /// </summary>
        /// <param name="pagesCount"> Number of pages/notes insides the tile. Can't be more than 8</param>
        /// <returns></returns>
        public async Task<BandTile> CreateBandTileAsync(int pagesCount)
        {
            //Create the Tile's Page layout.
            //Would look like this:
            // +--------------------+
            // | Note #1            |       <=== Header Textblock
            // | Angel is the best  |       <=== Wrapperd TextBlock for the Note
            // | girl. Ever <3      |
            // +--------------------+


            //Step 1: Create the TextBlock for the Header
            TextBlock myHeaderTextBlock = new TextBlock();
            myHeaderTextBlock.ElementId = 1;                //ElementID starts from 1.

            //TO CHANGE: This is currently fixed to Microsoft band 2.
            //Band 1 Workable Width : 245px;
            //Band 2 Workable Width : 258px
            myHeaderTextBlock.Rect = new PageRect(0, 0, 200, 25);       //Because we have to store everything in a ScrollFlowPanel, we leave the offset (x,y) to the scroll panel.


            //Color of the header will match with the color theme of the band for consistency. This will change with the user choice of color.
            myHeaderTextBlock.ColorSource = ElementColorSource.BandBase;


            //Step 2: Create the WrappedTextBlock below for the note.
            WrappedTextBlock myNoteWrappedTextBlock = new WrappedTextBlock();
            myNoteWrappedTextBlock.ElementId = 2;

            //TO CHANGE: This is currently fixed to Microsoft band 2.
            //Band 1 Workable Width : 245px;
            //Band 2 Workable Width : 258px
            //WARNING: WrappedTextBlock seems not to respect the width setting of the PageRect().
            myNoteWrappedTextBlock.Rect = new PageRect(0, 0, 250, 100);



            //Step 3: Create the container for the controllers. We use SCrollFlowPanel for long texts. This is much like grid on the WPF's window
            ScrollFlowPanel myPageScrollFlowPanel = new ScrollFlowPanel(myHeaderTextBlock, myNoteWrappedTextBlock);

            //Set the flow of the panel to be vertically as we will be scrolling down for more texts
            myPageScrollFlowPanel.Orientation = FlowPanelOrientation.Vertical;

            //Set the color of the scroll bar to match with the theme as well
            myPageScrollFlowPanel.ScrollBarColorSource = ElementColorSource.BandBase;

            //TO CHANGE: This is currently fixed to Microsoft band 2.
            //Band 1 Workable Width : 245px;
            //Band 2 Workable Width : 258px
            //Band 1 Workable Height: 106px;
            //Band 2 Workable Height: 128px;
            myPageScrollFlowPanel.Rect = new PageRect(0, 0, 250, 128);


            //Step 4: Create the page layout from the container with all the controllers  that we just defined.
            PageLayout myPageLayout = new PageLayout(myPageScrollFlowPanel);


            //Step 5: Create the actual BandTile
            //Each Tile need a GUID, this uses the Windows Store ID 
            Guid myGuid = new Guid(uniqueIDString);

            //Create the Band TIle.
            BandTile myTile = new BandTile(myGuid);

            //Setup the tile.

            //Name
            myTile.Name = "Notes on Band";

            //Small and Large Icon
            myTile.SmallIcon = await LoadIcon("ms-appx:///Assets/TileIconSmall.png");
            myTile.TileIcon = await LoadIcon("ms-appx:///Assets/TileIconLarge.png");

            //Add the layout to the tile. One tile can hold up to 8 different pages, we add no more than that.
            for(int i = 0; i < 8 && i < pagesCount; i++)
            {
                myTile.PageLayouts.Add(myPageLayout);
            }

            //done with tile. return it
            return myTile;
            
        }

        /// <summary>
        /// Sync the given tile over to the phone.
        /// </summary>
        /// <param name="myTile">Band Tile that we want to sync</param>
        /// <returns></returns>
        public async Task<bool> SyncTileToBandAsync()
        {
            //We want to start fresh and without any collision. so we remove the old one first (if we have one)
            await currentBandClient.TileManager.RemoveTileAsync(currentTile.TileId);

            //Sync it over to phone.
            bool status = await currentBandClient.TileManager.AddTileAsync(currentTile);

            return status;
        }


        public async Task<bool> SyncNotesToBandAsync(List<string> notes)
        {
            //We only sync over the notes that actually have data.
            string headerPrefix = "Note #";
            PageData[] pagesData = new PageData[notes.Count];

            for(int i = 0; i < notes.Count && i < 8;i++)
            {

                //Create the header
                TextBlockData headerText = new TextBlockData(1, headerPrefix + (i + 1).ToString());

                //Create the notetext.
                WrappedTextBlockData noteText = new WrappedTextBlockData(2, notes[i]);

                //Wrap them in a page data.
                pagesData[i] = new PageData(Guid.NewGuid(), i, headerText, noteText);

            }

            //Done creating data, sync over.
            bool status = await currentBandClient.TileManager.SetPagesAsync(currentTile.TileId, pagesData);

            return status;
        }


        /// <summary>
        /// Load up an icon (png) file and convert it to an actual BandIcon object to be used with creating a new tile.
        /// </summary>
        /// <param name="url"> Url to the icon</param>
        /// <returns>The BandIcon object that can be used to set the Icon on the band for the tile.</returns>
        private async Task<BandIcon> LoadIcon(string url)
        {

            //Get the Image file from disk.
            Windows.Storage.StorageFile iconFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(url));

            //Open up the file for random access (but stay with read>)
            using (Windows.Storage.Streams.IRandomAccessStream fileStream = await iconFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                //Create a new bitmap 
                Windows.UI.Xaml.Media.Imaging.WriteableBitmap iconWriteableBitmap = new Windows.UI.Xaml.Media.Imaging.WriteableBitmap(1, 1);

                //Process the picture to useable bitmap.
                await iconWriteableBitmap.SetSourceAsync(fileStream);

                //Extension method is written to convert a bitmap to an BandIcon that we can use with the Band
                return iconWriteableBitmap.ToBandIcon();
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
