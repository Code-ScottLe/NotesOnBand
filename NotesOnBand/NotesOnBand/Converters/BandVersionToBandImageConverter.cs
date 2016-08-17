using NotesOnBandEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace NotesOnBand.Converters
{
    public class BandVersionToBandImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BitmapImage bandImage = new BitmapImage(new Uri("/Assets/Band1.png", UriKind.Relative));

            if(value != null)
            {
                return null;
            }

            BandVersion bandVersion = (BandVersion)value;

            if(bandVersion == BandVersion.MicrosoftBand2)
            {
                bandImage = new BitmapImage(new Uri("/Assets/Band2.png", UriKind.Relative));
            }

            return bandImage;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
