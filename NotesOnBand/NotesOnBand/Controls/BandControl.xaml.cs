using NotesOnBandEngine.Models;
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

    public class BandControlViewModel : INotifyPropertyChanged
    {
        private ImageSource _bandImage = new BitmapImage(new Uri("/Assets/Band2.png", UriKind.Relative));
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

        private BandVersion _version = BandVersion.MicrosoftBand2;
        public BandVersion Version
        {
            get { return _version; }
            set
            {
                _version = value;
                OnPropertyChanged(nameof(Version));

                BandImage = (value == BandVersion.MicrosoftBand2) ? new BitmapImage(new Uri("/Assets/Band2.png", UriKind.Relative)) 
                    : new BitmapImage(new Uri("/Assets/Band1.png", UriKind.Relative));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
