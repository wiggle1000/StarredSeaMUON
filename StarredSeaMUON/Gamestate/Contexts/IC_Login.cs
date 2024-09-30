using StarredSeaMUON.Server;
using StarredSeaMUON.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Gamestate.Contexts
{
    internal class IC_Login : InputContext
    {
        int loginStage = 0;
        float timeInStage = 0;
        float animTimer = -1;
        string uName = "";
        string err = "";
        string errShort = "";

        public IC_Login(RemotePlayer player) : base(player)
        {
        }

        public override void OnActivate()
        {
            loginStage = 0;
            player.Output("");
            if (player.telnet.clientEncoding == Encoding.UTF8)
                AsciiArtHelper.SendAsciiArt(player, "title.utf8");
            else
                AsciiArtHelper.SendAsciiArt(player, "title.ascii");
            player.SetMusic("music/LobbyMusic1.mp3");
            player.PlaySound("sound/system/Startup.mp3");
            player.OutputTransponder("Greetings, traveller! I am Teline, TELCO's premium onboarding AI. Please enter your User Identification to continue the onboarding process.");
            //player.Output("Please enter your User Identification to continue the onboarding process:", player.options.terminalTheme.query);
        }

        public override void Tick(float dt)
        {
            base.Tick(dt);
            timeInStage += dt;

            if(animTimer >= 0)
            {
                float pAnimTimer = animTimer;
                animTimer += dt * 7f;
                if(Math.Floor(animTimer) > Math.Floor(pAnimTimer))
                {
                    if (animTimer > 10)
                    {
                        if (loginStage == 0) //username not found
                        {
                            animTimer = -1;
                            player.OutputError(errShort, false);
                            player.OutputTransponder(err);
                            //TODO: REGISTER USER
                        }
                        else if (loginStage == 1) //ask for password
                        {
                            player.Output("^!green;^yellow;FOUND USER!");
                            animTimer = -1;
                            player.OutputTransponder(TextFormatUtils.ApplyVarNames("Welcome back, %c! Please enter your account password.", player));
                        }
                        else if (loginStage == 2) //enter world
                        {
                            player.Output("^!green;^yellow;MATCH DETERMINED!");
                            player.Output("");
                            player.Output("");
                            animTimer = -1;
                            player.OutputTransponder(TextFormatUtils.ApplyVarNames("Thank you for taking part in this onboarding experience, %c! We hope you enjoy your stay.", player));

                            //switch to new playing context!
                            player.contextStack.SwitchBase(new IC_Playing(player));
                        }
                    }
                    else
                    {
                        player.OutputRaw(".");
                    }
                }
            }
        }

        public override void ProcessPlayerInput(string line)
        {
            base.ProcessPlayerInput(line);
            if (loginStage == 0) // username
            {
                uName = line;
                animTimer = 0;

                if (player.db.CheckForUser(uName))
                {
                    loginStage = 1;
                    animTimer = 0;
                }
                else
                {
                    loginStage = 0;
                    animTimer = 0;
                    errShort = "USER NOT FOUND.";
                    err = "TELCO regrets to inform you that our New User Registration program is not currently accepting applicants.\nYou may re-enter your username to try again.";
                }
                player.OutputRaw("Processing.");
                player.PlaySound("sound/system/UsernameEnter.mp3");
            }
            else if (loginStage == 1) // processing pas
            {
                string pass = line;
                if(player.db.CheckLogin(uName, pass))
                {
                    loginStage = 2;
                    animTimer = 0;
                    player.OutputRaw("Checking records.");
                    player.PlaySound("sound/system/AccessGranted.mp3");
                }
                else
                {
                    loginStage = 0;
                    animTimer = 0;
                    player.PlaySound("sound/system/AccessDenied.mp3");
                    errShort = "RECORD MISMATCH.";
                    err = "TELCO regrets to inform you that the password you have entered is not correct.\nYou may enter your username to try again.";
                }
            }
            else if (loginStage == 2) // logged in, should never get here
            {
            }
        }
    }
}
