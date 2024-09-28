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
        public ConsoleTextFormat prompt = new ConsoleTextFormat(Color.Yellow, Color.Black, true);
        public ConsoleTextFormat error = new ConsoleTextFormat(Color.OrangeRed, Color.DarkMagenta, true, false, false, false, false, true);
        public ConsoleTextFormat item = new ConsoleTextFormat(Color.LightYellow, Color.DarkOrange, false, true);
        public ConsoleTextFormat player = new ConsoleTextFormat(Color.LightSkyBlue, Color.DarkSlateBlue, false, true);
        public ConsoleTextFormat room_title = new ConsoleTextFormat(Color.Black, Color.LemonChiffon, true, false, true);
        public ConsoleTextFormat room_desc = new ConsoleTextFormat(Color.LemonChiffon, Color.Black, false, true, false);
        public ConsoleTextFormat room_exits = new ConsoleTextFormat(Color.LemonChiffon, Color.Black, true, false, false, false, false, true);
        public ConsoleTextFormat enemy = new ConsoleTextFormat(Color.OrangeRed, Color.DarkRed);

        public static TerminalTheme GetUserTheme(ClientConnection client)
        {
            return currentTheme;
        }
    }
}
