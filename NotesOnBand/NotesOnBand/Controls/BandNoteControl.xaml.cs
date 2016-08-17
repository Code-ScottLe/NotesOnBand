using NotesOnBandEngine.Models;
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
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotesOnBand.Controls
{
    public sealed partial class BandNoteControl : UserControl
    {

        #region Dependencies Properties

        public static readonly DependencyProperty CurrentBandVersionProperty =
            DependencyProperty.Register(nameof(CurrentBandVersion), typeof(BandVersion), typeof(BandNoteControl), new PropertyMetadata(BandVersion.MicrosoftBand2));

        public BandVersion CurrentBandVersion
        {
            get
            {
                return (BandVersion)GetValue(CurrentBandVersionProperty);
            }

            set
            {
                SetValue(CurrentBandVersionProperty, value);
            }
        }

        public static readonly DependencyProperty CurrentNoteProperty =
            DependencyProperty.Register(nameof(CurrentNote), typeof(BandNote), typeof(BandNoteControl), new PropertyMetadata(null));

        public BandNote CurrentNote
        {
            get
            {
                return (BandNote)GetValue(CurrentNoteProperty);
            }

            set
            {
                SetValue(CurrentNoteProperty, value);
            }
        }

        #endregion


        public BandNoteControl()
        {
            this.InitializeComponent();

           
        }
    }
}
