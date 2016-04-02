using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NotesOnBandEngine.Models
{
    public class XMLHandler
    {
        #region Fields
        private static XMLHandler currentInstance;


        /// <summary>
        /// The XML Document that hold all the data 
        /// </summary>
        private XDocument syncedNotesXDocument;


        private string savedFileName;

        #endregion

        #region Properties

        /// <summary>
        /// Get the singleton instance
        /// </summary>
        public static XMLHandler Instance
        {
            get
            {
                if (currentInstance == null)
                {
                    currentInstance = new XMLHandler();
                }

                return currentInstance;
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor, hidden
        /// </summary>
        protected XMLHandler()
        {
            syncedNotesXDocument = new XDocument();
            savedFileName = Windows.ApplicationModel.Package.Current.PublisherDisplayName + Windows.ApplicationModel.Package.Current.DisplayName + Windows.ApplicationModel.Package.Current.Id + ".xml";
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        public async Task SaveToXMLAsync(List<string> notes)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> LoadFromXMLAsync()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
