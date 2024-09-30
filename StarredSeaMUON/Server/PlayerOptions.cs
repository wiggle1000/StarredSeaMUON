using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Server
{
    internal class PlayerOptions
    {
        public TerminalColorSupport colorSupport;
        public TerminalTheme terminalTheme;
        public Size termSize = new Size(80, 25);

        public MSPSupportType mspSupport = MSPSupportType.MSP_ON;

        public PlayerOptions(RemotePlayer player)
        {
            colorSupport = TerminalColorSupport.ANSI4;
            terminalTheme = TerminalTheme.GetUserTheme(player);
        }

        public void SetTermSize(int w, int h)
        {
            this.termSize = new Size(w, h);
        }
    }
    public enum MSPSupportType
    {
        MSP_OFF = 0,
        MSP_ON = 1,
        MSP_OVER_GMCP = 2
    }
}
