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
            UminekoQuestionInstaller inst = new UminekoQuestionInstaller(@"c:\temp4\test");
            inst.doInstall();
            
            Console.WriteLine("Task Finished...");
            Console.ReadKey();
        }
    }
}
