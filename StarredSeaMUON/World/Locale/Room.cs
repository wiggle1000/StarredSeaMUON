using StarredSeaMUON.Server;
using StarredSeaMUON.Util;
using StarredSeaMUON.World.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.World.Locale
{
    internal class Room : MetadataHaver
    {
        public float size = 1;
        public string name = "room"; //shorthand dev name
        public string title = "Room"; //title shown to user
        public string description = "A room."; //description shown to user
        public RoomMaterial material = RoomMaterial.Generic;

        public List<WorldObject> objects = new List<WorldObject>();
        /// <summary>
        /// key: direction/exit name
        /// value: tuple of target room id and exit description
        /// </summary>
        public Dictionary<string, Tuple<long, string>> exits = new Dictionary<string, Tuple<long, string>>();

        internal void parseExits(string e)
        {
            string[] parts = e.ReplaceLineEndings("").Split(";");
            foreach(string s in parts)
            {//TODO: error checking
                int eq = s.IndexOf('=');
                if (eq == -1 || eq == 0 || eq == s.Length-1) continue;
                string[] halves = s.Split('=');
                int col = halves[1].IndexOf(':');
                if (col == -1 || col == 0 || col == halves[1].Length - 1) continue;
                long l = -1;
                if (!long.TryParse(halves[1].Substring(0, col), out l)) continue;
                exits.Add(halves[0].ToLower(), new Tuple<long, string>(l, halves[1].Substring(col+1)));
                Console.WriteLine("Parsed exit: 1[" + halves[0].ToLower() + "] -> " + l.ToString());
            }
        }


        internal void DisplayToUser(RemotePlayer client)
        {
            StreamWriter writer = client.telnet.writer;
            TerminalTheme theme = client.options.terminalTheme;
            client.OutputCentered(title, theme.room_title);
            client.Output(" ", theme.normal);
            if (hasMeta("image"))
            {
                string filename = getMetaString("image");
                if (filename.ToLower().EndsWith("hurp"))
                    AsciiArtHelper.SendAsciiArt(client, filename);
                else
                    AsciiArtHelper.SendAsciiArt(client, filename);
            }
            client.Output(" ", theme.normal);
            client.Output(description, theme.room_desc);
            client.Output(" ", theme.normal);
            if (exits.Count == 0)
            {
                client.Output("There are no obvious exits.", theme.room_exits);
            }
            else if (exits.Count == 1)
            {
                client.OutputCentered("There is one obvious exit:", theme.room_exits);
                foreach (KeyValuePair<string, Tuple<long, string>> a in exits)
                {
                    client.OutputCentered(a.Key + " - " + a.Value.Item2, theme.room_exits);
                }
            }
            else
            {
                client.OutputCentered("There are " + exits.Count + " obvious exits:", theme.room_exits);
                foreach(KeyValuePair<string, Tuple<long, string>> a in exits)
                {
                    client.OutputCentered(a.Key + " - " + a.Value.Item2, theme.room_exits);
                }
            }
        }
    }
}
