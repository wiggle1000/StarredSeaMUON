using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Server
{
    internal class TelnetManger
    {
        public const byte SE =   240; //end suboption
        public const byte SB =   250; //start suboption
        public const byte WILL = 251;
        public const byte WONT = 252;
        public const byte DO =   253;
        public const byte DONT = 254;
        public const byte IAC =  255;

        private static void SendAscii(StreamWriter w, byte[] c)
        {
            w.BaseStream.Write(c, 0, c.Length);
            if (c.Length % 2 == 1) w.BaseStream.WriteByte(0);
        }
        public static bool NegotiateOption(ClientConnection client, byte option)
        {
            StreamWriter w = client.writer;

            SendAscii(w, new byte[] { 0, IAC, DO, option });
            w.Flush();

            byte[] response = new byte[]{0, 0, 0};
            client.reader.BaseStream.Read(response, 0, 3);

            if (response[0] != IAC || response[2] != option)
            {
                Logger.LogError("Got invalid negotion response for option " + option + ": " +
                    ((int)response[0]).ToString() + "-" + ((int)response[1]).ToString() + "-" + ((int)response[2]).ToString());
                return false;
            }
            return response[1] == WILL;
        }
        public static TelnetCapabilities GetClientCapabilities(ClientConnection client)
        {
            client.reader.DiscardBufferedData();
            TelnetCapabilities cap = new TelnetCapabilities();
            cap.canMSP = NegotiateOption(client, 90);
            Logger.Log("can msp? -------- " + cap.canMSP);
            cap.canNAWS = NegotiateOption(client, 31);
            Logger.Log("can NAWS? ------- " + cap.canNAWS);
            return cap;
        }
    }
}
