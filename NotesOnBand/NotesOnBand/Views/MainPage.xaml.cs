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
        /// Add a new notes on to the view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddNote_Click(object sender, RoutedEventArgs e)
        {
            //Ask how many notes we have so far.
            int notesCount = MainStackPanel.Children.Count -1;

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
            //Button deleteButton = new Button();
            //deleteButton.Name = "Note" + (notesCount + 1).ToString() + "DeleteButton";
            //deleteButton.Margin = new Thickness(0);
            //deleteButton.Padding = new Thickness(0);
            //deleteButton.HorizontalAlignment = HorizontalAlignment.Right;
            //deleteButton.VerticalAlignment = VerticalAlignment.Center;
            //deleteButton.Tag = (notesCount + 1);

            ////The button will have the cancel (X) symbol.
            //deleteButton.Content = new SymbolIcon(Symbol.Cancel);

            ////Add in the event handler for the click event
            //deleteButton.Click += DeleteButton_Click;


            //Create the Grid for the note and the button.
            Grid noteGrid = new Grid();
            noteGrid.Margin = new Thickness(10, 5, 10, 0);
            noteGrid.Name = "Note" + (notesCount + 1).ToString() + "Grid";

            //Create 2 column definitions to hold the textbox and the button
            //We reuse the one that was defined in the XAML for ease of access.
            //ColumnDefinition noteColumnDefinition = new ColumnDefinition() { Width = Note1Grid.ColumnDefinitions[0].Width };

            //ColumnDefinition noteDeleteButtonColumnDefinition = new ColumnDefinition() { Width = Note1Grid.ColumnDefinitions[1].Width };

            //Add them on to the ColumnsDefinition of the new grid
            //noteGrid.ColumnDefinitions.Add(noteColumnDefinition);
            //noteGrid.ColumnDefinitions.Add(noteDeleteButtonColumnDefinition);

            //Add the TextBox and the Button to be the child of the note grid
            noteGrid.Children.Add(noteTextBox);
            //noteGrid.Children.Add(deleteButton);

            //Set the column of the note textbox to 0.
            Grid.SetColumn(noteTextBox, 0);

            //Set the column of the button to 1
            //Grid.SetColumn(deleteButton, 1);


            //Create a new stack panel to house the label and the actual note grid
            StackPanel noteStackpanel = new StackPanel();
            noteStackpanel.Name = "Note" + (notesCount + 1).ToString() + "StackPanel";
            noteStackpanel.Children.Add(noteLabelTextBlock);
            noteStackpanel.Children.Add(noteGrid);
            noteStackpanel.Opacity = 0.0;

            //Try to animate the opacity.
            Storyboard myStoryBoard = new Storyboard();
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            Storyboard.SetTarget(myDoubleAnimation, noteStackpanel);
            Storyboard.SetTargetProperty(myDoubleAnimation, "Opacity");
            myDoubleAnimation.From = 0.0;
            myDoubleAnimation.To = 1.0;
            myDoubleAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));   //300ms
            myStoryBoard.Children.Add(myDoubleAnimation);

            //Add them onto the main stack panel.
            MainStackPanel.Children.Add(noteStackpanel);

            //Now, try to animate
            myStoryBoard.Begin();

            //Check if we already have 8, disable the button
            if ((MainStackPanel.Children.Count - 1) >= 8)
            {
                AddNote.IsEnabled = false;
            }

            //Check if we have more than 2 notes, to re-enable the deleteButton (in app-bar) and delete button right next to the note.

            else if (((MainStackPanel.Children.Count - 1)) > 1)
            {
                DeleteNote.IsEnabled = true;
                //Note1DeleteButton.IsEnabled = true;
            }
        }


        /// <summary>
        /// Event handler for clicking the delete button for individual note
        /// </summary>
        /// <param name="sender">The delete button that the user pressed.</param>
        /// <param name="e"></param>
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //Find out the actual note that we are deleting
            int noteToDeleteNumber = (int)(sender as Button).Tag;

            //Now we get the actual note to delete, remember, that in the mainStack , every note is staying on their corresponded index
            //I.E : Note1 is at index 1 of the MainStack's Children list, Note 2 is at index 2 of the MainStack's Children list...
            //Because we have the header stuffs as index 0 on the main stack.

            //Now, get the note's stackpanel.
            StackPanel noteStackPanel = MainStackPanel.Children[noteToDeleteNumber] as StackPanel;

            if (noteStackPanel == null)
            {
                //Something went wrong again, noteStackPanel was not pulled out correctly.
                //C# 6 syntax for string interpolation
                await CreatePopupMessage($"Can't convert MainStackPanel.Children[{noteToDeleteNumber}] to a StackPanel", "Whoops, something went wrong :(");

                return;
            }

            //We animate the hiding.
            Storyboard myStoryBoard = new Storyboard();
            DoubleAnimation hidingDoubleAnimation = new DoubleAnimation();
            Storyboard.SetTarget(hidingDoubleAnimation, noteStackPanel);
            Storyboard.SetTargetProperty(hidingDoubleAnimation, "Opacity");

            //We animate it to run for 0.4ms
            hidingDoubleAnimation.From = 1.0;
            hidingDoubleAnimation.To = 0.0;
            hidingDoubleAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 400));

            //Add it to the storyboard
            myStoryBoard.Children.Add(hidingDoubleAnimation);

            //Begin animate
            myStoryBoard.Begin();

            //Wait for the animation to complete
            await Task.Delay(400);

            //remove the thing
            MainStackPanel.Children.Remove(noteStackPanel);         

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
            if (MainStackPanel.Children.Count == 2)
            {
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Can't remove the first note! Just leave it be. Pls.", "Caution!");
                await dialog.ShowAsync();
                return;
            }

            //We animate hiding it. 
            Storyboard myStoryBoard = new Storyboard();
            DoubleAnimation myAnimation = new DoubleAnimation();

            //Set the target element to be the last stack panel
            Storyboard.SetTarget(myAnimation, (StackPanel)MainStackPanel.Children.Last());
            Storyboard.SetTargetProperty(myAnimation, "Opacity");

            //Set the animation to run from fully visible to not visible in 0.3 sec
            myAnimation.From = 1.0;
            myAnimation.To = 0.0;
            myAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));

            //Now run.
            myStoryBoard.Children.Add(myAnimation);
            myStoryBoard.Begin();

            //Because StoryBoard.Begin() is a fire and forget, delay the removal for 400ms so the animation can be shown.
            await Task.Delay(300);

            //Reset the note first.
            (((MainStackPanel.Children.Last() as StackPanel).Children[1] as Grid).Children[0] as TextBox).Text = "";

            //Remove the last one
            MainStackPanel.Children.RemoveAt(MainStackPanel.Children.Count - 1);

            //Check if we only have 1 notes left. then disable the button
            if((MainStackPanel.Children.Count - 1)  <= 1)
            {
                DeleteNote.IsEnabled = false;
                //Note1DeleteButton.IsEnabled = false;
            }

            //Check if we have less than 8 notes, to enable the add  button. 
            else if ((MainStackPanel.Children.Count -1)  < 8)
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
