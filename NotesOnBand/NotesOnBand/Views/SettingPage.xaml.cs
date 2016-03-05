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
            //base.OnNavigatedTo(e);

            //Ask what kind of theme are we in and then set the correct toggle status
            var currentTheme = Application.Current.RequestedTheme;

            if (currentTheme == ApplicationTheme.Light)
            {
                ThemeSettingToggleSwitch.IsOn = true;
            }

            else
            {
                ThemeSettingToggleSwitch.IsOn = false;
            }

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

            //Handled  indicate if the back button has been handled or not.
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
        private void ThemeSettingToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //On indicate light theme
            if(ThemeSettingToggleSwitch.IsOn == true)
            {
               
            }
             
            //Not on is dark.
            else
            {
                
            }
        }
    }
}
