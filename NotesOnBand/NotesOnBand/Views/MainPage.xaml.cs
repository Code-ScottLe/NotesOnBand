using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;
using System.Threading.Tasks;
using NotesOnBand.Views;
using NotesOnBandEngine.ViewModels;
using NotesOnBandEngine.Models;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NotesOnBand
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            //Enable cache.
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey("BandVersion") == true)
            {
                mainPageViewModel.CurrentBandVersion = (BandVersion)Enum.Parse(typeof(BandVersion), (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["BandVersion"]);
            }

            Application.Current.Suspending += mainPageViewModel.OnSuspending;
            Application.Current.Resuming += mainPageViewModel.OnResuming;
        }

        /// <summary>
        ///     Create the popup message for the user.
        /// </summary>
        /// <param name="message">Main message to show the user</param>
        /// <param name="title">Optional title for the message</param>
        /// <returns></returns>
        private async Task CreatePopupMessage(string message, string title = "")
        {
            Windows.UI.Popups.MessageDialog dialog = null;

            if (string.IsNullOrEmpty(title))
            {
               dialog = new Windows.UI.Popups.MessageDialog(message);
            }

            else
            {
                dialog = new Windows.UI.Popups.MessageDialog(message, title);
            }

            await dialog.ShowAsync();
            
        }
      
        /// <summary>
        /// Click Event Handler for the Setting AppBar Button. Navigate to the Setting Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingPage));
        }

        /// <summary>
        /// Click Event handler for the Add AppBar Button. Add a new note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            //Temporary disable the button
            (sender as AppBarButton).IsEnabled = false;

            //Make a new note.
            mainPageViewModel.AddNote();

            //Check if we have to re-enable the delete button
            if(mainPageViewModel.Notes.Count > 0)
            {
                DeleteNote.IsEnabled = true;
            }

            if (mainPageViewModel.Notes.Count < 8)
            {
                //re-enable the button if we haven't use all the spaces.
                (sender as AppBarButton).IsEnabled = true;
            }
            
        }

        /// <summary>
        /// Click event handler for the Remove AppBar Button. Remove the last note.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteNote_Click(object sender, RoutedEventArgs e)
        {
            //Temporary disable the button
            (sender as AppBarButton).IsEnabled = false;

            //By Default, remove the last one.
            mainPageViewModel.RemoveNote();

            //See if we have to re-enable the add button
            if(mainPageViewModel.Notes.Count < 8)
            {
                AddNote.IsEnabled = true;
            }

            if(mainPageViewModel.Notes.Count > 0)
            {
                //re-enable the button if we haven't deleted all of the notes
                (sender as AppBarButton).IsEnabled = true;
            }
        }

        /// <summary>
        /// Event Handler for the Sync AppBar Button. Sync the notes to the Band
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SyncNote_Click(object sender, RoutedEventArgs e)
        {
            //Disable all button.
            AddNote.IsEnabled = false;
            DeleteNote.IsEnabled = false;
            SyncNote.IsEnabled = false;
            Setting.IsEnabled = false;

            //Reset progress bar.
            SyncProgressBar.Visibility = Visibility.Visible;
            mainPageViewModel.CompletionStatus = "";
            SyncProgressBarIndicator.Visibility = Visibility.Visible;
            mainPageViewModel.IsFaulted = false;

            //Sync it over.
            try
            {
                await mainPageViewModel.SyncNotesToBandAsync();

                //throw new InvalidOperationException("This is a test exception");
            }
            
            catch (Exception ex)
            {
                //Something is wrong. Display message
                mainPageViewModel.IsFaulted = true;
                mainPageViewModel.CompletionStatus = "Failure";
                string title = "Whoops :( Something is wrong";
                string message = ex.Message;

                if (ex.InnerException != null)
                {
                    message += System.Environment.NewLine;
                    message += "Inner Exception:";
                    message += ex.InnerException.Message;
                }

                //await CreatePopupMessage(message, title);

                //Build the popup. 
                ContentDialog dialog = new ContentDialog()
                {
                    Title = title,
                    Content = message,
                    PrimaryButtonText = "Send Report",
                    SecondaryButtonText = "Dismiss"
                };
                
                //Handle the first button click.
                dialog.PrimaryButtonClick += async (contentDialog, contentDialogClickEventArg) => 
                {
                    //call the whoever in the view model for this shit.
                    await mainPageViewModel.SendCrashReport(ex);
                };

                //Show the dialog
                await dialog.ShowAsync();
                
            }
            
            finally
            {
                //Enable all button.
                if (mainPageViewModel.Notes.Count < 8)
                {
                    AddNote.IsEnabled = true;
                }

                if (mainPageViewModel.Notes.Count > 0)
                {
                    DeleteNote.IsEnabled = true;
                }
                SyncNote.IsEnabled = true;
                Setting.IsEnabled = true;
            }    

            
        }
       
        /// <summary>
        /// Event handler for the event that the page was successfully loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Only do this if we have not had anything loaded in. 
            if(mainPageViewModel.IsInitialized == false)
            {
                //Flip the bool
                mainPageViewModel.IsInitialized = true;

                //Disable all button.
                AddNote.IsEnabled = false;
                DeleteNote.IsEnabled = false;
                SyncNote.IsEnabled = false;
                Setting.IsEnabled = false;

                //try to load from XML
                await mainPageViewModel.LoadPreviousSyncedNotesAsync();

                //Enable all button.
                if (mainPageViewModel.Notes.Count < 8)
                {
                    AddNote.IsEnabled = true;
                }

                if (mainPageViewModel.Notes.Count > 0)
                {
                    DeleteNote.IsEnabled = true;
                }
                SyncNote.IsEnabled = true;
                Setting.IsEnabled = true;
            }
            
        }

        /// <summary>
        /// Event handler for the Value Changed Event for the Progress bar. Hide progress bar and indicator at 100%
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyncProgressBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

            //100, then animate hiding.
            if ((sender as ProgressBar).Value == 100)
            {
                Storyboard myStoryBoard = new Storyboard();

                //Ref: http://stackoverflow.com/questions/5495446/setting-the-visibility-of-an-element-to-collapsed-when-storyboard-completes-usin
                ObjectAnimationUsingKeyFrames myKeyFrameProgressBar = new ObjectAnimationUsingKeyFrames()
                {
                    BeginTime = new TimeSpan(0,0,0)
                                        
                };

                //Add in key frames.
                myKeyFrameProgressBar.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = new TimeSpan(0, 0, 0), Value = Visibility.Visible });
                myKeyFrameProgressBar.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = new TimeSpan(0, 0, 5), Value = Visibility.Collapsed });
                //Set aim at the Sync Progressbar.
                Storyboard.SetTarget(myKeyFrameProgressBar, SyncProgressBar);
                Storyboard.SetTargetProperty(myKeyFrameProgressBar, "Visibility");

                //Ref: http://stackoverflow.com/questions/5495446/setting-the-visibility-of-an-element-to-collapsed-when-storyboard-completes-usin
                ObjectAnimationUsingKeyFrames myKeyFrameProgressStatus = new ObjectAnimationUsingKeyFrames()
                {
                    BeginTime = new TimeSpan(0, 0, 0)

                };

                //Add in key frames.
                myKeyFrameProgressStatus.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = new TimeSpan(0, 0, 0), Value = Visibility.Visible });
                myKeyFrameProgressStatus.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = new TimeSpan(0, 0, 5), Value = Visibility.Collapsed });

                //Set aim at the sync progress status
                Storyboard.SetTarget(myKeyFrameProgressStatus, SyncProgressBarIndicator);
                Storyboard.SetTargetProperty(myKeyFrameProgressStatus, "Visibility");

                //Add them all on to the storyboard
                myStoryBoard.Children.Add(myKeyFrameProgressBar);
                myStoryBoard.Children.Add(myKeyFrameProgressStatus);

                //Animate
                myStoryBoard.Begin();
            }
        }      

        /// <summary>
        /// Event handler for the Version Info Button click on the AppBar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void VersionInfo_Click(object sender, RoutedEventArgs e)
        {
            //Get the update file.
            var versionUpdateText = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Documents/VersionUpdate.txt"));
            //Read as string
            string versionUpdate = await Windows.Storage.FileIO.ReadTextAsync(versionUpdateText);


            //Create the popup.
            ContentDialog updateDialog = new ContentDialog()
            {
                Title = $"What is new in v{Windows.ApplicationModel.Package.Current.Id.Version.Major}.{Windows.ApplicationModel.Package.Current.Id.Version.Minor}.{Windows.ApplicationModel.Package.Current.Id.Version.Build}.{Windows.ApplicationModel.Package.Current.Id.Version.Revision}",
                Content = versionUpdate
            };

            //updateDialog.FullSizeDesired = true;
            updateDialog.PrimaryButtonText = "Dismiss";
            updateDialog.IsSecondaryButtonEnabled = false;

            await updateDialog.ShowAsync();
        }

        /// <summary>
        /// Right tapped event handler for the Stack Panel of Flyout.
        /// Only showing the flyout on non-touch device. Touch device will use hold.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            }

        }

        /// <summary>
        /// Holding event handler for the stack panel of the flyout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StackPanel_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        /// <summary>
        /// Event handler for the edit title button on the flyout of the item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EditTitleFlyoutButton_Click(object sender, RoutedEventArgs e)
        {           
            //Take the note out.
            BandNote currentNote = ((sender as Button).Parent as FrameworkElement).DataContext as BandNote;
            string title = currentNote.Title;

            TitleEditorContentDialog contentDialog = new TitleEditorContentDialog(title);
          
            //show it
            await contentDialog.ShowAsync();

            if(contentDialog.TitleChanged == true)
            {
                currentNote.Title = contentDialog.NewTitle;
            }

            //Make sure the damn thing go away. FUCKING UNBELIEVABLE
            (((sender as FrameworkElement).Parent as FrameworkElement).Parent as FlyoutPresenter).Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Event handler for the delete note on the flyout of the item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteNoteFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            //Take the note out.
            BandNote currentNote = ((sender as Button).Parent as FrameworkElement).DataContext as BandNote;

            //Remove it
            mainPageViewModel.RemoveNote(currentNote);        
          
        }
    }
}
