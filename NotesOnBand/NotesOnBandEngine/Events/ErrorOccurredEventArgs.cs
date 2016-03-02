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
        public Exception InnerException { get; set; }
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ErrorOccurredEventArgs()
        {
            Message = "Something went wrong!";
        }

        /// <summary>
        /// Constructor for the ErrorOccurredEventArgs. To be used with the ErrorOccurred Event
        /// </summary>
        /// <param name="message">Error message to pass though</param>
        public ErrorOccurredEventArgs(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Constructor for the ErrorOccurredEventArgs. To be used with the ErrorOccurred Event.
        /// </summary>
        /// <param name="message">Error message to pass through</param>
        /// <param name="innerException">The inner exception to encapsulate with this exception.</param>
        public ErrorOccurredEventArgs(string message, Exception innerException) :this(message)
        {
            InnerException = innerException;
        }
    }
}
