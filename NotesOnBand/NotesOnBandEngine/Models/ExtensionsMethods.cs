using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesOnBandEngine.Models
{
    public static class ExtensionsMethods
    {
        public static List<T> ConvertToList<T>(this ObservableCollection<T> collection)
        {
            List<T> listy = new List<T>();
            //Convert it to the enumerator.
            foreach(var item in collection)
            {
                listy.Add(item);
            }

            return listy;
        }
    }
}
