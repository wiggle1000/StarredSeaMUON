using StarredSeaMUON.Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON
{
    internal class AsciiArtHelper
    {
        private static Dictionary<char, Color> HURPColors = new Dictionary<char, Color>();


        static AsciiArtHelper()
        {
            HURPColors.Add('x', Color.Black);
            HURPColors.Add('b', Color.DarkBlue);
            HURPColors.Add('g', Color.DarkGreen);
            HURPColors.Add('c', Color.DarkCyan);
            HURPColors.Add('r', Color.DarkRed);
            HURPColors.Add('m', Color.DarkMagenta);
            HURPColors.Add('y', Color.FromArgb(0, 139, 128, 0));
            HURPColors.Add('w', Color.Gray);

            HURPColors.Add('X', Color.DarkGray);
            HURPColors.Add('V', Color.Blue);
            HURPColors.Add('G', Color.Green);
            HURPColors.Add('C', Color.Cyan);
            HURPColors.Add('R', Color.Red);
            HURPColors.Add('M', Color.Magenta);
            HURPColors.Add('Y', Color.LightYellow);
            HURPColors.Add('W', Color.White);
        }

        public static void DisplayArt(ClientConnection client, int width, int height, string body)
        {
            int i = 0;
            for (int y = 0; y < height; y++)
            {
                for (int off = 0; off < (client.termWidth / 2) - (width / 2); off++)
                {
                    client.writer.Write(" ");
                }
                for (int x = 0; x < width; x++)
                {
                    char fgStr = body.Substring(i, 1)[0];
                    char bgStr = body.Substring(i + 1, 1)[0];
                    client.writer.Write(ConsoleTextFormat.GetTelnetColorOnlyCode(client.colorSupport, HURPColors[fgStr], HURPColors[bgStr]));
                    client.writer.Write(body.Substring(i + 2, 1));
                    client.writer.Flush();
                    i += 3;
                }
                MessageSender.ResetTextStyle(client);
                client.writer.Write("\n");
            }
            client.writer.Flush();
        }
    }
}
