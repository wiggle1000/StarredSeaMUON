using StarredSeaMUON.Server;
using StarredSeaMUON.Server.Telnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON
{
    internal class MSPUtils
    {
        public static void SendMSPSoundBaseURL(RemotePlayer client, string url)
        {
            StreamWriter writer = client.telnet.writer;
            writer.Flush();
            writer.Write("!!SOUND U=\"");
            writer.Write(url);
            writer.WriteLine(")");
            writer.Write("!!MUSIC U=\"");
            writer.Write(url);
            writer.WriteLine(")");
            writer.Flush();
        }

        public static void SendMSPSoundTrigger(RemotePlayer client, string fName, string url = "", int vol = 100, int repeats = 1, int priority = 50, bool continueIfPlayedAgain = true)
        {
            StreamWriter writer = client.telnet.writer;
            writer.Flush();
            client.telnet.stream.WriteByte((byte)TelCtrl.IAC);
            client.telnet.stream.WriteByte((byte)TelCtrl.SB);
            client.telnet.stream.WriteByte((byte)TelOption.OPT_MSP);
            writer.Write("!!");
            if (fName.ToLower().Contains("/music/"))
                writer.Write("MUSIC");
            else
                writer.Write("SOUND");
            writer.Write("(");
            writer.Write(fName);
            if (vol != 100)
                writer.Write(" V=" + Math.Clamp(vol, 0, 100));
            if (repeats != 1)
                writer.Write(" L=" + Math.Clamp(repeats, -1, 2048));
            if (priority != 50)
                writer.Write(" P=" + Math.Clamp(priority, 0, 100));
            if (!continueIfPlayedAgain)
                writer.Write(" C=0");
            if (url != "")
                writer.Write(" U=" + url + "");
            writer.Write(")\r\n");
            writer.Flush();
            client.telnet.stream.WriteByte((byte)TelCtrl.IAC);
            client.telnet.stream.WriteByte((byte)TelCtrl.SE);
        }
    }
}
