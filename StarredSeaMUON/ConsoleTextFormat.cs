using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON
{
    public enum TerminalColorSupport
    {
        None = 0,
        BlackWhite = 1,
        ANSI4 = 4,
        Full = 24
    }

    internal class ConsoleTextFormat
    {
        public Color textColor = Color.White;
        public Color bgColor = Color.Black;
        public bool bold = false;
        public bool italic = false;
        public bool underline = false;
        public bool blink = false;
        public bool framed = false;
        public bool encircled = false;

        public ConsoleTextFormat(Color textColor, Color bgColor, bool bold = false, bool italic = false, bool underline = false, bool blink = false, bool framed = false, bool encircled = false)
        {
            this.textColor = textColor;
            this.bgColor = bgColor;
            this.bold = bold;
            this.italic = italic;
            this.underline = underline;
            this.blink = blink;
            this.framed = framed;
            this.encircled = encircled;
        }


        public static string GetNearestColorCode(Color c, bool isBG, TerminalColorSupport colorSupport)
        {
            if (colorSupport == TerminalColorSupport.BlackWhite)
            {
                    int index = (c.R + c.G + c.B > 128*3) ? 30 : 97;
                if (isBG) index += 10;
                return index.ToString();
            }
            else if (colorSupport == TerminalColorSupport.ANSI4)
            {
                int index = (c.R > 128 | c.G > 128 | c.B > 128) ? 90 : 30; // Bright bit
                index += (c.R > 64) ? 1 : 0; // Red bit
                index += (c.G > 64) ? 2 : 0; // Green bit
                index += (c.B > 64) ? 4 : 0; // Blue bit
                if (isBG) index += 10;
                return index.ToString();
            }
            else if (colorSupport == TerminalColorSupport.Full)
            {
                string s = "";
                if (isBG) s = "48;2";
                else s = "38;2";
                s += ";" + c.R.ToString();
                s += ";" + c.G.ToString();
                s += ";" + c.B.ToString();
                return s;
            }
            return "0";
        }

        public string GetTelnetFormatCode(TerminalColorSupport colorSupport)
        {
            string formatStr = "\x1b[0";
            formatStr += ";" + GetNearestColorCode(this.textColor, false, colorSupport);
            formatStr += ";" + GetNearestColorCode(this.bgColor, true, colorSupport);
            if (bold) formatStr += ";1";
            if (underline) formatStr += ";4";
            if (italic) formatStr += ";3";
            if (blink) formatStr += ";5";
            if (framed) formatStr += ";51";
            if (encircled) formatStr += ";52";


            formatStr += "m";
            return formatStr;
        }

        //for ascii art
        public static string GetTelnetColorOnlyCode(TerminalColorSupport colorSupport, Color fg, Color bg)
        {
            string formatStr = "\x1b[0";
            formatStr += ";" + GetNearestColorCode(fg, false, colorSupport);
            formatStr += ";" + GetNearestColorCode(bg, true, colorSupport);
            formatStr += "m";
            return formatStr;
        }
    }
}
