using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace umineko_cs_installer
{
    class umineko_question
    {
        string basePath;

        string relPath(string path)
        {
            return Path.Combine(basePath, path)
        }

        void doInstall()
        {
            basePath = @"c:\temp4\test";

            //check file exists
            File.Exists(relPath("arc.nsa"));

        }
    }
}
