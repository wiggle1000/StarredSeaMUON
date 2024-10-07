using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Server.Telnet
{
    internal class TelnetConnection
    {
        public Socket socket;
        public NetworkStream stream;
        public StreamReader reader;
        public StreamWriter writer;

        public TelnetOptions telOpts;

        public Thread connectionThread;

        public Encoding clientEncoding = System.Text.Encoding.ASCII;

        GMCPData gmcpData;

        public RemotePlayer player;

        public bool isClientDoneNegotiating = false; //to let server wait for charset negotiation if supported

        public TelOption[] serverSupportedOptions = { TelOption.OPT_GMCP, TelOption.OPT_MSP, TelOption.OPT_NAWS, TelOption.OPT_CHARSET };
        public List<TelOption> optionsDoneNegotiating = new List<TelOption>(); //add options to this list when their initial negotiations are completely done


        public string commandBuffer = ""; //current input line

        public TelnetConnection(Socket client)
        {
            //clientEncoding = System.Text.Encoding.GetEncoding(437);
            //clientEncoding = System.Text.Encoding.UTF8;
            socket = client;
            stream = new NetworkStream(socket);
            reader = new StreamReader(stream, System.Text.Encoding.Latin1);
            writer = new StreamWriter(stream, System.Text.Encoding.Latin1);

            connectionThread = new Thread(new ThreadStart(connThread));
            telOpts = new TelnetOptions();
            gmcpData = new GMCPData();
            connectionThread.Start();
        }

        ~TelnetConnection()
        {
            connectionThread.Join(100);
        }

        public void SendEncoded(string toSend)
        {
            writer.Flush();
            stream.Write(clientEncoding.GetBytes(toSend));
            /*foreach (byte b in clientEncoding.GetBytes(toSend))
            {
                Console.WriteLine("AAA: " + b.ToString());
            }*/
            stream.Flush();
        }
        /*public void SendUTF8(string toSend)
        {
            writer.Flush();
            stream.Write(System.Text.Encoding.UTF8.GetBytes(toSend));
            stream.Flush();
        }*/

        public byte ReadSubByte()
        {
            int b = reader.Read();
            if (b == 255) reader.Read(); //255s doubled within packets
            return (byte)b;
        }

        public int ReadSubInt16()
        {
            int val = (int)ReadSubByte();
            val = (val << 8) | ReadSubByte();
            return val;
        }

        public void DoTelnetSubnegotiation(TelOption option)
        {
            //Console.WriteLine("Checking SB for "+option.ToString());
            switch (option)
            {
                case TelOption.OPT_NAWS:
                    int width = ReadSubInt16();
                    int height = ReadSubInt16();
                    if (reader.Read() != (int)TelCtrl.IAC) throw new MalformedSubnegotiationException("During NAWS; missing tailing IAC");
                    if (reader.Read() != (int)TelCtrl.SE) throw new MalformedSubnegotiationException("During NAWS; SE not where expected");
                    Console.WriteLine("Client reports a width and height of {0}, {1}.", width, height);
                    player.options.SetTermSize(width, height);
                    break;
                case TelOption.OPT_GMCP:
                    gmcpData.ReadRawGMCPPacket(this);
                    break;
                case TelOption.OPT_CHARSET:
                    int nextByte = reader.Read();
                    switch(nextByte)
                    {
                        case 02: //accepted
                            string acceptedCharset = "";
                            for(int i = 0; i < 256; i++)
                            {
                                nextByte = reader.Peek();
                                if (nextByte == (byte)TelCtrl.IAC || nextByte == -1)
                                    break;
                                reader.Read();
                                acceptedCharset += (char)nextByte;
                            }
                            if (reader.Read() != (int)TelCtrl.IAC) throw new MalformedSubnegotiationException("During CHARSET ACCEPT; missing tailing IAC");
                            if (reader.Read() != (int)TelCtrl.SE) throw new MalformedSubnegotiationException("During CHARSET ACCEPT; SE not where expected");
                            try { clientEncoding = Encoding.GetEncoding(acceptedCharset); Logger.Log("Applied encoding: \"" + acceptedCharset + "\""); }
                            catch (ArgumentException e) { clientEncoding = Encoding.ASCII; Logger.LogError("Failed to apply encoding \"" + acceptedCharset + "\"! Falling back to ASCII"); }
                            optionsDoneNegotiating.Add(TelOption.OPT_CHARSET);
                            break;
                        case 03: //rejected
                            if (reader.Read() != (int)TelCtrl.IAC) throw new MalformedSubnegotiationException("During CHARSET DECLINE; missing tailing IAC");
                            if (reader.Read() != (int)TelCtrl.SE) throw new MalformedSubnegotiationException("During CHARSET DECLINE; SE not where expected");
                            clientEncoding = Encoding.ASCII;
                            optionsDoneNegotiating.Add(TelOption.OPT_CHARSET);
                            break;
                    }
                    Console.WriteLine("Got Charset Response");
                    break;
            }
        }

        public void OnClientAgree(TelOption opt)
        {
            Logger.Log("AAA!!! " + opt.ToString());
            switch (opt)
            {
                case TelOption.OPT_CHARSET:
                    Console.WriteLine("Sending list of charsets");
                    SendRequestedCharsets();
                    break;
                default:
                    optionsDoneNegotiating.Add(opt);
                    break;
            }
        }
        public void OnClientDisagree(TelOption opt)
        {
            Logger.Log("BBB!!! " + opt.ToString());
            switch (opt)
            {
                default:
                    optionsDoneNegotiating.Add(opt);
                    break;
            }
        }

        public void ParseTelnetCommand()
        {
            // A second 255, this isn't actually IAC.
            if (reader.Peek() == 255) return;

            int nextByte = reader.Read();
            //IAC is already read in when we get here!
            if (nextByte == (int)TelCtrl.DO) //do
            {
                TelOption opt = (TelOption)reader.Read();
                telOpts.SetOptionClient(opt, true);
                OnClientAgree(opt);

                Console.WriteLine("Got DO " + opt.ToString());
                if (!telOpts.HasServerSent(opt))
                {
                    if (serverSupportedOptions.Contains(opt))
                        SendWill(opt);
                    else
                        SendWont(opt);
                }
            }
            else if (nextByte == (int)TelCtrl.DONT) //don't
            {
                TelOption opt = (TelOption)reader.Read();
                Console.WriteLine("Got DON'T " + opt.ToString());
                telOpts.SetOptionClient(opt, false);
                OnClientDisagree(opt);
                if (!telOpts.HasServerSent(opt))
                {
                    SendWont(opt);
                }
            }
            else if (nextByte == (int)TelCtrl.WILL) //will
            {
                TelOption opt = (TelOption)reader.Read();
                telOpts.SetOptionClient(opt, true);

                Console.WriteLine("Got WILL " + opt.ToString());
                OnClientAgree(opt);
                if (!telOpts.HasServerSent(opt))
                {
                    if (serverSupportedOptions.Contains(opt))
                    {
                        SendDo(opt);
                    }
                    else
                        SendDont(opt);
                }
            }
            else if (nextByte == (int)TelCtrl.WONT) //won't
            {
                TelOption opt = (TelOption)reader.Read();
                Console.WriteLine("Got WON'T " + opt.ToString());
                telOpts.SetOptionClient(opt, false);
                OnClientDisagree(opt);
                if (!telOpts.HasServerSent(opt))
                {
                    SendDont(opt);
                }
            }
            else if (nextByte == (int)TelCtrl.SB) //subnegotiation
            {
                //Console.WriteLine("Got SB!");
                int subType = reader.Read(); //get option that called the subnegotiation
                DoTelnetSubnegotiation((TelOption)subType);
            }
        }
        public void SendRequestedCharsets()
        {
            writer.Flush();
            stream.WriteByte((byte)TelCtrl.IAC);
            stream.WriteByte((byte)TelCtrl.SB);
            stream.WriteByte((byte)TelOption.OPT_CHARSET);
            stream.WriteByte((byte)01); //REQUEST
            stream.WriteByte(Encoding.ASCII.GetBytes(" ")[0]);
            stream.Write(Encoding.ASCII.GetBytes("UTF-8 IBM437 US-ASCII"));
            stream.WriteByte((byte)TelCtrl.IAC);
            stream.WriteByte((byte)TelCtrl.SE);
        }

        public void SendWill(TelOption option)
        {
            writer.Flush();
            stream.WriteByte((byte)TelCtrl.IAC);
            stream.WriteByte((byte)TelCtrl.WILL);
            stream.WriteByte((byte)option);
            telOpts.SetOptionServer(option, true);
        }
        public void SendDo(TelOption option)
        {
            writer.Flush();
            stream.WriteByte((byte)TelCtrl.IAC);
            stream.WriteByte((byte)TelCtrl.DO);
            stream.WriteByte((byte)option);
            telOpts.SetOptionServer(option, true);
        }
        public void SendDont(TelOption option)
        {
            writer.Flush();
            stream.WriteByte((byte)TelCtrl.IAC);
            stream.WriteByte((byte)TelCtrl.DONT);
            stream.WriteByte((byte)option);
            telOpts.SetOptionServer(option, true);
        }
        public void SendWont(TelOption option)
        {
            writer.Flush();
            stream.WriteByte((byte)TelCtrl.IAC);
            stream.WriteByte((byte)TelCtrl.WONT);
            stream.WriteByte((byte)option);
            telOpts.SetOptionServer(option, true);
        }
        public void SendServerOptions()
        {
            SendDo(TelOption.OPT_NAWS);
            foreach (TelOption opt in serverSupportedOptions)
            {
                SendWill(opt);
            }
        }

        private bool ProcessClientInput()
        {
            int read = reader.Read();
            if (read == -1) return false; // end of stream, exit thread

            //Console.WriteLine(read);
            if (read == (int)TelCtrl.IAC)
            {
                try
                {
                    //Console.WriteLine("Getting telnet command!");
                    ParseTelnetCommand();
                }
                catch (Exception e)
                {
                    if (e is MalformedSubnegotiationException)
                    {
                        Logger.LogError("Got malformed subnegotiation!");
                        Logger.LogError(e.Message);
                        return true;
                    }
                    throw;
                }
            }
            else if (read == '\r') //ignore CR
            {
            }
            else if (read == '\n') //newline submits command
            {
                player.Input(commandBuffer);
                commandBuffer = "";
            }
            else //normal input
            {
                commandBuffer += (char)read;
            }
            return true;
        }

        private void connThread()
        {
            //SendUTF8("Hello from UTF8!. ⌂ ↔ ░▒▓█\r\n");
            player = new RemotePlayer(this);
            try
            {
                SendServerOptions();
                for(int i = 0; isClientDoneNegotiating == false; i++)
                {
                    if (ProcessClientInput() == false) return;
                    Thread.Sleep(100);
                    isClientDoneNegotiating = true;
                    foreach (TelOption opt in serverSupportedOptions)
                    {
                        if (!optionsDoneNegotiating.Contains(opt))
                        {
                            isClientDoneNegotiating = false;
                            break;
                        }
                    }
                    if (i > 20) //2 seconds
                    {
                        SendEncoded("CLIENT DIDN'T FINISH NEGOTIATING IN TIME! DISCONNECTING.\n");
                        foreach (TelOption opt in serverSupportedOptions)
                        {
                            if (!optionsDoneNegotiating.Contains(opt))
                            {
                                SendEncoded(opt.ToString() + " NOT NEGOTIATED!\n");
                            }
                            else
                            {
                                SendEncoded(opt.ToString() + " Was negotiated.\n");
                            }
                        }
                        writer.Flush();
                        Thread.Sleep(100);
                        writer.Dispose();
                        reader.Dispose();
                        socket.Close();
                        return; //kill connection.
                    }    
                }
                SendEncoded("Hello from " + clientEncoding.EncodingName + "! ⌂ ↔ ░▒▓█\r\n");
                player.NegotiationFinished();
                while (true)
                {
                    if (ProcessClientInput() == false) break;
                }
            }
            catch(Exception e)
            {
                Logger.LogError(e.Message);
                if(e.StackTrace != null)
                    Logger.LogError(e.StackTrace);
            }
            Console.WriteLine("A client is gone!");
            reader.Close();
            writer.Close();
            stream.Close();
        }

        internal void SendGMCP(string header, string body)
        {
            GMCPData.SendGMCP(this, header, body);
        }
    }
}
