﻿using NotesOnBandEngine.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotesOnBand.Controls
{
    /// <summary>
    /// Code behind for the instance of the BandControl
    /// </summary>
    public sealed partial class BandControl : UserControl
    {
        public readonly static DependencyProperty VersionProperty =
            DependencyProperty.Register("Version", typeof(BandVersion), typeof(BandControl), new PropertyMetadata(BandVersion.MicrosoftBand2));
        
        public BandVersion Version
        {
            get { return (BandVersion)GetValue(VersionProperty); }
            set { SetValue(VersionProperty, value); (DataContext as BandControlViewModel).Version = value; }
        }        

        public BandControl()
        {
            this.InitializeComponent();
        }
    }

    /// <summary>
    /// View Model for the BandControl. 
    /// </summary>
    public class BandControlViewModel : INotifyPropertyChanged
    {
        #region Fields
        public static string DEFAULT_BAND2_BACKGROUND_LOCATION = "/Assets/Band2.png";
        public static string DEFAULT_BAND1_BACKGROUND_LOCATION = "/Assets/Band1.png";

        private static Thickness _band2ScreenGridMargin = new Thickness(99, 70, 102, 67);
        private static Thickness _band1ScreenGridMargin = new Thickness(99, 75, 102, 72);

        private ImageSource _bandImage = new BitmapImage(new Uri(DEFAULT_BAND2_BACKGROUND_LOCATION, UriKind.Relative));
        private BandVersion _version = BandVersion.MicrosoftBand2;

        private Thickness _currentBandGridMargin = _band2ScreenGridMargin;

        #endregion

        #region Properties

        public ImageSource BandImage
        {
            get
            {
                return _bandImage;
            }
            set
            {
                _bandImage = value;
                OnPropertyChanged(nameof(BandImage));
            }
        }

        public BandVersion Version
        {
            get { return _version; }
            set
            {
                _version = value;
                OnPropertyChanged(nameof(Version));

                BandImage = (value == BandVersion.MicrosoftBand2) ? new BitmapImage(new Uri("/Assets/Band2.png", UriKind.Relative))
                    : new BitmapImage(new Uri("/Assets/Band1.png", UriKind.Relative));

                CurrentBandGridMargin = (value == BandVersion.MicrosoftBand2) ? _band2ScreenGridMargin : _band1ScreenGridMargin;
            }
        }

        public Thickness CurrentBandGridMargin
        {
            get { return _currentBandGridMargin; }

            set
            {
                _currentBandGridMargin = value;
                OnPropertyChanged(nameof(CurrentBandGridMargin));
            }

        }
        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        #endregion

        #region Methods

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion




    }
}
