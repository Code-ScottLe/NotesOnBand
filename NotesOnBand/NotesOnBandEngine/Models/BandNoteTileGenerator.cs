using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;

namespace NotesOnBandEngine.Models
{
    public class BandNoteTileGenerator
    {

        #region Fields
        private static BandNoteTileGenerator instance;
        private string uniqueIDString = "b40d28db-a774-4b6f-a97a-76272146a174";
        #endregion


        #region Properties
        public static BandNoteTileGenerator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BandNoteTileGenerator();
                }

                return instance;
            }
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Default Constructor for Band Note Tile Generator
        /// </summary>
        BandNoteTileGenerator()
        {

        }
        #endregion


        #region Methods

        /// <summary>
        /// Generate a band tile for the Note application with the layout.
        /// </summary>
        /// <returns></returns>
        public async Task<BandTile> GenerateBandTileAsync(BandVersion CurrentVersion)
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

            //Band 1 Workable Width : 245px;
            //Band 2 Workable Width : 258px
            //Band 1 Workable Height: 106px;
            //Band 2 Workable Height: 128px;
            myHeaderTextBlock.Rect = new PageRect(0, 0, 200, 25);       //Because we have to store everything in a ScrollFlowPanel, we leave the offset (x,y) to the scroll panel.


            //Color of the header will match with the color theme of the band for consistency. This will change with the user choice of color.
            myHeaderTextBlock.ColorSource = ElementColorSource.BandBase;


            //Step 2: Create the WrappedTextBlock below for the note.
            WrappedTextBlock myNoteWrappedTextBlock = new WrappedTextBlock();
            myNoteWrappedTextBlock.ElementId = 2;

            //Band 1 Workable Width : 245px;
            //Band 2 Workable Width : 258px
            //Band 1 Workable Height: 106px;
            //Band 2 Workable Height: 128px;
            //WARNING: WrappedTextBlock seems not to respect the width setting of the PageRect().
            if (CurrentVersion == BandVersion.MicrosoftBand2)
            {
                myNoteWrappedTextBlock.Rect = new PageRect(0, 0, 250, 100);
            }


            else
            {
                myNoteWrappedTextBlock.Rect = new PageRect(0, 0, 250, 80);
            }



            //Step 3: Create the container for the controllers. We use SCrollFlowPanel for long texts. This is much like grid on the WPF's window
            ScrollFlowPanel myPageScrollFlowPanel = new ScrollFlowPanel(myHeaderTextBlock, myNoteWrappedTextBlock);

            //Set the flow of the panel to be vertically as we will be scrolling down for more texts
            myPageScrollFlowPanel.Orientation = FlowPanelOrientation.Vertical;

            //Set the color of the scroll bar to match with the theme as well
            myPageScrollFlowPanel.ScrollBarColorSource = ElementColorSource.BandBase;

            //Band 1 Workable Width : 245px;
            //Band 2 Workable Width : 258px
            //Band 1 Workable Height: 106px;
            //Band 2 Workable Height: 128px;
            if (CurrentVersion == BandVersion.MicrosoftBand2)
            {
                myPageScrollFlowPanel.Rect = new PageRect(0, 0, 250, 128);
            }

            else
            {
                myPageScrollFlowPanel.Rect = new PageRect(0, 0, 250, 106);
            }


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

            //Add the layout to the tile. One tile can hold up to 5 different layout.
            myTile.PageLayouts.Add(myPageLayout);

            //done with tile. return it
            return myTile;
        }


        /// <summary>
        /// Generate pages data to be synced over to the band
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        public PageData[] GenerateDataFromNotes(List<BandNote> notes)
        {           
            string headerPrefix = "Note #";
            PageData[] pagesData = new PageData[notes.Count];

            for (int i = 0; i < notes.Count; i++)
            {

                //Create the header. Remember that the notes list are backward (as how the band display them), so the title with note number will be backward as well.
                TextBlockData headerText = new TextBlockData(1, headerPrefix + (notes.Count - i).ToString());

                //Create the note text.
                WrappedTextBlockData noteText = new WrappedTextBlockData(2, notes[i].Content);

                //Wrap them in a page data.
                //the pageLayoutIndex is the layout index that will be used to create this page with the given data. the layout the blueprint for a page, not an actual page.
                pagesData[i] = new PageData(Guid.NewGuid(), 0, headerText, noteText);

            }

            return pagesData;
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

        #endregion
    }
}
