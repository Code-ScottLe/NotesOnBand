using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace NotesOnBandEngine.Models
{
    public class BandNote : INotifyPropertyChanged
    {

        private string content;
        private string title;
        private Brush titleColor;

        public string Content
        {
            get
            {
                return content;
            }

            set
            {
                content = value;
                OnPropertyChanged("Content");
            }

        }
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        public Brush TitleColor
        {
            get
            {
               if(titleColor == null)
                {
                    titleColor = new SolidColorBrush(Windows.UI.Colors.LightCyan);
                }

                return titleColor;
            }

            set
            {
                titleColor = value;
                OnPropertyChanged(nameof(TitleColor));
            }
        }

        /// <summary>
        /// Implement the INotifyPropertyChanged Interface. Use this to notify the View about the property that was changed to perform updates.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Fire up the PropertyChanged event and notify all the listener about the changed property.
        /// </summary>
        /// <param name="propertyName">Name of the property that was changed.</param>
        public void OnPropertyChanged(string propertyName)
        {

            //Make sure we do have a listener.
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
