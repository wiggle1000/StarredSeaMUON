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
            HURPColors.Add('y', Color.FromArgb(139, 128, 0));
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

        private static void DisplayArt(RemotePlayer client, int width, int height, string body)
        {
            int i = 0;
            for (int y = 0; y < height; y++)
            {
                for (int off = 0; off < (client.options.termSize.Width / 2) - (width / 2); off++)
                {
                    client.telnet.SendEncoded(" ");
                }
                for (int x = 0; x < width; x++)
                {
                    char fgStr = body.Substring(i, 1)[0];
                    char bgStr = body.Substring(i + 1, 1)[0];
                    client.telnet.SendEncoded(ConsoleTextFormat.GetTelnetColorOnlyCode(client.options.colorSupport, HURPColors[fgStr], HURPColors[bgStr]));
                    client.telnet.SendEncoded(body.Substring(i + 2, 1));
                    i += 3;
                }
                client.telnet.SendEncoded(ConsoleTextFormat.GetResetCode(client.options.colorSupport));
                client.telnet.SendEncoded("\n");
            }
        }

        public static void SendAsciiArt(RemotePlayer client, string fileName)
        {
            StreamWriter writer = client.telnet.writer;
            if (File.Exists("resources/ascii/" + fileName + ".hurp"))
            {
                string dat = File.ReadAllText("resources/ascii/" + fileName + ".hurp").ReplaceLineEndings("");
                if (!dat.StartsWith("{")) return;
                int headerEndPos = dat.IndexOf("}");
                if (headerEndPos == -1) return;
                string header = dat.Substring(1, headerEndPos - 1);
                string[] headerParts = header.Split(",");
                if (headerParts.Length != 2) return;
                int w, h = 0;
                if (!int.TryParse(headerParts[0], out w) || !int.TryParse(headerParts[1], out h)) return;
                string body = dat.Substring(headerEndPos + 1);
                AsciiArtHelper.DisplayArt(client, w, h, body);
                //writer.WriteLine("<<[nm.msg.asciiArt.hurp:" + File.ReadAllText("resources/ascii/" + fileName + ".hurp").ReplaceLineEndings("") + "]>>");
                //writer.Flush();
            }
            else if (File.Exists("resources/ascii/" + fileName + ".txt"))
            {
                client.Output(File.ReadAllText("resources/ascii/" + fileName + ".txt").ReplaceLineEndings("\r\n"));
            }
            else
            {
                Logger.LogError("Tried to send nonexistant ascii art: " + fileName);
            }
        }
    }
}
