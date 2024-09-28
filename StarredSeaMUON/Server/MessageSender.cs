using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Server
{
    internal class MessageSender
    {
        public static void SetTextStyle(ClientConnection client, ConsoleTextFormat f)
        {
            client.writer.Write(f.GetTelnetFormatCode(client.colorSupport));
            client.writer.Flush();
        }
        public static void ResetTextStyle(ClientConnection client)
        {
            client.writer.Write("\x1b[0m");
            client.writer.Flush();
        }
        public static void SetTermColors(ClientConnection client, Color fg, Color bg)
        {
            client.writer.Write("\x1b[0m");
            client.writer.Flush();
        }
        /*public static void SendDat(StreamWriter writer, string id, string content = "")
        {
            writer.WriteLine("<<[" + id + ":" + content + "]>>");
            writer.Flush();
        }*/
        public static void SendAsciiArt(ClientConnection client, string fileName)
        {
            StreamWriter writer = client.writer;
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
                writer.WriteLine(File.ReadAllText("resources/ascii/" + fileName + ".txt").ReplaceLineEndings("\n"));
                writer.Flush();
            }
            else
            {
                Logger.LogError("Tried to send nonexistant ascii art: " + fileName);
            }
        }
        public static void ClearHistory(ClientConnection client)
        {
            //teehee! :3c
            client.writer.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
            
            client.writer.WriteLine("\x1B[2J");
            client.writer.Flush();
        }
        public static void SendDat(StreamWriter writer, string id, bool content)
        {
            writer.WriteLine("<<[" + id + ":" + (content?"true":"false") + "]>>");
            writer.Flush();
        }
        public static void SendText(StreamWriter writer, string msg)
        {
            writer.WriteLine(msg);
            writer.Flush();
        }
        public static void SendError(ClientConnection client, string msg, bool centered = false, bool silent = false)
        {
            StreamWriter writer = client.writer;

            MessageSender.SetTextStyle(client, client.cTheme.error);
            if (centered)
                SendCentered(client, msg);
            else
                SendText(writer, msg);
            MessageSender.SetTextStyle(client, client.cTheme.normal);
            //if (!silent)
                //MessageSender.SendDat(writer, "nm.sound.playOgg", "sound/system/error");
        }
        public static void SetCursorText(StreamWriter writer, string msg)
        {
            //SendDat(writer, "nm.term.cursorText", msg);
        }
        public static void SendCentered(ClientConnection client, string msg)
        {
            for(int i = 0; i < (client.termWidth/2) - (msg.Length/2); i++)
            {
                client.writer.Write(" ");
            }
            client.writer.Write(msg);

            for (int i = 0; i < (client.termWidth / 2) - (msg.Length / 2); i++)
            {
                client.writer.Write(" ");
            }
            client.writer.Write("\n");
            //SendDat(writer, "nm.msg.centered", msg);
        }
        public static void DrawCursor(ClientConnection client)
        {
            SetTextStyle(client, client.cTheme.prompt);
            client.writer.Write(client.cursorText);
            SetTextStyle(client, client.cTheme.normal);
            client.writer.Flush();
        }
    }
}
