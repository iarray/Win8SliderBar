using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Win8SliderBar
{
    [Serializable]
    public class ShortcutsCollection : ObservableCollection<Shortcuts>
    {

        public ShortcutsCollection()
        { 
        }

        public void SaveData(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(ShortcutsCollection));
                formatter.Serialize(fs, this);
            }
        }

        public static ShortcutsCollection LoadDataFromFile(string path)
        {
            ShortcutsCollection sc; 
            using (FileStream fs = new FileStream(path, FileMode.Open)) 
            {
                XmlSerializer formatter = new XmlSerializer(typeof(ShortcutsCollection));
                sc = (ShortcutsCollection)formatter.Deserialize(fs);
            }
            return sc;
        }
    }
}
