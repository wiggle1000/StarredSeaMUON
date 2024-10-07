using StarredSeaMUON.Database;
using StarredSeaMUON.Database.Objects;
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

        string regPass = "";
        string regEmail = "";

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
            player.SetMusic("music/LobbyMusic3.mp3");
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
                        else if (loginStage == 10) //start registration
                        {
                            player.Output("^!red;^yellow;NO MATCH FOUND!");
                            animTimer = -1;
                            player.OutputTransponder(TextFormatUtils.ApplyVarNames("The requested Username was not found in our system. Would you like to sign up using this name?", player));

                        }
                        else if (loginStage == 11) //Get email
                        {
                            player.Output("^!green;^yellow;STARTING ACCOUNT CREATION!");
                            animTimer = -1;
                            player.OutputTransponder(TextFormatUtils.ApplyVarNames("Please enter your e-mail address. This will never be intentionally shared with a third party. (Optional)", player));
                        }
                        else if (loginStage == 12) //Get password
                        {
                            player.Output("^!green;^yellow;" + ((regEmail == "") ? "EMAIL OMITTED." : "EMAIL OK!"));
                            animTimer = -1;
                            player.OutputTransponder(TextFormatUtils.ApplyVarNames("Please enter a password for your account. Make sure it's memorable!", player));
                        }
                        else if (loginStage == 13) //Confirm password
                        {
                            player.Output("^!green;^yellow;PASSWORD VALID...");
                            animTimer = -1;
                            player.OutputTransponder(TextFormatUtils.ApplyVarNames("Please enter the password again, to confirm.", player));
                        }
                        else if (loginStage == 20) //End of registration message
                        {
                            player.Output("^!green;^yellow;ACCOUNT CREATED!");
                            animTimer = -1;
                            player.OutputTransponder(TextFormatUtils.ApplyVarNames("Thank you for partaking in this guided account creation process.\nRedirecting to character creation...", player));

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
                string usernameIssue = InputVerifiers.verifyName(line);
                if (usernameIssue != "")
                {
                    loginStage = 0;
                    animTimer = 0;
                    errShort = "USERNAME NOT VALID";
                    err = usernameIssue;
                    return;
                }
                uName = line;
                animTimer = 0;

                DbAccount? loggingInAccount = player.db.GetAccount(uName);
                if (loggingInAccount != null)
                {
                    loginStage = 1;
                    animTimer = 0;
                }
                else
                {
                    loginStage = 0;
                    animTimer = 0;
                    if(Constants.RegistrationEnabled)
                    {
                        loginStage = 10;
                        animTimer = 0;
                    }
                    else
                    {
                        errShort = "USER NOT FOUND.";
                        err = "TELCO regrets to inform you that our New User Registration program is not currently accepting applicants.\nYou may re-enter your username to try again.";
                    }
                }
                player.OutputRaw("Processing.");
                player.PlaySound("sound/system/UsernameEnter.mp3");
            }
            else if (loginStage == 1) // processing pas
            {
                string pass = line;
                if(player.LogIn(uName, pass))
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
            else if (loginStage == 10) // register, step 1 (confirm)
            {
                if (line.ToLower() == "yes" || line.ToLower() == "y")
                {
                    loginStage = 11;
                    animTimer = 0;
                }
                else
                {
                    loginStage = 0;
                    animTimer = 0;
                    errShort = "DECLINED NEW ACCOUNT CREATION.";
                    err = "You may re-enter your username to try again.";
                }
                player.OutputRaw("Preparing.");
            }
            else if (loginStage == 11) // register, step 2 (email)
            {
                player.OutputRaw("Checking E-Mail.");
                string email = line;
                if(email == "")
                {
                    regEmail = "";
                    loginStage = 12;
                    animTimer = 0;
                }
                else
                {
                    string emailNotes = InputVerifiers.verifyEmail(email);
                    if (emailNotes != "")
                    {
                        loginStage = 11;
                        animTimer = 0;
                        player.OutputError(emailNotes, true, false);
                    }
                    else
                    {
                        regEmail = email;
                        loginStage = 12;
                        animTimer = 0;
                    }
                }
            }
            else if (loginStage == 12) // register, step 3 (password)
            {
                player.OutputRaw("Checking Password Validity.");
                string pass = line;
                string passNotes = InputVerifiers.verifyPassword(pass);
                if (passNotes != "")
                {
                    loginStage = 12;
                    animTimer = 0;
                    player.OutputError(passNotes, true, false);
                }
                else
                {
                    regPass = pass;
                    loginStage = 13;
                    animTimer = 0;
                }
            }
            else if (loginStage == 13) // register, step 4 (password confirm)
            {
                player.OutputRaw("Checking Password Match..");
                string pass = line;
                if (pass != regPass)
                {
                    loginStage = 12;
                    animTimer = 0;
                    player.OutputError("Passwords Don't match!", true, false);
                }
                else
                {
                    loginStage = 20; //end of registration
                    animTimer = 0;
                    if(player.Register(uName, regEmail, regPass))
                    {
                        player.LogIn(uName, regPass);
                    }
                    else
                    {
                        loginStage = 0;
                        errShort = "ERROR REGISTERING ACCOUNT.";
                        err = "You may re-enter your username to try again.";
                    }
                    regPass = "";
                    regEmail = "";
                }
            }
        }
    }
}
