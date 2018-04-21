using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//consider https://github.com/dwmkerr/consolecontrol
//notes on c# string interp using $"example: {variable name}" https://weblog.west-wind.com/posts/2016/Dec/27/Back-to-Basics-String-Interpolation-in-C

namespace umineko_cs_installer
{
    class Program
    {
        static void Main(string[] args)
        {
            InstallSettings installSettings = new InstallSettings(
                gameFolderPath: @"C:\temp4\test_cs_installer",
                downloadFolderPath: @"C:\temp4\test_cs_installer\downloads",
                sevenZipPath: @"C:\temp4\test_cs_installer\temp\7za.exe",
                aria2cPath: @"C:\temp4\test_cs_installer\temp\aria2c.exe");
            UminekoQuestionInstaller inst = new UminekoQuestionInstaller(installSettings);
            inst.doInstall();
            
            Console.WriteLine("Task Finished...");
            Console.ReadKey();
        }
    }
}
