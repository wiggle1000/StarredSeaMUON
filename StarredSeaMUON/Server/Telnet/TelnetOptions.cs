using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Server.Telnet
{
    public enum TelOption
    {
        OPT_Echo = 1,                   //
        OPT_Status = 5,                 //
        OPT_CR_Disposition = 10,        //
        OPT_Formfeed_Disposition = 13,  //
        OPT_Extended_ASCII = 17,        //
        OPT_Terminal_Type = 24,         //
        OPT_NAWS = 31,                  // Window Size
        OPT_GMCP = 201,                 //
        OPT_MSP = 90,                   //
    }
    public enum TelCtrl
    {
        SE = 0xF0,      //240   Subnegotiation End
        SB = 0xFA,      //250   Subnegotiation Begin
        WILL = 0xFB,    //251   Will
        WONT = 0xFC,    //252   Won't
        DO = 0xFD,      //253   Do
        DONT = 0xFE,    //254   Don't
        IAC = 0xFF,     //255   Interpret As Command
    }
    public enum TelOptionState
    {
        Unknown    =    0b000,
        ClientIs   =    0b001,
        ServerWill =    0b010,
        ClientIsnt =    0b100, //seperate flag to prevent repeated re-negotiation
        Agreed     =    0b011,
    }

    internal class TelnetOptions
    {
        TelOptionState[] options = new TelOptionState[255];
        public bool SupportsOption(TelOption option)
        {
            if (option < 0 || (int)option >= 255) return false;
            return options[(int)option] == TelOptionState.Agreed;
        }
        public void SetOptionClient(TelOption option, bool clientIs)
        {
            if (option < 0 || (int)option >= 255) return;
            if (clientIs)
            {
                options[(int)option] &= ~TelOptionState.ClientIsnt; //clear isnt flag
                options[(int)option] |= TelOptionState.ClientIs; //set is flag
            }
            else
            {
                options[(int)option] &= ~TelOptionState.ClientIs; //clear is flag
                options[(int)option] |= TelOptionState.ClientIsnt; //set isn't flag
            }
        }
        public void SetOptionServer(TelOption option, bool serverWillSent)
        {
            if (option < 0 || (int)option >= 255) return;
            if (serverWillSent) options[(int)option] |= TelOptionState.ServerWill;
            else options[(int)option] &= ~TelOptionState.ServerWill;
        }

        public void SetOptionState(TelOption option, TelOptionState state)
        {
            if (option < 0 || (int)option >= 255) return;
            options[(int)option] = state;
        }

        public bool HasServerSent(TelOption option)
        {
            if (option < 0 || (int)option >= 255) return true; //dont try to send out of bounds option
            return (options[(int)option] & TelOptionState.ServerWill) > 0;
        }
    }
}
