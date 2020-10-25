using System;
using System.IO;
using System.Threading;

namespace MoveFilesFromSubdirs
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "-GetInfo")
                {
                    Console.WriteLine("Извлечь файлы из подкаталогов");
                    Console.WriteLine("into");
                }

                if (args[0] == "-Path")
                {
                    var dir = new DirectoryInfo(args[1]);

                    GatFiles(dir.FullName, dir);
                }
            }
        }

        static void GatFiles(string BasePath, DirectoryInfo from)
        {
            foreach (var d in from.GetDirectories())
            {
                foreach (var f in d.GetFiles())
                {
                    var fn = d.FullName.Replace($"{BasePath}\\", "");
                    string nn = "";
                    foreach (string ss in fn.Split('\\')) nn += ss + '.';

                    f.MoveTo($@"{BasePath}\{nn}{f.Name}");
                }
                GatFiles(BasePath, d);

                while (Directory.Exists(d.FullName))
                {
                    d.Delete();
                    Thread.Sleep(100);
                }
            }
        }
    }
}
