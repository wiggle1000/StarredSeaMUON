using StarredSeaMUON.Server;
using System;
using System.Diagnostics;
using System.Text;

namespace StarredSeaMUON
{
    internal class Program
    {
        private static Process? assetServer;

        public static string MudDisplayName = "StarredSeaMUON";
        static void Main(string[] args)
        {
            Console.Title = MudDisplayName + " Server";

            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Console.Write("Starting Asset Server...");
            //Start Asset Server (disabled because it makes it hard to work on..)
            /*if (File.Exists("AssetServer.exe"))
            {
                ProcessStartInfo psi = new ProcessStartInfo("AssetServer.exe");
                psi.UseShellExecute = true;
                psi.Verb = "runas";
                assetServer = Process.Start(psi);
                Console.WriteLine(" OK!");
            } 
            else
            {
                Console.WriteLine(" Asset Server not found! Proceeding without it.");
            }*/

            //Start MUD Server
            ConnectionManager.StartServer();
        }
    }
}