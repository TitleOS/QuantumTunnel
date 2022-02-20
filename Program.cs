using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace QuantumTunnel
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("QuantumTunnel - C# FlashFS Reader");
            if(args.Length > 1)
            {
                Console.WriteLine("Provide name of file on flash and size. Example: certkeys.bin OR -rawdump to obtain a raw image of the flash, named flash.bin.");
                Environment.Exit(1);
            }
            if(args[0].ToLower() == "-rawdump")
            {
                Console.WriteLine("Dumping raw flash image, this will take a while...");
                if (Flash.DumpFlashImage("flash.bin"))
                {
                    Console.WriteLine("Dumped flash to flash.bin");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine($"An error occured, do you have privileges? Error: {new Win32Exception(Marshal.GetLastWin32Error()).Message}");
                    Environment.Exit(1);
                }
            }
            string filename = args[0];
            int filesize = int.Parse(args[1]);
            FlashFS.ReadFile(filename);
            Console.WriteLine($"File read to {Environment.CurrentDirectory}\\{filename}");
        }
    }
}
