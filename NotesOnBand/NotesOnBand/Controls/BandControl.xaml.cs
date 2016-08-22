using NotesOnBandEngine.Models;
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
        public readonly static DependencyProperty VersionProperty =
            DependencyProperty.Register(nameof(Version), typeof(BandVersion), typeof(BandControl), new PropertyMetadata(BandVersion.MicrosoftBand2, (s, e) =>
            {
                DependencyChanged?.Invoke(null, "Version");
            }));
       
        public BandVersion Version
        {
            get { return (BandVersion)GetValue(VersionProperty); }
            set { SetValue(VersionProperty, value);}
        }

        public static readonly DependencyProperty NotesProperty =
            DependencyProperty.Register(nameof(Notes), typeof(ObservableCollection<BandNote>), typeof(BandControl), new PropertyMetadata(new ObservableCollection<BandNote>(), (s, e) =>
            {
                DependencyChanged?.Invoke(null, "Notes");
            }));

        public ObservableCollection<BandNote> Notes
        {
            get { return (ObservableCollection<BandNote>)GetValue(NotesProperty); }
            set { SetValue(NotesProperty, value); }
        }

        private static EventHandler<string> DependencyChanged;        

        public BandControl()
        {
            this.InitializeComponent();

            //event stuffs
            DependencyChanged += OnVersionPropertyChanged;

            //binding stuffs
            Binding myBinding = new Binding();
            myBinding.Path = new PropertyPath("Notes");
            myBinding.Source = (DataContext as BandControlViewModel);
            myBinding.Mode = BindingMode.TwoWay;
            this.SetBinding(NotesProperty, myBinding);

        }

        private void OnVersionPropertyChanged(object sender, string arg)
        {
            if(arg == "Version")
            {
                (DataContext as BandControlViewModel).Version = Version;
            }

            else if(arg == "Notes")
            {
                (DataContext as BandControlViewModel).Notes = new ObservableCollection<BandNote>(Notes);
            }
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

        private static Thickness _band2ScreenGridMargin = new Thickness(99, 70, 102, 67);
        private static Thickness _band1ScreenGridMargin = new Thickness(99, 75, 102, 72);

        private ImageSource _bandImage = new BitmapImage(new Uri(DEFAULT_BAND2_BACKGROUND_LOCATION));
        private BandVersion _version = BandVersion.MicrosoftBand2;

        private Thickness _currentBandGridMargin = _band2ScreenGridMargin;

        private ObservableCollection<BandNote> _notes;
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
