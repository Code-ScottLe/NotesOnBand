using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;

namespace NotesOnBand.Models
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
        public Band(IBandInfo bandInfo, BandVersion version): this()
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
            if(currentBandInfo != null)
            {
                return true;
            }

            //We don't have a band info yet, get it from the phone.
            var pairedBands = await BandClientManager.Instance.GetBandsAsync();     //BandClientManager is a singleton, return an array of IBandInfo

            if(pairedBands.Count() < 1)
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
            if(currentBandClient != null)
            {
                return true;
            }


            //We haven't connected yet. Only connect if we already know which band we are connecting to.
            if(currentBandInfo == null)
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

            catch(BandException ex)
            {
                //Something is wrong.
                return false;
            }
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
