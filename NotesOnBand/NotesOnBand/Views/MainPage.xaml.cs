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

            //Create the textBox for the notes
            TextBox noteTextBox = new TextBox();
            noteTextBox.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            //noteTextBox.Margin = new Thickness(10, 5, 10, 0);
            noteTextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            noteTextBox.VerticalAlignment = VerticalAlignment.Stretch;
            noteTextBox.AcceptsReturn = true;
            noteTextBox.TextWrapping = TextWrapping.Wrap;
            noteTextBox.Name = "Note" + (notesCount + 1).ToString() + "TextBox";          

            //Set the binding for the text element.
            Binding textBind = new Binding();
            textBind.Path = new PropertyPath("Note" + (notesCount + 1).ToString());
            textBind.Source = mainPageViewModel;
            textBind.Mode = BindingMode.TwoWay;
            noteTextBox.SetBinding(TextBox.TextProperty, textBind);


            //Create the delete button.
            Button deleteButton = new Button();
            deleteButton.Name = "Note" + (notesCount + 1).ToString() + "DeleteButton";
            deleteButton.Margin = new Thickness(0);
            deleteButton.Padding = new Thickness(0);
            deleteButton.HorizontalAlignment = HorizontalAlignment.Right;
            deleteButton.VerticalAlignment = VerticalAlignment.Center;

            //The button will have the cancel (X) symbol.
            deleteButton.Content = new SymbolIcon(Symbol.Cancel);

            //Add in the event handler for the click event
            deleteButton.Click += DeleteButton_Click;


            //Create the Grid for the note and the button.
            Grid noteGrid = new Grid();
            noteGrid.Margin = new Thickness(10, 5, 10, 0);
            noteGrid.Name = "Note" + (notesCount + 1).ToString() + "Grid";

            //Create 2 column definitions to hold the textbox and the button
            //We reuse the one that was defined in the XAML for ease of access.
            ColumnDefinition noteColumnDefinition = new ColumnDefinition() { Width = Note1Grid.ColumnDefinitions[0].Width };

            ColumnDefinition noteDeleteButtonColumnDefinition = new ColumnDefinition() { Width = Note1Grid.ColumnDefinitions[1].Width };

            //Add them on to the ColumnsDefinition of the new grid
            noteGrid.ColumnDefinitions.Add(noteColumnDefinition);
            noteGrid.ColumnDefinitions.Add(noteDeleteButtonColumnDefinition);

            //Add the TextBox and the Button to be the child of the note grid
            noteGrid.Children.Add(noteTextBox);
            noteGrid.Children.Add(deleteButton);

            //Set the column of the note textbox to 0.
            Grid.SetColumn(noteTextBox, 0);

            //Set the column of the button to 1
            Grid.SetColumn(deleteButton, 1);
            


            //Add them onto the stack panel.
            MainStackPanel.Children.Add(noteLabelTextBlock);
            MainStackPanel.Children.Add(noteGrid);         

            //Check if we already have 8, disable the button
            if (((MainStackPanel.Children.Count - 1) / 2) >= 8)
            {
                AddNote.IsEnabled = false;
            }

            //Check if we have more than 2 notes, to re-enable the deleteButton (in app-bar) and delete button right next to the note.

            else if((((MainStackPanel.Children.Count - 1) / 2)) > 1)
            {
                DeleteNote.IsEnabled = true;
                Note1DeleteButton.IsEnabled = true;
            }
        }


        /// <summary>
        /// Event handler for clicking the button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
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
            try
            {
                await mainPageViewModel.SyncNotesToBandAsync(notesCount);
            }
            

            catch (Exception ex)
            {
                //Something is wrong. Display message
                string title = "Whoops :( Something is wrong";
                string message = ex.Message;

                if(ex.InnerException != null)
                {
                    message += System.Environment.NewLine;
                    message += "Inner Exception:";
                    message += ex.InnerException.Message;
                }

                await CreatePopupMessage(message, title);
            }

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
            ((MainStackPanel.Children.Last() as Grid).Children[0] as TextBox).Text = "";

            //Remove the last 2.

            MainStackPanel.Children.RemoveAt(MainStackPanel.Children.Count - 1);
            MainStackPanel.Children.RemoveAt(MainStackPanel.Children.Count - 1);

            //Check if we only have 1 notes left. then disable the button
            if(((MainStackPanel.Children.Count - 1) / 2) <= 1)
            {
                DeleteNote.IsEnabled = false;
                Note1DeleteButton.IsEnabled = false;
            }

            //Check if we have less than 8 notes, to enable the add  button. 
            else if (((MainStackPanel.Children.Count -1) / 2) < 8)
            {
                AddNote.IsEnabled = true;
            }

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


        /// <summary>
        /// Event handler for the SEtting click, send the user to the setting page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingPage));
        }
    }
}
