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
        /// Event handler for checked button. = Band 2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BandChoiceToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            //Change the text value of the version to Band 2.
            BandVersionTextBlock.Text = "Band 2";
            BandVersionTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.DeepSkyBlue);
            mainPageViewModel.CurrentBandVersion = NotesOnBandEngine.Models.BandVersion.MicrosoftBand2;
        }



        /// <summary>
        /// Event handler for unchecked button = Band 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BandChoiceToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            //Change the text value of the version to band 1
            BandVersionTextBlock.Text = "Band 1";
            BandVersionTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Purple);
            mainPageViewModel.CurrentBandVersion = NotesOnBandEngine.Models.BandVersion.MicrosoftBand1;
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
            BandNote newBandNote = new BandNote()
            {
                Title = $"Note #{mainPageViewModel.Notes.Count + 1}", Content = $"Note #{mainPageViewModel.Notes.Count + 1}"
            };

            //Add a new note
            mainPageViewModel.Notes.Add(newBandNote);

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
            mainPageViewModel.Notes.RemoveAt(mainPageViewModel.Notes.Count - 1);

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

            //Sync it over.
            try
            {
                await mainPageViewModel.SyncNotesToBandAsync();

                //throw new InvalidOperationException("This is a test exception");
            }
            
            catch (Exception ex)
            {
                //Something is wrong. Display message
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
        /// Event handler for the double tab on the TextBlock of Note Title.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            //We call in the pop up for the Title editor with the old title
            TitleEditorContentDialog contentDialog = new TitleEditorContentDialog((sender as TextBlock).Text);

            //Show the thing
            await contentDialog.ShowAsync();

            //After this, check if the title has been changed
            if(contentDialog.TitleChanged == true)
            {
                mainPageViewModel.Notes.Where(n => n.Title == (sender as TextBlock).Text).
                    Select(n => n).FirstOrDefault().Title = contentDialog.NewTitle;
            }
        }
    }
}
