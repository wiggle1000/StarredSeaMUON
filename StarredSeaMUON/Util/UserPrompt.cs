using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StarredSeaMUON.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace StarredSeaMUON.Util
{
    internal class UserPrompt
    {
        private const string DEFAULT_PROMPT_INPUTS = "1234567890qwertyuiopasdfghjkl;zxcvbnm,./";

        string title = "Choose an option:";
        public Dictionary<string, string> options = new();


        public UserPrompt(string title, params string[] options)
        {
            foreach(string o in options)
            {
                if (this.options.Count > DEFAULT_PROMPT_INPUTS.Length) return;
                this.options.Add(DEFAULT_PROMPT_INPUTS[this.options.Count].ToString(), o);
            }
            this.title = title;
        }

        public UserPrompt WithElement(string key, string value)
        {
            this.options.Add(key, value);
            return this;
        }

        public UserPrompt Sorted()
        {
            this.options.OrderBy(pair => pair.Key);
            return this;
        }

        public void DisplayRow(RemotePlayer player, string key, string value, bool isUTF8)
        {
            string borderColor = player.options.terminalTheme.table_border.GetTelnetFormatCode(player.options.colorSupport);
            string keyColor = player.options.terminalTheme.table_key.GetTelnetFormatCode(player.options.colorSupport);
            string valueColor = player.options.terminalTheme.table_value.GetTelnetFormatCode(player.options.colorSupport);
            string sepColor = player.options.terminalTheme.table_rowline.GetTelnetFormatCode(player.options.colorSupport);

            string listLine = borderColor;
            listLine += isUTF8 ? "╟ " : "| ";
            listLine += keyColor;
            listLine += key + ".";
            listLine += sepColor;
            int spacerLength = player.options.termSize.Width - key.Length - value.Length - 6; //-6 for added in characters
            for (int s = 0; s < spacerLength; s++)
                listLine += isUTF8 ? "┈" : ((key.Length + s)%2==1?"-":" ");
            listLine += " ";
            listLine += valueColor;
            listLine += value;
            listLine += borderColor;
            listLine += isUTF8 ? " ╢" : " |";
            player.Output(listLine);
        }
        public void DisplayDividerRow(RemotePlayer player, string firstChar, string middleChar, string lastChar)
        {

            string listLine = firstChar;
            int spacerLength = player.options.termSize.Width - firstChar.Length - lastChar.Length;
            for (int s = 0; s < spacerLength; s++)
            {
                if (middleChar.Length == 1)
                {
                    listLine += middleChar;
                }
                else
                {
                    listLine += middleChar[s % middleChar.Length];
                }
            }
            listLine += lastChar;
            player.Output(listLine, player.options.terminalTheme.table_border);
        }
        public void DisplayTitleRow(RemotePlayer player, string title_raw, bool isUTF8)
        {
            string title = TextFormatUtils.ApplyColorTags(title_raw, player.options.colorSupport, player.options.terminalTheme.table_heading, player.options.terminalTheme.highlight);
            int tLength = TextFormatUtils.StrLenIgnoreANSI(title);
            string borderColor = player.options.terminalTheme.table_border.GetTelnetFormatCode(player.options.colorSupport);
            string titleColor = player.options.terminalTheme.table_heading.GetTelnetFormatCode(player.options.colorSupport);

            string listLine = borderColor;
            listLine += isUTF8 ? "║" : "|";
            int spacerLength = ((player.options.termSize.Width-2) / 2) - (tLength / 2);
            Logger.Log("AWAWAWA " + player.options.termSize.Width + ", " + tLength + ", " + spacerLength);
            for (int s = 0; s < spacerLength; s++)
                listLine += " "; 
            listLine += titleColor;
            listLine += title;
            listLine += borderColor;
            if ((tLength + player.options.termSize.Width) % 2 == 1)
                spacerLength--;
            for (int s = 0; s < spacerLength; s++)
                listLine += " ";
            listLine += isUTF8 ? "║" : "|";
            player.Output(listLine);
        }

        public void DisplayTo(RemotePlayer player)
        {
            bool isUTF8 = player.telnet.clientEncoding == Encoding.UTF8;
            DisplayDividerRow(player, isUTF8 ? "╓" : ",", isUTF8 ? "─" : "=", isUTF8 ? "╖" : ",");
            DisplayTitleRow(player, title, isUTF8);
            DisplayDividerRow(player, isUTF8 ? "╟" : "|", isUTF8 ? "─" : "=", isUTF8 ? "╢" : "|");
            for (int i = 0; i < options.Count; i++)
            {
                string key = options.Keys.ElementAt(i);
                DisplayRow(player, key.ToString(), options[key], isUTF8);
            }
            DisplayDividerRow(player, isUTF8 ? "╙" : "'", isUTF8 ? "─" : "=", isUTF8 ? "╜" : "'");
        }
    }
}
