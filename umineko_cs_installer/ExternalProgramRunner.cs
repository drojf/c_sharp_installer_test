using InstallUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace umineko_cs_installer
{
    public class ExternalProgramRunner
    {
        //see https://stackoverflow.com/questions/240171/launching-an-application-exe-from-c
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exePath"></param>
        /// <param name="arguments"></param>
        /// <returns>Returns True if successful, False otherwise</returns>
        static public bool Run(string exePath, string arguments, Logger logger, string printOnSuccess, string printOnFail) => IRun(exePath, arguments, logger, printOnSuccess, printOnFail) == 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exePath"></param>
        /// <param name="arguments"></param>
        /// <returns>call this if you want the integer return value from a process</returns>
        static public int IRun(string exePath, string arguments, Logger logger, string printOnSuccess = null, string printOnFail = null)
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    //CreateNoWindow = true
                }
            };

            logger.Log($"Running: '{exePath}' Args: '{arguments}'");

            // Run the external process & wait for it to finish
            proc.Start();
            proc.WaitForExit();

            if (proc.ExitCode == 0)
            {
                if(printOnSuccess != null)
                    logger.LogOK(printOnSuccess);
            }
            else
            {
                if (printOnFail != null)
                    logger.LogError(printOnFail);
            }

            return proc.ExitCode;
        }

        /// <summary>
        /// Extract a file to the specified extractionPath
        /// </summary>
        /// <param name="archivePath"></param>
        /// <returns></returns>
        static public bool ExtractFile(string sevenZipPath, string archivePath, string extractionPath, Logger logger)
        {
            logger.Log($"Begin 7zip Extracting '{archivePath}' to '{extractionPath}'");
            return Run(exePath        : sevenZipPath, 
                       arguments      : $"x {archivePath} -aoa -o{extractionPath}", 
                       logger         : logger,
                       printOnSuccess : $"Extraction of {archivePath} OK", 
                       printOnFail    : $"Error during extraction of {archivePath}");
        }

        static public bool DownloadMetaLink(string aria2cPath, string metaLinkURL, string downloadFolder, Logger logger, int numConnections=8, int maxconcurrent=1)
        {
            logger.Log($"Begin Downloading Metalink '{metaLinkURL}' to '{downloadFolder}'");
            return Run(exePath          : aria2cPath,
                       arguments        : $"--file-allocation=none --continue=true --check-integrity=true --max-concurrent-downloads={maxconcurrent}" + 
                                          $"--retry-wait 5 -x {numConnections} --follow-metalink=mem --dir={downloadFolder} {metaLinkURL}",
                       logger           : logger,
                       printOnSuccess   : $"Download metalink {metaLinkURL} OK",
                       printOnFail      : $"Error during download of {metaLinkURL}");
        }
        

    }


}
