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

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NotesOnBand.Views
{
    public sealed partial class TitleEditorContentDialog : ContentDialog
    {
        //Title result.
        public string NewTitle { get; set; }
        public bool TitleChanged { get; set; }

        public TitleEditorContentDialog()
        {
            this.InitializeComponent();

            TitleChanged = false;
        }


        /// <summary>
        /// Overloaded Constructor,  taking in a string for the previous title.
        /// </summary>
        /// <param name="currentTitle">Previous title</param>
        public TitleEditorContentDialog(string currentTitle) : this()
        {
            NewTitle = currentTitle;
            TitleEditorContentDialogTextBox.Text = NewTitle;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //Flip the switch
            TitleChanged = true;

            //Save back the value
            NewTitle = TitleEditorContentDialogTextBox.Text;

        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //Flip the switch.
            TitleChanged = false;
        }
    }
}
