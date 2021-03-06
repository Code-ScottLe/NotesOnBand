﻿using NotesOnBandEngine.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static string DEFAULT_BAND2_BACKGROUND_LOCATION = "ms-appx:///Assets/Band2.png";
        public static string DEFAULT_BAND1_BACKGROUND_LOCATION = "ms-appx:///Assets/Band1.png";

        private static Thickness _band2ScreenGridMargin = new Thickness(99, 43, 102, 41);
        private static Thickness _band1ScreenGridMargin = new Thickness(99, 48, 102, 46);

        private ImageSource _bandImage = new BitmapImage(new Uri(DEFAULT_BAND2_BACKGROUND_LOCATION));
        private BandVersion _version = BandVersion.MicrosoftBand2;

        private Thickness _currentBandGridMargin = _band2ScreenGridMargin;

        private ObservableCollection<BandNote> _notes;

        private Brush _tileHighlightAccentColor = new SolidColorBrush(Windows.UI.Colors.LightCyan);

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

                BandImage = (value == BandVersion.MicrosoftBand2) ? new BitmapImage(new Uri(DEFAULT_BAND2_BACKGROUND_LOCATION))
                    : new BitmapImage(new Uri(DEFAULT_BAND1_BACKGROUND_LOCATION));

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

        public ObservableCollection<BandNote> Notes
        {
            get { return _notes; }
            internal set { _notes = value;  OnPropertyChanged(nameof(Notes)); }
        }

        public Brush TileHighlightAccentColor
        {
            get
            {
                return _tileHighlightAccentColor;
            }

            set
            {
                _tileHighlightAccentColor = value;
                OnPropertyChanged(nameof(TileHighlightAccentColor));
            }
        }


        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public BandControlViewModel()
        {
            Notes = new ObservableCollection<BandNote>();
        }

        #endregion

        #region Methods

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion




    }
}
