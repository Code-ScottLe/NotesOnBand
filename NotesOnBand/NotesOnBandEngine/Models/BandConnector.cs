using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Band;

namespace NotesOnBandEngine.Models
{
    public class BandConnector
    {
        #region Fields
        /// <summary>
        /// Interface representing the info about the current Microsoft Band
        /// </summary>
        private IBandInfo currentBandInfo;


        /// <summary>
        /// Interface representing the actual client for connecting and syncing data to Microsoft Band
        /// </summary>
        private IBandClient currentBandClient;

        #endregion

        #region Properties

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor. Connector that handles connection to the Microsoft Band
        /// </summary>
        public BandConnector()
        {

        }
        #endregion

        #region Methods

        /// <summary>
        /// Get an information about paired Band from Device asynchronously
        /// </summary>
        private async Task GetBandInfoAsync()
        {
            //Do this only if we don't already have a band information.
            if (currentBandInfo != null)
            {
                return;
            }

            //We don't have a band info yet, get it from the phone.
            var pairedBands = await BandClientManager.Instance.GetBandsAsync();     //BandClientManager is a singleton, return an array of IBandInfo

            if (pairedBands.Count() < 1)
            {
                //we don't have a band yet.
                throw new InvalidOperationException("No paired Band detected!");
            }

            //Get the band info. Default is the first one.
            currentBandInfo = pairedBands[0];
        }


        /// <summary>
        /// Remove a tile from the Microsoft Band.
        /// </summary>
        /// <param name="tileGuid">The "to-be-removed" tile GUID</param>
        /// <returns></returns>
        public async Task RemoveTileFromBandAsync(Guid tileGuid)
        {
            if (currentBandClient == null)
            {
                throw new ArgumentNullException("currentbandClient", "Current Band Client can't be null! Please connect to a Band first");
            }

            //try to remove it.
            await currentBandClient.TileManager.RemoveTileAsync(tileGuid);
        }


        /// <summary>
        /// Remove all the data pages of the tile
        /// </summary>
        /// <param name="tileID">The tileID of the tile.</param>
        /// <returns></returns>
        public async Task RemoveAllPagesFromBandTileAsync(Guid tileID)
        {
            //remove all pages data.
            await currentBandClient.TileManager.RemovePagesAsync(tileID);
            
        }



        /// <summary>
        /// Check wheather if we have synced the given tile to the Band
        /// </summary>
        /// <param name="tileID">the TileID of the Tile</param>
        /// <returns></returns>
        public async Task<bool> IsTileSyncedAsync(Guid tileID)
        {
            //Get the list of all the tiles on the Band
            var tiles = await currentBandClient.TileManager.GetTilesAsync();

            foreach(var tile in tiles)
            {
                if(tile.TileId == tileID)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Remove a tile from the Microsoft Band
        /// </summary>
        /// <param name="tile">The "to-be-removed" Band Tile</param>
        /// <returns></returns>
        public async Task RemoveTileFromBandAsync(Microsoft.Band.Tiles.BandTile tile)
        {
            if (currentBandClient == null)
            {
                throw new ArgumentNullException("currentbandClient", "Current Band Client can't be null! Please connect to a Band first");
            }

            //try to remove it.
            await currentBandClient.TileManager.RemoveTileAsync(tile);
        }


        /// <summary>
        /// Connect to the Microsoft Band asynchronously. 
        /// </summary>
        public async Task ConnectToBandAsync(bool resuming = false)
        {
            //Do this only if we haven't connected to a Band.
            if (currentBandClient != null && resuming == false)
            {
                return;
            }


            //We haven't connected yet. Only connect if we already know which band we are connecting to.
            if (currentBandInfo == null)
            {
                //Have not acquire which band yet.
                await GetBandInfoAsync();
            }

            try
            {
                //Try to connect to the paired band
                currentBandClient = await BandClientManager.Instance.ConnectAsync(currentBandInfo);

                if (currentBandClient == null)
                {
                    //something is wrong.
                    throw new InvalidOperationException("Can't connect to this Band: " + currentBandInfo.Name);
                }

            }

            catch (BandException ex)
            {
                //Something is wrong.
                throw new AggregateException("Can't connect to this Band: " + currentBandInfo.Name, ex);
            }
        }


        /// <summary>
        /// Sync the given Tile to Band asynchronously
        /// </summary>
        /// <param name="tile"> The Band Tile to sync over.</param>
        /// <returns></returns>
        public async Task AddTileToBandAsync(Microsoft.Band.Tiles.BandTile tile)
        {
            //Because we are adding in new tiles. Remove the old one, just in case that we do have one.
            await RemoveTileFromBandAsync(tile);

            //Try to sync it over
            try
            {
                int slotsCount = await currentBandClient.TileManager.GetRemainingTileCapacityAsync();

                if (slotsCount == 0)
                {
                    throw new InvalidOperationException("Can't add tile to Band! Please go to Microsoft Health app and free a spot for the app!");
                }

                bool status = await currentBandClient.TileManager.AddTileAsync(tile);

                if (status == false)
                {
                    throw new InvalidOperationException("Can't add tile to Band!" + System.Environment.NewLine + "Detail: AddTileAsync() from currentBandClient return false!");
                }
            }

            //We only catch the anticipated one. All others will go up to the caller to handle
            catch (Microsoft.Band.BandException e)
            {
                throw new AggregateException("Can't add tile to band! Detail: Sync Failure on Band Exception", e);
            }
        }


        /// <summary>
        /// Sync the pages data with the given layout to Band asynchronously 
        /// </summary>
        /// <param name="tile">The Band Tile to sync data to</param>
        /// <param name="pageLayoutIndex"> The index of the layout that corresponds to the list od data</param>
        /// <param name="pagesData">The data for pages that we can sync over. Microsoft band can't hold more than 8 pages at once</param>
        /// <returns></returns>
        public async Task SyncDataToBandAsync(Microsoft.Band.Tiles.BandTile tile, params Microsoft.Band.Tiles.Pages.PageData[] pagesData)
        {

            //Verify data
            if (tile == null)
            {
                throw new ArgumentNullException("tile", "given tile can't be null!. Please pass in a valid Band Tile!");
            }

            else if (pagesData.Count() == 0)
            {
                //nothing to sync
                return;
            }

            //Check if we have connected
            if (currentBandClient == null)
            {
                throw new ArgumentNullException("currentbandClient", "Current Band Client can't be null! Please connect to a Band first");
            }

            //try to sync the data over
            try
            {
                bool status = await currentBandClient.TileManager.SetPagesAsync(tile.TileId, pagesData);

                if (status == false)
                {
                    throw new InvalidOperationException("Can't sync data to Band. Status is false");
                }
            }

            //Catch band exception only
            catch (Microsoft.Band.BandException e)
            {
                throw new AggregateException("Can't sync data to Band. BandException occurred", e);
            }
        }


        /// <summary>
        /// Get the current band theme from the band
        /// </summary>
        /// <returns></returns>
        public async Task<BandTheme> GetCurrentBandThemeAsync()
        {
            if(currentBandClient == null)
            {
                throw new ArgumentNullException("currentbandClient", "Current Band Client can't be null! Please connect to a Band first");
            }

            return await currentBandClient.PersonalizationManager.GetThemeAsync();
        }

        #endregion
    }
}
