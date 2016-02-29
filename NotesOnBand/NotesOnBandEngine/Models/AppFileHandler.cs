using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;

namespace NotesOnBandEngine.Models
{
    /// <summary>
    /// A Singleton, use to handle File I/O traffic for the Store App.
    /// </summary>
    public class AppFileHandler
    {
        
        private static AppFileHandler instance;
        private string pathPrefix = "ms-appx:///";
        private StorageFolder localStorageFolder;

        /// <summary>
        /// Access the current instance of the Handler.
        /// </summary>
        public static AppFileHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppFileHandler();
                }

                return instance;
            }

        }


        /// <summary>
        /// Default Constructor
        /// </summary>
        protected AppFileHandler()
        {
            localStorageFolder = ApplicationData.Current.LocalFolder;
        }

        /// <summary>
        /// Open and return the internal file within the app local folder
        /// </summary>
        /// <param name="fileName">The relative url to the file within the application local folder.</param>
        /// <returns></returns>
        public async Task<StorageFile> GetFileFromLocalFolder(string fileName)
        {
            //Get the file.
            return await localStorageFolder.GetFileAsync(fileName);

        }

        /// <summary>
        /// Create the file with the given name at the local folder.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<StorageFile> CreateFileFromLocalFolder(string fileName)
        {
            return await localStorageFolder.CreateFileAsync(fileName);
        }

        /// <summary>
        /// Convert and get an in-memory stream representation of the storage file.
        /// </summary>
        /// <param name="fileToRead"> File to get stream from</param>
        /// <returns></returns>
        public async Task<Stream> GetReadStreamFromStorageFile(StorageFile fileToRead)
        {
            //Get the RandomAccessStream to the file.
            IRandomAccessStream fileRandomAccessStream = await fileToRead.OpenAsync(FileAccessMode.Read);
            IInputStream inputStream = fileRandomAccessStream.GetInputStreamAt(0);

            using (DataReader reader = new DataReader(inputStream))
            {
                //Read the data bytes by bytes

                //Get the bytes Count
                uint byteCount = (uint)fileRandomAccessStream.Size;

                //create a buffer to hold the data temporarily
                byte[] buffer = new byte[byteCount];

                for(int i = 0; i < byteCount; i++)
                {
                    buffer[i] = reader.ReadByte();              
                }

                //Create and return the stream from the buffer.
                return new MemoryStream(buffer);
            }
        }
    }
}
