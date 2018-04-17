using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//consider https://github.com/dwmkerr/consolecontrol

namespace umineko_cs_installer
{
    class Program
    {
        

        static void Main(string[] args)
        {
            UminekoQuestionInstaller inst = new UminekoQuestionInstaller();
            inst.doInstall();

            Console.WriteLine("Task Finished...");
            Console.ReadKey();
        }
    }
}
