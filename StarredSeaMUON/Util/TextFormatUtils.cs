using StarredSeaMUON.Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Util
{
    internal class TextFormatUtils
    {
        public static Dictionary<string, Color> formatterColorNames = new Dictionary<string, Color>();

        static TextFormatUtils()
        {
            /* //HAS TROUBLE WITH ANSI4 :/
            formatterColorNames.Add("black",    Color.FromArgb(0, 0, 0));
            formatterColorNames.Add("dblue",    Color.FromArgb(0, 56, 217));
            formatterColorNames.Add("dgreen",   Color.FromArgb(21, 161, 12));
            formatterColorNames.Add("dcyan",    Color.FromArgb(64, 147, 224));
            formatterColorNames.Add("dred",     Color.FromArgb(197, 14, 32));
            formatterColorNames.Add("dmagenta", Color.FromArgb(136, 22, 154));
            formatterColorNames.Add("dyellow",  Color.FromArgb(194, 155, 0));
            formatterColorNames.Add("dgray",    Color.FromArgb(204, 204, 204));
            formatterColorNames.Add("blue",     Color.FromArgb(118, 118, 118));
            formatterColorNames.Add("green",    Color.FromArgb(22, 198, 12));
            formatterColorNames.Add("cyan",     Color.FromArgb(96, 214, 216));
            formatterColorNames.Add("red",      Color.FromArgb(230, 72, 86));
            formatterColorNames.Add("magenta",  Color.FromArgb(212, 25, 195));
            formatterColorNames.Add("yellow",   Color.FromArgb(255, 255, 202));
            formatterColorNames.Add("white",    Color.FromArgb(255, 255, 255));
            */
            formatterColorNames.Add("black", Color.Black);
            formatterColorNames.Add("dblue", Color.DarkBlue);
            formatterColorNames.Add("dgreen", Color.DarkGreen);
            formatterColorNames.Add("dcyan", Color.DarkCyan);
            formatterColorNames.Add("dred", Color.DarkRed);
            formatterColorNames.Add("dmagenta", Color.DarkMagenta);
            formatterColorNames.Add("dyellow", Color.Orange);
            formatterColorNames.Add("orange", Color.Orange); //alternate name
            formatterColorNames.Add("gray", Color.DarkGray);
            formatterColorNames.Add("grey", Color.Gray);//alternate name
            formatterColorNames.Add("dgray", Color.Gray);
            formatterColorNames.Add("dgrey", Color.DarkGray); //alternate name
            formatterColorNames.Add("blue", Color.Blue);
            formatterColorNames.Add("green", Color.Green);
            formatterColorNames.Add("cyan", Color.Cyan);
            formatterColorNames.Add("red", Color.Red);
            formatterColorNames.Add("magenta", Color.Magenta);
            formatterColorNames.Add("yellow", Color.Yellow);
            formatterColorNames.Add("white", Color.White);
        }
        public static string ParseFormatModifier(string fullFormat, TerminalColorSupport colorSupport, ConsoleTextFormat resetTo, ConsoleTextFormat highlight)
        {
            string format = fullFormat;
            Console.WriteLine("FORMAT: \"" + format + "\"");
            bool isBG = false;
            if(fullFormat.StartsWith('!'))
            {
                isBG = true;
                format = fullFormat.Substring(1);
            }

            //reset
            if (format.ToLower() == "reset" || format.ToLower() == "r")
            {
                return resetTo.GetTelnetFormatCode(colorSupport);
            }

            //italic
            if (format.ToLower() == "i")
            {
                return resetTo.GetTelnetFormatCode(colorSupport);
                return "\x1b[3m";
                //if (bold) formatStr += ";1";
                //if (underline) formatStr += ";4";
                //if (italic) formatStr += ";3";
                //if (blink) formatStr += ";5";
                //if (framed) formatStr += ";51";
                //if (encircled) formatStr += ";52";
            }
            else if (format.ToLower() == "b")
                return "\x1b[1m";
            else if (format.ToLower() == "u")
                return "\x1b[4m";
            else if (format.ToLower() == "blink")
                return "\x1b[5m";
            else if (format.ToLower() == "frame")
                return "\x1b[51m";
            else if (format.ToLower() == "circle")
                return "\x1b[52m";
            else if (format.ToLower() == "hl")
                return highlight.GetTelnetFormatCode(colorSupport);

            //color (listed)
            if (formatterColorNames.ContainsKey(format.ToLower()))
            {
                return ConsoleTextFormat.GetTelnetColorOnlyFormat(colorSupport, formatterColorNames[format.ToLower()], isBG);
            }
            //color (r,g,b)
            else if(format.Contains(','))
            {
                string[] parts = format.Split(',');
                if(parts.Length == 3)
                {
                    int r, g, b;
                    if (int.TryParse(parts[0], out r) && int.TryParse(parts[1], out g) && int.TryParse(parts[2], out b))
                    {
                        return ConsoleTextFormat.GetTelnetColorOnlyFormat(colorSupport, Color.FromArgb(Math.Clamp(r, 0, 255), Math.Clamp(g, 0, 255), Math.Clamp(b, 0, 255)), isBG);

                    }
                }
            }
            return "";
        }
        public static string ApplyColorTags(string input, TerminalColorSupport colorSupport, ConsoleTextFormat resetTo, ConsoleTextFormat highlight)
        {
            string output = resetTo.GetTelnetFormatCode(colorSupport);
            for(int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if(c == '^')
                {
                    int nextSpace = input.IndexOf(' ', i);
                    int formatEnd = input.IndexOf(';', i);
                    if(formatEnd != -1 && (nextSpace == -1 || nextSpace > formatEnd))
                    {
                        i++; //skip over ;
                        
                        string format = input.Substring(i, formatEnd - i);
                        Logger.Log("Doing format to a string: " + format);

                        string formatCode = ParseFormatModifier(format, colorSupport, resetTo, highlight);

                        output += formatCode; //skip over format code

                        i = formatEnd;
                    }
                }
                else
                {
                    output += c;
                }
            }
            return output;
        }

        public static string ApplyVarNames(string input, RemotePlayer player)
        {
            string output = "";
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '%' && (i == 0 || input[i-1] == ' '))
                {
                    i++; //skip over %

                    char formatSpecifier = input[i];
                    
                    switch(formatSpecifier)
                    {
                        case 'c': output += "[YOUR NAME]"; break;
                        default: output += "%" + formatSpecifier; break;
                    }

                    i++; //skip over specifier
                }
                else
                {
                    output += c;
                }
            }
            return output;
        }

        public static string ApplyColorTags(string input, TerminalColorSupport colorSupport)
        {
            return ApplyColorTags(input, colorSupport, ConsoleTextFormat.PLAIN, ConsoleTextFormat.HIGHLIGHT);
        }
        public static string ApplyColorTags(string input, RemotePlayer player)
        {
            return ApplyColorTags(input, player.options.colorSupport, player.options.terminalTheme.normal, player.options.terminalTheme.highlight);
        }

        public static string CenteredText(string input, RemotePlayer player, ConsoleTextFormat format, bool formatEntireLine = true)
        {
            string output = "";
            int sideLength = (player.options.termSize.Width / 2) - (input.Length/2);
            if (formatEntireLine) output += format.GetTelnetFormatCode(player.options.colorSupport);
            for (int i = 0; i < sideLength; i++) output += " ";
            if (!formatEntireLine) output += format.GetTelnetFormatCode(player.options.colorSupport);

            output += input;

            if (!formatEntireLine) output += format.GetTelnetFormatCode(player.options.colorSupport);
            if (input.Length % 2 == 1) sideLength--; //fix uneven lines not being the right width
            for (int i = 0; i < sideLength; i++) output += " ";
            if (formatEntireLine) output += format.GetTelnetFormatCode(player.options.colorSupport);

            return output;
        }

        
    }
}
