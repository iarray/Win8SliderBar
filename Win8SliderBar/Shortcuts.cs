using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Win8SliderBar
{
    public class Shortcuts
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string FilePath { get; set; }

        public Shortcuts()
        {
            Name = string.Empty;
            Type = string.Empty;
            FilePath = string.Empty;
        }

        public Shortcuts(string name, string type, string path)
        {
            this.Name = name;
            this.Type = type;
            this.FilePath = path;
        }
    }
}
