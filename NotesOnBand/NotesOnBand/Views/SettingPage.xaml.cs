using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NotesOnBand.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            this.InitializeComponent();

            //Register back button
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += SettingPage_BackRequested;

            //Grab the version of the application
            var packageVersion = Windows.ApplicationModel.Package.Current.Id.Version;
            string version = string.Format("{0}.{1}.{2}.{3}", packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
            VersionTextBlock.Text += version;
        }

        /// <summary>
        /// Override the NavigatedTo Event Handler. This will fire up when the page is being navigated to from the main UI.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);      
             
        }

        /// <summary>
        /// Event handler for the back pressed button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            //Get the current frame that is being displayed in the current window.
            Frame currentFrame = Window.Current.Content as Frame;

            if (currentFrame == null)
            {
                //something wrong
                return;
            }

            //Handled property indicate if the back button has been handled or not.
            //We only handle it here if it is not already handled (usually by the system)
            if (currentFrame.CanGoBack == true && e.Handled == false)
            {
                //set the event as already being handled
                e.Handled = true;

                currentFrame.GoBack();
            }
        }


        /// <summary>
        /// Event handler for the toggled theme setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ThemeSettingToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //On indicate light theme
            if(ThemeSettingToggleSwitch.IsOn == true && (int)ThemeSettingToggleSwitch.Tag == 1)
            {
                //Save the setting
                var localSttings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSttings.Values["UserRequestedTheme"] = "Light";
                
            }
             
            //Not on is dark.
            else if (ThemeSettingToggleSwitch.IsOn == false && (int)ThemeSettingToggleSwitch.Tag == 1)
            {
                //Save the setting
                var localSttings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSttings.Values["UserRequestedTheme"] = "Dark";
                
            }

            if ((int)ThemeSettingToggleSwitch.Tag == 1)
            {
                MessageDialog myPopUp = new MessageDialog("Restart the app now?");
                myPopUp.Commands.Add(new UICommand("Yep", this.AppKillIUICommandEventHandler));
                myPopUp.Commands.Add(new UICommand("Nope"));
                await myPopUp.ShowAsync();
            }
           
            
        }


        /// <summary>
        /// Handler for the app restart on the popup upon theme change.
        /// </summary>
        /// <param name="command"></param>
        private void AppKillIUICommandEventHandler(IUICommand command)
        {
            Application.Current.Exit();
        }

        private void ThemeSettingToggleSwitch_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Ask what kind of theme are we in and then set the correct toggle status
            var currentTheme = Application.Current.RequestedTheme;
            ThemeSettingToggleSwitch.Tag = 0;

            if (currentTheme == ApplicationTheme.Light)
            {
                ThemeSettingToggleSwitch.IsOn = true;

            }

            else
            {
                ThemeSettingToggleSwitch.IsOn = false;

            }

            ThemeSettingToggleSwitch.Tag = 1;
        }


        /// <summary>
        /// Event handler for clicking the delete cache button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteCacheButton_Click(object sender, RoutedEventArgs e)
        {
            //Disable the button to prevent multiple clicking
            (sender as Button).IsEnabled = false;

            //Set the progress ring to be active.
            DeleteCacheProgressRing.IsActive = true;


            MessageDialog myPopUp = new MessageDialog("Delete the cached notes? This will remove any previously saved notes on the phone only (you will still have them on the Band)");
            myPopUp.Commands.Add(new UICommand("Yep", async (command) => {
                //try to delete the file.
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile myXMLStorageFile = await localFolder.TryGetItemAsync("NotesOnBandSyncedNotes.xml") as Windows.Storage.StorageFile;

                if (myXMLStorageFile != null)
                {
                    await myXMLStorageFile.DeleteAsync();
                }
            }));

            myPopUp.Commands.Add(new UICommand("Nope"));

            await myPopUp.ShowAsync();

            //disable the ring
            DeleteCacheProgressRing.IsActive = false;

            //re-enable the button
            (sender as Button).IsEnabled = true;
        }
    }
}
