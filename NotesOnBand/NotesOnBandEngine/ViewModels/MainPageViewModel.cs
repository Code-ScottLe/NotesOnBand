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
        private bool isInitialized = false;
        private bool isFaulted = false;
        #endregion

        #region events

        /// <summary>
        /// Implement the INotifyPropertyChanged Interface. Use this to notify the View about the property that was changed to perform updates.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This event will fire up in the case of something went wrong during the process of doing its job
        /// </summary>
        public event EventHandler<AggregateException> ErrorOccured;

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

            private set
            {
                notes = value;
                OnPropertyChanged(nameof(Notes));
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
                //back up band type to setting
                ApplicationData.Current.LocalSettings.Values["BandVersion"] = value.ToString();
                OnPropertyChanged(nameof(CurrentBandVersion));
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
                OnPropertyChanged(nameof(CompletionPercentage));
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
                OnPropertyChanged(nameof(CompletionStatus));
            }
        }

        public bool IsInitialized
        {
            get
            {
                return isInitialized;
            }

            set
            {
                isInitialized = value;
            }
        }

        public bool IsFaulted
        {
            get
            {
                return isFaulted;
            }

            set
            {
                isFaulted = false;
                OnPropertyChanged(nameof(IsFaulted));
            }
        }

        public bool IsSyncing { get; set; }

        public bool IsResumed { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor of the MainPageViewModel
        /// </summary>
        public MainPageViewModel()
        {
            notes = new ObservableCollection<BandNote>();
            connector = new BandConnector();
            CompletionStatus = string.Empty;

            CurrentBandVersion = BandVersion.MicrosoftBand2;

            IsResumed = false;

        }

        #endregion

        #region Methods         

        public async Task RemoveTileFromBandAsync()
        {
            //Starting.
            CompletionPercentage = 0;

            //Connect to Band
            await connector.ConnectToBandAsync(IsResumed);

            //Empty note. Just remove the Tile.
            await connector.RemoveTileFromBandAsync(new Guid(uniqueIDString));

            //write empty XML.
            await XMLHandler.Instance.SaveToXMLAsync(Notes.ConvertToList());

            //Set the value to 100 to hide away the progress bar
            CompletionPercentage = 100;
            CompletionStatus = "Tile Removed!";
            return;
        }

        public void AddNote(BandNote note)
        {
            Notes.Add(note);
        }

        public void AddNote()
        {
            AddNote(new BandNote() { Title = $"Note #{Notes.Count + 1}", Content = $"Note #{Notes.Count + 1}" });
        }

        public void RemoveNote(BandNote note)
        {
            Notes.Remove(note);
        }

        public void RemoveNote()
        {
            if(Notes.Count > 0)
            {
                RemoveNote(Notes.Last());
            }         
        }

        /// <summary>
        /// Sync the given notes to the Band asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task SyncNotesToBandAsync()
        {
            //Starting.
            IsSyncing = true;
            CompletionPercentage = 0;
            //Check if we have notes to sync
            if(Notes.Count == 0)
            {
                await RemoveTileFromBandAsync();
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
            await connector.ConnectToBandAsync(IsResumed);
            CompletionPercentage = 50;

            bool tileExisted = await connector.IsTileSyncedAsync(myBandTile.TileId);

            //Try to add Tile if we haven't already.
            if(tileExisted == false)
            {
                try
                {
                    await connector.AddTileToBandAsync(myBandTile);
                }
               
                catch (InvalidOperationException e)
                {
                    //Invalid operation is expected to be thrown on certain case.
                    if(e.Message.Contains("AddTileAsync()"))
                    {
                        //Can't add tile
                        CompletionStatus = "Tile didn't sync";
                        IsFaulted = true;
                        return;
                    }

                    else
                    {
                        //Can't add tile due to no slot available
                        CompletionStatus = "Full slot";
                        IsFaulted = true;
                        return;
                    }
                }
            }
           
            CompletionPercentage = 75;

            //Generate the data from the notes.
            var listy = Notes.ConvertToList();
            listy.Reverse();

            //Generate the data.
            var pagesData = BandNoteTileGenerator.Instance.GenerateDataFromNotes(listy);

            //Sync the data over.

            //if the tile was previously synced, remove all the previous notes.
            if(tileExisted == true)
            {
                await connector.RemoveAllPagesFromBandTileAsync(myBandTile.TileId);
            }

            await connector.SyncDataToBandAsync(myBandTile, pagesData);
            CompletionPercentage = 95;

            //After we are done. Wait for the t if it hasn't done already
            await t;
            CompletionPercentage = 100;
            CompletionStatus = "Done.";

            IsSyncing = false;

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
        /// Method for sending back the exception detail for bug fixes
        /// </summary>
        /// <param name="e"></param>
        public async Task SendCrashReport(Exception e)
        {
            //Create the email to be sent.
            Windows.ApplicationModel.Email.EmailMessage emailMessage = new Windows.ApplicationModel.Email.EmailMessage();

            //Get the current app version
            var packageVersion = Windows.ApplicationModel.Package.Current.Id.Version;
            string version = $"{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}.{packageVersion.Revision}";

            //Build the body.
            StringBuilder emailMessageBodyBuilder = new StringBuilder();
            emailMessageBodyBuilder.AppendLine($"Exception Type: {e.GetType().FullName}");
            emailMessageBodyBuilder.AppendLine($"Message: {e.Message}");
            emailMessageBodyBuilder.AppendLine($"App version: {version}");
            emailMessageBodyBuilder.AppendLine($"Detail: {e.ToString()}");

            //set the body:       
            emailMessage.Body = emailMessageBodyBuilder.ToString();

            //format the subject of the email (for inbox filtering)
            emailMessage.Subject = $"[NoB][v{version}]{e.GetType().FullName} ";

            //set the sender.
            emailMessage.To.Add(new Windows.ApplicationModel.Email.EmailRecipient("code.scottle@outlook.com"));

            //send it to the default mail application.
            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);

        }

        /// <summary>
        /// Event handler for the event of app suspending
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            //Save all notes, in case of lost
            await XMLHandler.Instance.SaveToXMLAsync(Notes.ConvertToList());

            deferral.Complete();

        }

        /// <summary>
        /// Event handler for the event of app resuming
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnResuming(object sender, object e)
        {
            IsResumed = true;          
        }


        /// <summary>
        /// Fire up the PropertyChanged event and notify all the listener about the changed property.
        /// </summary>
        /// <param name="propertyName">Name of the property that was changed.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Fire up the ErrorOccured event that notify about the failure of the given task.
        /// </summary>
        /// <param name="innerException"></param>
        private void OnErrorOccured(Exception innerException)
        {
            ErrorOccured?.Invoke(this, new AggregateException(innerException));
        }

        #endregion

    }
}
