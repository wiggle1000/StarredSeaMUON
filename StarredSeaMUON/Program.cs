using Microsoft.EntityFrameworkCore;
using StarredSeaMUON.Database;
using StarredSeaMUON.Server;
using StarredSeaMUON.Server.Telnet;
using System;
using System.Diagnostics;
using System.Text;

namespace StarredSeaMUON
{
    internal class Program
    {
        private static Process? assetServer;
        const int tickIntervalMS = 250;

        public delegate void GlobalTickHandler(GlobalTickEventArgs e);
        public static event GlobalTickHandler? GlobalTick;

        public static string MudDisplayName = "StarredSeaMUON";

        public static DbContextStarredSea db = new DbContextStarredSea();


        static void Main(string[] args)
        {
            Console.Title = MudDisplayName + " Server";

            //set up encoding
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            if (db.Database.GetPendingMigrations().Count() > 0)
            {
                Console.WriteLine("Pending DB migrations found! Applying.");
                db.Database.Migrate(); //migrates if needed
                db.SaveChanges();
            }

            //start listening for clients
            ConnectionListener cl = new ConnectionListener(System.Net.IPAddress.Any, 9876);
            cl.Start();

            //run ticks on main thread
            while (true)
            {
                Thread.Sleep(tickIntervalMS);
                GlobalTick?.Invoke(new GlobalTickEventArgs(tickIntervalMS/1000f));
            }
        }
    }
    public class GlobalTickEventArgs : EventArgs
    {
        public float Delta;
        public GlobalTickEventArgs(float delta)
        {
            this.Delta = delta;
        }
    }
}