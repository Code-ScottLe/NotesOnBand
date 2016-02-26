using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesOnBand.Models
{
    public class BandNote : INotifyPropertyChanged
    {
        #region Fields
        private string note;

        #endregion

        #region events

        /// <summary>
        /// Implement the INotifyPropertyChanged Interface. Use this to notify the View and others about the property that was changed to perform updates.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Note that the users have typed to be synced with the Band
        /// </summary>
        public string Note
        {
            get
            {
                return note;
            }

            set
            {
                note = value;
                OnPropertyChanged("Note");
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor for the BandNote. Create an instance of BandNote with an empty string.
        /// </summary>
        public BandNote()
        {
            note = string.Empty;
        }

        /// <summary>
        /// Constructor for the BandNote. Create an instance of BandNote with the given string
        /// </summary>
        /// <param name="customNote">Note to be created with this instance</param>
        public BandNote(string customNote)
        {
            note = customNote;
        }
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
