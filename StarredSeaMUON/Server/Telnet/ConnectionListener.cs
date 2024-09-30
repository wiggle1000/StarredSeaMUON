using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Server.Telnet
{
    internal class ConnectionListener
    {
        TcpListener listener;
        Thread listenerThread;

        public ConnectionListener(IPAddress address, int port)
        {
            listener = new TcpListener(address, port);
            listenerThread = new Thread(new ThreadStart(listen));
        }

        public void Start()
        {
            listener.Start(64);
            listenerThread.Start();
                Console.WriteLine("Ready!");
        }
        public void Stop()
        {
            listenerThread.Join();
        }

        private void listen()
        {
            while(true)
            {
                Console.WriteLine("Awaiting Connections.");
                Socket client = listener.AcceptSocket();
                TelnetConnection conn = new TelnetConnection(client);
                Thread.Yield();
            }
        }
    }
}
