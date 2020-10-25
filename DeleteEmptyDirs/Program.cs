using System;
using System.IO;
using System.Threading;

namespace DeleteEmptyDirs
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "-GetInfo")
                {
                    Console.WriteLine("Удалить пустые подкаталоги");
                    Console.WriteLine("into");
                }

                if (args[0] == "-Path")
                    DeleteDirs(new DirectoryInfo(args[1]));
            }
        }

        static void DeleteDirs(DirectoryInfo from)
        {
            foreach (var d in from.GetDirectories())
            {
                if (d.GetFiles().Length == 0)
                {
                    while (Directory.Exists(d.FullName))
                    {
                        d.Delete();
                        Thread.Sleep(100);
                    }
                }
                else DeleteDirs(d);
            }
        }
    }
}
