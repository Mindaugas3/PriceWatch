using System;

namespace ASP.NETCoreWebApplication.Utils
{
    public class ConsoleWriter
    {
        public static void WriteHttpGetScrappers(string url)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[SCRAPPERS]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[HTTP GET]");
            Console.ResetColor();
            Console.Write(url + '\n');
        }
        
        public static void WriteHttpPostScrappers(string url)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[SCRAPPERS]");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[HTTP POST]");
            Console.ResetColor();
            Console.Write(url + '\n');
        }
    }
}