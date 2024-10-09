using StarredSeaMUON.Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON
{
    internal class TerminalTheme
    {
        public static TerminalTheme currentTheme = new TerminalTheme();

        public ConsoleTextFormat normal = new ConsoleTextFormat(Color.WhiteSmoke, Color.Black);
        public ConsoleTextFormat highlight = new ConsoleTextFormat(Color.LightGoldenrodYellow, Color.Black, true, true);
        public ConsoleTextFormat transponder = new ConsoleTextFormat(Color.Black, Color.Cyan);
        public ConsoleTextFormat prompt = new ConsoleTextFormat(Color.Yellow, Color.Black, true);
        public ConsoleTextFormat query = new ConsoleTextFormat(Color.Yellow, Color.Black, true);
        public ConsoleTextFormat error = new ConsoleTextFormat(Color.OrangeRed, Color.DarkMagenta, true, false, false, false, false, true);
        public ConsoleTextFormat item = new ConsoleTextFormat(Color.LightYellow, Color.DarkOrange, false, true);
        public ConsoleTextFormat player = new ConsoleTextFormat(Color.LightSkyBlue, Color.DarkSlateBlue, false, true);
        public ConsoleTextFormat room_title = new ConsoleTextFormat(Color.Black, Color.LemonChiffon, true, false, true);
        public ConsoleTextFormat room_desc = new ConsoleTextFormat(Color.LemonChiffon, Color.Black, false, true, false);
        public ConsoleTextFormat room_exits = new ConsoleTextFormat(Color.LemonChiffon, Color.Black, true, false, false, false, false, true);
        public ConsoleTextFormat enemy = new ConsoleTextFormat(Color.OrangeRed, Color.DarkRed);

        public ConsoleTextFormat table_heading = new ConsoleTextFormat(Color.Cyan, Color.DarkBlue);
        public ConsoleTextFormat table_border = new ConsoleTextFormat (Color.DarkBlue, Color.Black);
        public ConsoleTextFormat table_rowline = new ConsoleTextFormat(Color.DarkCyan, Color.Black);
        public ConsoleTextFormat table_key = new ConsoleTextFormat    (Color.Yellow, Color.Black);
        public ConsoleTextFormat table_value = new ConsoleTextFormat  (Color.Orange, Color.Black);

        public static TerminalTheme GetUserTheme(RemotePlayer client)
        {
            return currentTheme;
        }
    }
}
