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

            
        }

        /// <summary>
        /// Add a new notes on to the view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddNote_Click(object sender, RoutedEventArgs e)
        {
            //Ask how many notes we have so far.
            int notesCount = (MainStackPanel.Children.Count -1) / 2;

            //We can't add more than 8.
            if (notesCount >= 8)
            {
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Can't have more than 8 notes!", "Caution!");
                await dialog.ShowAsync();
                return;
            }

            //Create a new TextBlock for the label
            TextBlock noteLabelTextBlock = new TextBlock();
            noteLabelTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Gray);
            noteLabelTextBlock.Text = "Note #" + (notesCount + 1).ToString();
            noteLabelTextBlock.Margin = new Thickness(10, 10, 0, 0);
            noteLabelTextBlock.Name = "Note" + (notesCount + 1).ToString() + "LabelTextBlock";

           

            //Create the textBox for the label.
            TextBox noteTextBox = new TextBox();
            noteTextBox.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            noteTextBox.Margin = new Thickness(10, 5, 10, 0);
            noteTextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            noteTextBox.AcceptsReturn = true;
            noteTextBox.TextWrapping = TextWrapping.Wrap;
            noteTextBox.Name = "Note" + (notesCount + 1).ToString() + "TextBox";

            //Set the binding for the text element.
            Binding textBind = new Binding();
            textBind.Path = new PropertyPath("Note" + (notesCount + 1).ToString());
            textBind.Source = mainPageViewModel;
            textBind.Mode = BindingMode.TwoWay;
            noteTextBox.SetBinding(TextBox.TextProperty, textBind);

            //Add them onto the stack panel.
            MainStackPanel.Children.Add(noteLabelTextBlock);
            MainStackPanel.Children.Add(noteTextBox);         

            //Check if we already have 8, disable the button
            if (((MainStackPanel.Children.Count - 1) / 2) >= 8)
            {
                AddNote.IsEnabled = false;
            }

            //Check if we have more than 2 notes, to re-enable the deleteButton

            else if((((MainStackPanel.Children.Count - 1) / 2)) > 1)
            {
                DeleteNote.IsEnabled = true;
            }
        }

        /// <summary>
        /// Sync the Notes to the band
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SyncNote_Click(object sender, RoutedEventArgs e)
        {
            //Disable the add/remove/sync button.
            AddNote.IsEnabled = false;
            DeleteNote.IsEnabled = false;
            SyncNote.IsEnabled = false;

            //Disable changing text on every textBox
            foreach( var element in MainStackPanel.Children)
            {
                if(element is TextBox)
                {
                    ((TextBox)element).IsReadOnly = true;
                }
            }

            //Get the number of notes.
            int notesCount = (MainStackPanel.Children.Count - 1) / 2;

            //Enable the progressbar
            SyncProgressBar.Visibility = Visibility.Visible;
            SyncProgressBar.IsIndeterminate = true;
            

            //Sync it over.
            await mainPageViewModel.SyncNotesToBandAsync(notesCount);

            //Done. 
            SyncProgressBar.IsIndeterminate = false;
            SyncProgressBar.Visibility = Visibility.Collapsed;

            //Re-Enable the add/remove/sync button.
            AddNote.IsEnabled = true;
            DeleteNote.IsEnabled = true;
            SyncNote.IsEnabled = true;

            //Re-enable the textbox for editing

            //Disable changing text on every textBox
            foreach (var element in MainStackPanel.Children)
            {
                if (element is TextBox)
                {
                    ((TextBox)element).IsReadOnly = false;
                }
            }
        }

        /// <summary>
        /// Delete the note. From bottom up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteNote_Click(object sender, RoutedEventArgs e)
        {
            //we only remove up to the first note.
            if (MainStackPanel.Children.Count == 3)
            {
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Can't remove the first note! Just leave it be. Pls.", "Caution!");
                await dialog.ShowAsync();
                return;
            }

            //Reset the note first.
            ((TextBox)MainStackPanel.Children.Last()).Text = "";

            //Remove the last 2.

            MainStackPanel.Children.RemoveAt(MainStackPanel.Children.Count - 1);
            MainStackPanel.Children.RemoveAt(MainStackPanel.Children.Count - 1);

            //Check if we only have 1 notes left. then disable the button
            if(((MainStackPanel.Children.Count - 1) / 2) <= 1)
            {
                DeleteNote.IsEnabled = false;
            }

            //Check if we have less than 8 notes, to enable the add  button. 
            else if (((MainStackPanel.Children.Count -1) / 2) < 8)
            {
                AddNote.IsEnabled = true;
            }

        }

        /// <summary>
        /// Event handler for the About Button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void About_Click(object sender, RoutedEventArgs e)
        {
            //Show a messege box consist of Application name, Author name and link.
            string appName = "Notes On Band";
            string appVersion = "v1.0";
            string redditLink = "u/AMRAAM_Missiles/";
            string twitterLink = "CodeScottLe";

            //Create the message box for popup.
            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(appName + " " + appVersion, "About");

            dialog.Content += (System.Environment.NewLine + "Reddit: " + redditLink + System.Environment.NewLine + "Twitter: " + twitterLink);


            //Show it.
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
            mainPageViewModel.CurrentBand.CurrentVersion = NotesOnBandEngine.Models.BandVersion.MicrosoftBand2;
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
            mainPageViewModel.CurrentBand.CurrentVersion = NotesOnBandEngine.Models.BandVersion.MicrosoftBand1;
        }
    }
}
