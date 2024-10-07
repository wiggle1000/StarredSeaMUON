using StarredSeaMUON.Database;
using StarredSeaMUON.Database.Objects;
using StarredSeaMUON.Gamestate.Contexts;
using StarredSeaMUON.Server.Telnet;
using StarredSeaMUON.Util;
using StarredSeaMUON.World.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StarredSeaMUON.Server
{
    internal class RemotePlayer
    {
        public TelnetConnection telnet;
        public InputContextStack contextStack = new InputContextStack();
        public PlayerOptions options;
        public DbAccount? userInfo;

        public Animal? controlling;

        public DbContextStarredSea db = new DbContextStarredSea();

        public RemotePlayer(TelnetConnection telnet)
        {
            this.telnet = telnet;
            this.options = new PlayerOptions(this);
            Program.GlobalTick += this.contextStack.Tick;
            PlaySound("stop");
        }

        public void NegotiationFinished()
        {
            this.contextStack.SwitchBase(new IC_Login(this));
        }

        ~RemotePlayer()
        {
            Program.GlobalTick -= this.contextStack.Tick;
        }

        public void Input(string line)
        {
            this.contextStack.GetCurrent().ProcessPlayerInput(line);
        }

        public void Output(string text, ConsoleTextFormat? format = null)
        {
            OutputRaw(text + "\r\n", format);
        }

        /// <summary>Doesn't add a newline</summary>
        public void OutputRaw(string text, ConsoleTextFormat? format = null)
        {
            if (format != null)
                telnet.SendEncoded(TextFormatUtils.ApplyColorTags(text, this.options.colorSupport, format, options.terminalTheme.highlight));
            else
                telnet.SendEncoded(TextFormatUtils.ApplyColorTags(text, this));
        }

        public void SetCursorText(string text)
        {
            //TODO
        }
        public void OutputCentered(string text, ConsoleTextFormat format, bool formatEntireLine = true)
        {
            Output(TextFormatUtils.CenteredText(text, this, format, formatEntireLine), format);
        }
        public void OutputTransponder(string text, bool silent = false)
        {
            if (!silent)
                PlaySound("sound/system/Transponder3.mp3");

            //recurse for newlines
            if (text.Contains('\n'))
            {
                string[] subTransponderMessages = text.Replace("\r", "").Split('\n');
                foreach(string smsg in subTransponderMessages)
                {
                    OutputTransponder(smsg, true);
                }
                return;
            }

            int width = options.termSize.Width;
            int lineMaxWidth = width - 12;
            bool canUTF8 = telnet.clientEncoding == Encoding.UTF8;
            string pre  = canUTF8 ? "█▓▒░ " : "]#=- ";
            string post = canUTF8 ? " ░▒▓█" : " -=#[";


            if (text.Length <= lineMaxWidth)
            {
                int bufferAmt = (lineMaxWidth / 2) - (text.Length / 2);
                string line = pre;
                for (int i = 0; i < bufferAmt; i++) line += " ";
                line += text;
                bufferAmt = width - (line.Length + 6);
                for (int i = 0; i < bufferAmt; i++) line += " ";
                Output(line + post, options.terminalTheme.transponder);
            }
            else
            {
                string[] words = text.Split(' ');
                string cLine = "";
                foreach(string word in words)
                {
                    if(cLine.Length + word.Length > lineMaxWidth)
                    {
                        int bufferAmt = (lineMaxWidth / 2) - (cLine.Length / 2) - 1;
                        string line = pre;
                        for (int i = 0; i < bufferAmt; i++) line += " ";
                        line += cLine;
                        bufferAmt = width - (line.Length + 6);
                        for (int i = 0; i < bufferAmt; i++) line += " ";
                        Output(line + post, options.terminalTheme.transponder);
                        cLine = "";
                    }
                    cLine += ((cLine == "") ? "" : " ") + word;
                }
                if(cLine != "") //print last line
                {
                    int bufferAmt = (lineMaxWidth / 2) - (cLine.Length / 2) - 1;
                    string line = pre;
                    for (int i = 0; i < bufferAmt; i++) line += " ";
                    line += cLine;
                    bufferAmt = width - (line.Length + 6);
                    for (int i = 0; i < bufferAmt; i++) line += " ";
                    Output(line + post, options.terminalTheme.transponder);
                    cLine = "";
                }
            }
        }

        public bool PlaySound(string path, int fadein = 0, int fadeout = 0, int vol = 100, int repeats = 1, int priority = 50, bool continueIfPlayedAgain = true)
        {
            MSPSound sound = AssetManager.GetSound(path);
            return AssetManager.PlaySoundToPlayer(this, sound, fadein, fadeout, vol, repeats, priority, continueIfPlayedAgain);
        }

        public bool SetMusic(string path)
        {
            MSPSound sound = AssetManager.GetSound(path);
            return AssetManager.PlaySoundToPlayer(this, sound, 100, 100, 100, -1, 10, true);
        }

        public void SendGMCP(string header, string body)
        {
            telnet.SendGMCP(header, body);
        }

        public bool LogIn(string user, string pass)
        {
            if (InputVerifiers.verifyName(user) != "") return false;
            if (InputVerifiers.verifyPassword(pass) != "") return false;
            DbAccount? userAccount = db.GetAccount(user);
            if (userAccount == null) return false;

            //check password
            if (!PasswordHelper.checkPassword(pass, userAccount.PassHash)) return false;

            if (userAccount.UserID == -1) return false;

            userInfo = userAccount;
            if (userInfo == null) return false;
            return true;
        }

        public bool Register(string user, string email, string pass)
        {
            try
            {
                db.Database.BeginTransaction();
                db.Accounts.Add(new DbAccount() { Email = email, Username = user, PassHash = PasswordHelper.hashPassword(pass) });
                db.Database.CommitTransaction();
                db.SaveChanges();
            }
            catch(Exception e)
            {
                Logger.LogError("Registration error: " + e.Message);
                return false;
            }
            return true;
        }

        internal void OutputError(string error, bool centered = false, bool silent = false)
        {
            string output = options.terminalTheme.error.GetTelnetFormatCode(options.colorSupport) + error;
            if (centered) output = TextFormatUtils.CenteredText(output, this, options.terminalTheme.error);
            Output(output);
            if(!silent)
            {
                PlaySound("sound/system/error.ogg");
            }
        }
    }
}
