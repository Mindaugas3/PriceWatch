using System;
using System.Runtime.CompilerServices;

namespace ASP.NETCoreWebApplication.Utils
{
    public class Logger
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteHttpGetScrappers(string url)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[SCRAPPERS]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[HTTP GET]");
            Console.ResetColor();
            Console.Write(url + '\n');
        }
        
        
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteHttpPostScrappers(string url)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[SCRAPPERS]");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[HTTP POST]");
            Console.ResetColor();
            Console.Write(url + '\n');
        }
        
        
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteJurassic(string script)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[JURASSIC]");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[RUN JAVASCRIPT]");
            Console.ResetColor();
            Console.Write(script + '\n');
        }
    }
}