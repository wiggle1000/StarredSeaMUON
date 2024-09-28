using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.World
{
    internal class MetadataHaver
    {
        public Dictionary<string, string> metadata = new Dictionary<string, string>();

        public void ParseMeta(string e) //can probably be optimized a lot
        {
            string[] parts = e.ReplaceLineEndings("\n").Split(";");
            foreach (string _s in parts)
            {
                string s = _s;
                if(_s.Contains("\n")) //always use last line if theres a newline. also allows for comments after line ends!
                {
                    string[] sp = _s.Split('\n');
                    s = sp[sp.Length-1];
                }
                int eq = s.IndexOf('=');
                if (eq == -1 || eq == 0 || eq == s.Length - 1) continue;
                string[] halves = s.Split('=');
                metadata.Add(halves[0], halves[1]);
                Console.WriteLine("Parsed meta: " + halves[0] + " -> " + halves[1]);
            }
        }

        public bool hasMeta(string name)
        {
            return metadata.ContainsKey(name);
        }
        public string getMetaString(string name)
        {
            return (string)metadata[name];
        }
        public long? getMetaLong(string name)
        {
            long l;
            if (long.TryParse(metadata[name], out l))
                return l;
            return null;
        }
    }
}
