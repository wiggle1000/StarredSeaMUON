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

        /*public static Dictionary<string, string> ParseMeta(string e) //can probably be optimized a lot
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            string[] parts = e.ReplaceLineEndings("\n").Split(";");
            foreach (string _s in parts)
            {
                string s = _s;
                if (_s.Contains("\n")) //always use last line if theres a newline.
                {
                    string[] sp = _s.Split('\n');
                    s = sp[sp.Length - 1];
                }
                int eq = s.IndexOf('=');
                if (eq == -1 || eq == 0 || eq == s.Length - 1) continue;
                string[] halves = s.Split('=');
                metadata.Add(halves[0], halves[1]);
                Console.WriteLine("Parsed meta: " + halves[0] + " -> " + halves[1]);
            }
            return metadata;
        }

        public static string UnparseMeta(Dictionary<string, string> meta) //can probably be optimized a lot
        {
            string output = "";
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            foreach (string key in metadata.Keys)
            {
                string val = metadata[key];
                output += key;
                output += "=";
                output += val;
                output += ";\n";
            }
            if(output.EndsWith("\n"))
            {
                output = output.Substring(0, output.Length - 1);
            }
            return output;
        }*/

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
