using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Threading;

namespace ExtractFilesTree;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            if (args[0] == "-Path")
            {
                var dir = new DirectoryInfo(args[1]);
                GatFiles(dir.FullName, dir);
            }
        }
        if (args.Length == 0)
        {
            if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                Console.WriteLine("Подключить - [1], Отключить - [2]");
                switch (Console.ReadLine()) 
                {
                    case "1": 
                        {
                            using (var root = Registry.ClassesRoot.OpenSubKey("Directory", true).OpenSubKey("Background", true).OpenSubKey("Shell", true).CreateSubKey("ExtractFilesTree", true))
                            {
                                //header
                                root.SetValue(null, "ExtractFilesTree");
                                using (var cmd = root.CreateSubKey("command", true))
                                {
                                    //command
                                    cmd.SetValue(null, $"{Process.GetCurrentProcess().MainModule.FileName} -Path \"%V\"");
                                    Console.WriteLine("Успешно!!");
                                    Console.ReadLine();
                                }
                            }
                        }
                        return;
                    case "2":
                        {
                            Registry.ClassesRoot.OpenSubKey("Directory", true).OpenSubKey("Background", true).OpenSubKey("Shell", true).DeleteSubKeyTree("ExtractFilesTree");
                            Console.WriteLine("Успешно!!");
                            Console.ReadLine();
                        }
                        return;
                }
            }
            Console.WriteLine("Развернуть древо файлов");
            Console.WriteLine("Введите путь");
            var dir = new DirectoryInfo($"{Console.ReadLine()}");
            GatFiles(dir.FullName, dir);
        }
    }

    static void GatFiles(string BasePath, DirectoryInfo from)
    {
        Console.WriteLine("Обработка");

        foreach (var d in from.GetDirectories())
        {
            foreach (var f in d.GetFiles())
            {
                var fn = d.FullName.Replace($"{BasePath}\\", "");
                string nn = "";
                foreach (string ss in fn.Split('\\')) nn += ss + '.';

                f.Attributes = FileAttributes.Normal;
                f.MoveTo($@"{BasePath}\{nn}{f.Name}");
            }
            GatFiles(BasePath, d);

            while (Directory.Exists(d.FullName))
            {
                d.Attributes = FileAttributes.Normal;
                d.Delete();
                Thread.Sleep(100);
            }
        }
    }
}