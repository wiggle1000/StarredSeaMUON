using StarredSeaMUON.Server;
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


        internal void DisplayToUser(ClientConnection client)
        {
            StreamWriter writer = client.writer;
            TerminalTheme theme = client.cTheme;
            MessageSender.SetTextStyle(client, theme.room_title);
            MessageSender.SendCentered(client, title);
            MessageSender.SetTextStyle(client, theme.normal);
            MessageSender.SendText(writer, " ");
            if (hasMeta("image"))
            {
                string filename = getMetaString("image");
                if (filename.ToLower().EndsWith("hurp"))
                    MessageSender.SendAsciiArt(client, filename);
                else
                    MessageSender.SendAsciiArt(client, filename);
            }
            MessageSender.SendText(writer, " ");
            MessageSender.SetTextStyle(client, theme.room_desc);
            MessageSender.SendCentered(client, description);
            MessageSender.SetTextStyle(client, theme.normal);
            MessageSender.SendText(writer, " ");
            if (exits.Count == 0)
            {
                MessageSender.SetTextStyle(client, theme.normal);
                MessageSender.SendCentered(client, "There are no obvious exits.");
            }
            else if (exits.Count == 1)
            {
                MessageSender.SendCentered(client, "There is one obvious exit:");
                foreach (KeyValuePair<string, Tuple<long, string>> a in exits)
                {
                    MessageSender.SendCentered(client, a.Key + " - " + a.Value.Item2);
                }
            }
            else
            {
                MessageSender.SetTextStyle(client, theme.room_exits);
                MessageSender.SendCentered(client, "There are " + exits.Count + " obvious exits:");
                foreach(KeyValuePair<string, Tuple<long, string>> a in exits)
                {
                    MessageSender.SendCentered(client, a.Key + " - " + a.Value.Item2);
                }
            }
            MessageSender.SetTextStyle(client, theme.normal);
        }
    }
}
