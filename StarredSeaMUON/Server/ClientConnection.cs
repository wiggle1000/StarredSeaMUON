using Microsoft.Data.Sqlite;
using StarredSeaMUON.Server;
using StarredSeaMUON.Commands;
using StarredSeaMUON.Database;
using StarredSeaMUON.World.Locale;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Server
{
    internal class ClientConnection
    {
        //SslStream sslStream;
        NetworkStream stream;
        public StreamReader reader;
        public StreamWriter writer;

        public DBHelper db = new DBHelper();

        public bool isConnected = false;
        public bool isAuthenticated = false;
        public long authenticatedID = -1;
        public string authenticatedName = null;

        private string cRoomMusic = "";

        public TerminalTheme cTheme = TerminalTheme.currentTheme;
        public TerminalColorSupport colorSupport = TerminalColorSupport.Full;
        public TelnetCapabilities capabilities = new TelnetCapabilities();
        public int termWidth
        {
            get { return capabilities.termWidth; }
        }

        public string cursorText = "";

        public enum ClientConnectionState
        {
            Initializing = 0,
            LoggingIn = 1,
            Registering = 2,
            Ingame = 10,
        }
        public Socket socket;
        public ClientConnection(Socket s)
        {
            this.socket = s;
            
            Logger.Log("Created ClientConnection object for " + s.RemoteEndPoint.ToString());
            //sslStream = new SslStream(new System.Net.Sockets.NetworkStream(s));
            //sslStream.AuthenticateAsServer(X509Certificate.CreateFromCertFile("NeoMud.test.pfx"));
            stream = new NetworkStream(s);
            Logger.Log("SSL Initialized for " + s.RemoteEndPoint.ToString());
            isConnected = true;
            reader = new StreamReader(stream, Encoding.UTF8);
            writer = new StreamWriter(stream, Encoding.UTF8);
            //reader = new StreamReader(sslStream);
            //writer = new StreamWriter(sslStream);

            //MessageSender.SendDat(writer, "nm.input.cursorVisible", false);
            //MessageSender.SendDat(writer, "nm.gameName", Program.MudDisplayName);
            //MessageSender.SendDat(writer, "nm.gameArea", "Logging In");
            //MessageSender.SendDat(writer, "nm.gameRoom", "");
            //MessageSender.SendDat(writer, "nm.clearScreen", ""); //TODO: REIMPLEMENT THESE WITH MUD PROTOCOLS
            MessageSender.SendAsciiArt(this, "title");
            //MessageSender.SendAsciiArt(writer, "testForGame");
            //MessageSender.SendAsciiArt(writer, "Nether");
            //MessageSender.SendDat(writer, "nm.input.cursorVisible", true);
            //MessageSender.SendDat(writer, "nm.sound.playMidi.loop", "music/peaceful");
            //MessageSender.SendDat(writer, "nm.sound.playOgg", "sound/system/welcome");
            try
            {
                while (!isAuthenticated)
                {
                    //MessageSender.SendDat(writer, "nm.gameArea", "Welcome!");
                    string err = doLoginProcess();
                    if (!isAuthenticated)
                    {
                        //MessageSender.SendDat(writer, "nm.clearScreen", "");
                        MessageSender.SendAsciiArt(this, "title");
                        //MessageSender.SendDat(writer, "nm.color.fg", "R");
                        MessageSender.SetTextStyle(this, cTheme.prompt);
                        MessageSender.SendText(writer, "RETURNED TO LOGIN: " + err);
                        MessageSender.SetTextStyle(this, cTheme.normal);
                        //MessageSender.SendDat(writer, "nm.sound.playOgg", "sound/system/error");
                        MessageSender.ClearHistory(this);
                    }
                }
            }
            catch(Exception e)
            {
                //doDisconnect("Error during Authentication.\n" + e.ToString());
                doDisconnect("Error during Authentication.");
                Logger.LogError("Authentication error: \n" + e.ToString());
            }
            MessageSender.ClearHistory(this);
            if (!isAuthenticated)
            {
                //MessageSender.SendDat(writer, "nm.sound.playOgg", "sound/system/error");
                doDisconnect("Failed to Authenticate.");
            }

            //MessageSender.SendDat(writer, "nm.sound.stopMidi", "");
            //MessageSender.SendDat(writer, "nm.sound.playOgg", "sound/system/chimes");
            MessageSender.ClearHistory(this);
            try
            {
                doPostLoginMaintenance();
            }
            catch (Exception e)
            {
                MessageSender.SendError(this, "Error thrown during PLM!");
                Logger.LogError("Error thrown during PLM!\n" + e.ToString());
            }
            while (isConnected)
            {
                try
                {
                    doGameplay();
                }
                catch(Exception e)
                {
                    MessageSender.SendError(this, "Gameplay error thrown!!");
                    Logger.LogError("Gameplay error thrown!\n" + e.ToString());
                }
            }
            Logger.Log("User is disconnecting.");
            doDisconnect();
        }

        public void doGameplay()
        {
            MessageSender.DrawCursor(this);
            string input = reader.ReadLine();
            CommandParser.TryRunCommand(this, input);
            //MessageSender.SendText(writer, input);

        }

        public void visitRoom(long id)
        {
            Room room = World.World.TryGetRoom(id);
            //MessageSender.SendDat(writer, "nm.gameArea", room.material.ToString());
            //MessageSender.SendDat(writer, "nm.gameRoom", room.title);
            room.DisplayToUser(this);
            if (room.hasMeta("music") && cRoomMusic != room.getMetaString("music"))
            {
                //MessageSender.SendDat(writer, "nm.sound.playMidi.loop", room.getMetaString("music"));
                cRoomMusic = room.getMetaString("music");
            }
        }


        public void doPostLoginMaintenance()
        {
            capabilities = TelnetManger.GetClientCapabilities(this);
            AssetManager.PlaySoundToPlayer(this, new MSPSound("music/peaceful.mid"));
            using (SqliteDataReader r = db.GetUserReader(authenticatedID))
            {
                if (r.Read())
                {
                    if (r.IsDBNull("roomid"))
                    {
                        Console.WriteLine(authenticatedName + " HAS NULL ROOMID! Assigning to room 1");
                        db.ExecuteNonQuery(new SqliteCommandBuilder(db.connection, "UPDATE users SET roomid=@rid WHERE id=@uid")
                            .WithIntParam("uid", authenticatedID).WithIntParam("rid", 1));
                    }
                }
            }
            using (SqliteDataReader r = db.GetUserReader(authenticatedID))
            {
                if (r.Read())
                {
                    long roomID = (long)r.GetValue("roomid");
                    visitRoom(roomID);

                    //db.ExecuteNonQuery(new SqliteCommandBuilder(db.connection, "UPDATE users SET roomid=@rid WHERE id=@uid")
                    //    .WithIntParam("uid", authenticatedID).WithIntParam("rid", roomID));
                }
            }
        }
        public string doLoginProcess()
        {
            MessageSender.SendText(writer, "Please Enter your Username:");
            string user = reader.ReadLine();
            string userError = InputVerifiers.verifyName(user);
            if (userError != "")
            {
                return ("Bad Username: " + userError);
            }
            if (db.CheckForUser(user))
            {
                //MessageSender.SendDat(writer, "nm.gameArea", "Logging In");
                MessageSender.SendText(writer, "Welcome back,  " + user + "!");
                MessageSender.SendText(writer, "Please enter your password.");
                string pass = reader.ReadLine();
                string passError = InputVerifiers.verifyPassword(pass);
                if (passError == "" && db.CheckLogin(user, pass))
                {
                    authenticatedID = db.GetUserID(user);
                    if(authenticatedID == -1)
                    {
                        return ("Error getting user ID!");
                    }
                    authenticatedName = user;
                    isAuthenticated = true;
                    return ("Logged in! :D");
                }
                else
                {
                    return ("Incorrect password!");
                }
            }
            else //user doesn't exist
            {
                //MessageSender.SendDat(writer, "nm.gameArea", "Registration");
                MessageSender.SendText(writer, "The username \"" + user + "\" doesn't exist!");
                MessageSender.SendText(writer, "Would you like to create it? (Yes/<No>)");
                string createAnswer = reader.ReadLine();
                if (createAnswer.ToLower().StartsWith("y"))
                {
                    MessageSender.ClearHistory(this);
                    MessageSender.SendAsciiArt(this, "title");
                    MessageSender.SendText(writer, "Account creation for: \"" + user + "\"");
                    MessageSender.SendText(writer, "Please enter your e-mail address or press enter to opt out.");
                    MessageSender.SendText(writer, "This may be used in the future to help provide account services, but we'll never share it with any third parties without your consent.");
                    string email = reader.ReadLine();
                    if (email != "")
                    {
                        string emailError = InputVerifiers.verifyEmail(email);
                        if (emailError != "")
                        {
                            return ("E-Mail Error: " + emailError);
                        }
                    }
                    MessageSender.ClearHistory(this);
                    MessageSender.SendAsciiArt(this, "title");
                    MessageSender.SendText(writer, "Account creation for: \"" + user + "\"");
                    MessageSender.SendText(writer, "E-Mail: " + ((email.Length == 0) ? "(opted out)" : email));
                    MessageSender.SendText(writer, "Now enter a password.");
                    MessageSender.SendText(writer, "Don't forget it! There is currently no reset process.");
                    MessageSender.SendText(writer, "Requirements: between 8 and 64 characters");
                    MessageSender.SendText(writer, "Contains at least one: Uppercase letter, lowercase letter, number, special character");
                    string pass = reader.ReadLine();
                    string passError = InputVerifiers.verifyPassword(pass);
                    if (passError != "")
                    {
                        return ("Password Error: " + passError);
                    }
                    //just to be safe
                    if (db.CheckForUser(user))
                    {
                        return ("User exists during final check! Maybe someone beat you here?");
                    }
                    db.CreateUser(user, email, pass);
                    authenticatedID = db.GetUserID(user);
                    if (authenticatedID == -1)
                    {
                        return ("Error getting user ID!");
                    }
                    authenticatedName = user;
                    isAuthenticated = true;
                    return "Account created!";
                }
                else //chose no
                {
                    return ("Chose not to create account.");
                }
            }
        }

        public void doDisconnect(string message = "Goodbye!")
        {
            if (!socket.Connected) return;
            Logger.Log("DISCONNECTING A USER");
            isConnected = false;
            MessageSender.SendText(writer, "DISCONNECTED!: "+message);
            db.Dispose();
            //sslStream.Close();
            stream.Close();
            reader.Close();
            writer.Close();
            socket.Close();
        }
    }
}
