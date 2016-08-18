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
    public class BandTwoPageVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            var me = (BandVersion) value;

            return (me == BandVersion.MicrosoftBand2) ? Visibility.Visible : Visibility.Collapsed;


        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BandOnePageVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            var me = (BandVersion)value;

            return (me == BandVersion.MicrosoftBand1) ? Visibility.Visible : Visibility.Collapsed;


        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
