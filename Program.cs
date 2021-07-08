using System;

namespace QuantumTunnel
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("QuantumTunnel - C# FlashFS Reader");
            if(args.Length == 0)
            {
                Console.WriteLine("Provide name of file on flash. Example: certkeys.bin");
                Environment.Exit(-1);
            }
            string filename = args[0];
            FlashFS.ReadFile(filename);
            Console.WriteLine($"File read to {Environment.CurrentDirectory}\\{filename}");
        }
    }
}
