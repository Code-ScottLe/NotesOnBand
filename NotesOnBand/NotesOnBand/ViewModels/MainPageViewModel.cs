﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesOnBand.ViewModels
{
    /// <summary>
    /// ViewModel for the MainPage (MainPage.xaml)
    /// </summary>
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region Fields

        #endregion

        #region events

        /// <summary>
        /// Implement the INotifyPropertyChanged Interface. Use this to notify the View about the property that was changed to perform updates.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        #endregion

        #region Properties

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
