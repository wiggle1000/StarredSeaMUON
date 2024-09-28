using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Server
{
    internal class ConnectionManager
    {
        public static int port = 9876;


        public static List<ClientConnection> connectedClients = new List<ClientConnection>();
        public static void StartServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);

            Logger.Log("Starting Server on port " + port);
            listener.Start();
            Logger.Log("Local endpoint is " + listener.LocalEndpoint);
            Logger.Log("Server is ready to accept connections!");

            while (true)
            {
                Socket s = listener.AcceptSocket();
                Logger.Log("New Socket connection: "+s.RemoteEndPoint.ToString());
                new Task(() => { connectedClients.Add(new ClientConnection(s)); }).Start();
                
            }
        }
    }
}
