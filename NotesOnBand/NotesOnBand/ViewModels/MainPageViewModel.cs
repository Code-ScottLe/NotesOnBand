using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;
using NotesOnBand.Models;

namespace NotesOnBand.ViewModels
{
    /// <summary>
    /// ViewModel for the MainPage (MainPage.xaml)
    /// </summary>
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region Fields

        private Band currentBand;
        private List<string> notesList;

        #endregion

        #region events

        /// <summary>
        /// Implement the INotifyPropertyChanged Interface. Use this to notify the View about the property that was changed to perform updates.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        #endregion

        #region Properties

        public Band CurrentBand
        {
            get
            {
                return currentBand;
            }

            protected set
            {
                currentBand = value;
                OnPropertyChanged("CurrentBand");
            }
        }

        public string Note1
        {
            get
            {
                return notesList[0];
            }

            set
            {
                notesList[0] = value;
                OnPropertyChanged("Note1");

            }
        }

        public string Note2
        {
            get
            {
                return notesList[1];
            }

            set
            {
                notesList[1] = value;
                OnPropertyChanged("Note2");

            }
        }

        public string Note3
        {
            get
            {
                return notesList[2];
            }

            set
            {
                notesList[2] = value;
                OnPropertyChanged("Note3");

            }
        }

        public string Note4
        {
            get
            {
                return notesList[3];
            }

            set
            {
                notesList[3] = value;
                OnPropertyChanged("Note4");

            }
        }

        public string Note5
        {
            get
            {
                return notesList[4];
            }

            set
            {
                notesList[4] = value;
                OnPropertyChanged("Note5");

            }
        }

        public string Note6
        {
            get
            {
                return notesList[5];
            }

            set
            {
                notesList[5] = value;
                OnPropertyChanged("Note6");

            }
        }

        public string Note7
        {
            get
            {
                return notesList[6];
            }

            set
            {
                notesList[6] = value;
                OnPropertyChanged("Note7");

            }
        }

        public string Note8
        {
            get
            {
                return notesList[7];
            }

            set
            {
                notesList[7] = value;
                OnPropertyChanged("Note8");

            }
        }


        #endregion

        #region Constructors

        #endregion

        #region Methods
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



        #endregion

    }
}
