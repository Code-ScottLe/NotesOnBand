using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesOnBandEngine.Events
{
    /// <summary>
    /// Representing the Event Argument for the ErrorOccurred Event
    /// </summary>
    public class ErrorOccurredEventArgs : EventArgs
    {
        public string Message { get; set; }

        /// <summary>
        /// Default constructor, hidden.
        /// </summary>
        private ErrorOccurredEventArgs()
        {
            
        }

        /// <summary>
        /// Constructor for the ErrorOccurredEventArgs. To be used with the ErrorOccurred Event
        /// </summary>
        /// <param name="message">Error message to pass though</param>
        public ErrorOccurredEventArgs(string message)
        {
            Message = message;
        }
    }
}
