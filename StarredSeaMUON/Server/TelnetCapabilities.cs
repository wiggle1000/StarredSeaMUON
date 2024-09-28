using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Server
{
    internal class TelnetCapabilities
    {
        public int termWidth = 80;
        public int termHeight = 25;

        public TerminalColorSupport colorSupport = TerminalColorSupport.None;

        public bool canNAWS = false; //OPT31; RFC1073, telnet window size option

        public bool canMSP = false; //OPT90; Mud Sound Protocol


    }
}
