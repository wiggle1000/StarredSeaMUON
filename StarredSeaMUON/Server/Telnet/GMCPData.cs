using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Server.Telnet
{
    internal class GMCPData
    {
        public void ReadRawGMCPPacket(TelnetConnection telnet)
        {
            //already read in IAC SB GMCP at this point
            StreamReader reader = telnet.reader;
            string packet = "";
            int maxLen = 2048;
            while(maxLen >= 0)
            {
                int read = reader.Read();
                if (read == -1) return; //buffer ended. client vanished while sending packet!

                if (read == (int)TelCtrl.IAC)
                {
                    if (reader.Peek() == (int)TelCtrl.IAC) //not IAC because it's doubled
                        reader.Read(); //advance one so we dont parse both 255s
                    else
                        break; //IAC hit, packet ended
                }

                packet += (char)read;

                maxLen--;
            }
            if (reader.Read() != (int)TelCtrl.SE) //malformed packet
                return;
            int spaceIndex = packet.IndexOf(' ');
            if(spaceIndex == -1) //no body
            {
                ProcessGMCPPacket(packet, "");
            }
            else
            {
                string header = packet.Substring(0, spaceIndex);
                string body = packet.Substring(spaceIndex + 1);
                ProcessGMCPPacket(header, body);
            }
        }

        internal static bool SendGMCP(TelnetConnection telnet, string header, string body)
        {
            if (!telnet.telOpts.SupportsOption(TelOption.OPT_GMCP)) return false;

            telnet.writer.Flush();
            telnet.stream.WriteByte((byte)TelCtrl.IAC);
            telnet.stream.WriteByte((byte)TelCtrl.SB);
            telnet.stream.WriteByte((byte)TelOption.OPT_GMCP);
            telnet.writer.Write(header);
            if(body.Length > 0)
            {
                telnet.writer.Write(" ");
                telnet.writer.Write(body);
            }
            telnet.writer.Flush();
            telnet.stream.WriteByte((byte)TelCtrl.IAC);
            telnet.stream.WriteByte((byte)TelCtrl.SE);
            return true;
        }

        public void ProcessGMCPPacket(string packetType, string body)
        {
            Console.WriteLine("GOT GMCP PACKET: " + packetType);
            Console.WriteLine(body);
        }
    }
}
