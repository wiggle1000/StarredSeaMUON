using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON
{
    internal class Logger
    {
        public static void Log(string s)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[INFO][" + DateTime.Now.ToLongTimeString() + "] " + s);
        }
        public static void LogError(string s)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERR][" + DateTime.Now.ToLongTimeString() + "] " + s);
        }
    }
}
