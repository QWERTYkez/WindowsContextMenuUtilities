using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Threading;

namespace DeleteEmptyDirs;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            if (args[0] == "-Path")
                DeleteDirs(new DirectoryInfo(args[1]));
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
                            using (var root = Registry.ClassesRoot.OpenSubKey("Directory", true).OpenSubKey("Background", true).OpenSubKey("Shell", true).CreateSubKey("DeleteEmptyDirs", true))
                            {
                                //header
                                root.SetValue(null, "DeleteEmptyDirs");
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
                            Registry.ClassesRoot.OpenSubKey("Directory", true).OpenSubKey("Background", true).OpenSubKey("Shell", true).DeleteSubKeyTree("DeleteEmptyDirs");
                            Console.WriteLine("Успешно!!");
                            Console.ReadLine();
                        }
                        return;
                }
            }
            Console.WriteLine("Удалить пустые каталоги");
            Console.WriteLine("Введите путь");
            DeleteDirs(new DirectoryInfo($"{Console.ReadLine()}"));
        }
    }

    static void DeleteDirs(DirectoryInfo from)
    {
        foreach (var d in from.GetDirectories())
        {
            if (d.GetDirectories().Length > 0) DeleteDirs(d);
            if (d.GetFiles().Length == 0 && d.GetDirectories().Length == 0)
            {
                while (Directory.Exists(d.FullName))
                {
                    d.Attributes = FileAttributes.Normal;
                    d.Delete();
                    Thread.Sleep(100);
                }
            }
        }
    }
}