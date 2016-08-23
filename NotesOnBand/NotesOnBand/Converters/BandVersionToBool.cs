using NotesOnBandEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace NotesOnBand.Converters
{
    public class BandVersionToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BandVersion version = (BandVersion)value;

            return (version == BandVersion.MicrosoftBand2) ? true : false;          
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool isOn = (bool)value;

            return isOn ? BandVersion.MicrosoftBand2 : BandVersion.MicrosoftBand1;
        }
    }

}
