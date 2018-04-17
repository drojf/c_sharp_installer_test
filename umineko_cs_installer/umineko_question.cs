using InstallUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace umineko_cs_installer
{
    class UminekoQuestionInstaller
    {
        string basePath;

        string relPath(string path)
        {
            return Path.Combine(basePath, path);
        }

        public void doInstall()
        {
            basePath = @"c:\temp4\test";

            //check file exists
            File.Exists(relPath("arc.nsa"));

            Logger logger = new Logger(@"c:\temp\logLocation.txt");
            logger.logOK("Log OK test");
            logger.logError("Log Error test");
            logger.logWarn("Log Warn test");
            logger.log("Log normal test");



        }
    }
}
