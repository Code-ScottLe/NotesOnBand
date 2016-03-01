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

        //Nope.
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Add a new notes on to the view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddNote_Click(object sender, RoutedEventArgs e)
        {
            //Ask how many notes we have so far.
            int notesCount = ((MainStackPanel.Children.Count / 2) - 1) + 1;

            //We can't add more than 8.
            if (notesCount >= 8)
            {
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Can't have more than 8 notes!");
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
            noteTextBox.Text = "Haha";
            noteTextBox.Margin = new Thickness(10, 5, 10, 0);
            noteTextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            noteTextBox.AcceptsReturn = true;
            noteTextBox.TextWrapping = TextWrapping.Wrap;
            noteTextBox.Name = "Note" + (notesCount + 1).ToString() + "TextBox";

            //Add them onto the stack panel.
            MainStackPanel.Children.Add(noteLabelTextBlock);
            MainStackPanel.Children.Add(noteTextBox);         
        }

        /// <summary>
        /// Sync the Notes to the band
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyncNote_Click(object sender, RoutedEventArgs e)
        {

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
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Can't remove the first note! Just leave it be. Pls.");
                await dialog.ShowAsync();
                return;
            }
            //Remove the last 2.
            MainStackPanel.Children.RemoveAt(MainStackPanel.Children.Count - 1);
            MainStackPanel.Children.RemoveAt(MainStackPanel.Children.Count - 1);

        }
    }
}
