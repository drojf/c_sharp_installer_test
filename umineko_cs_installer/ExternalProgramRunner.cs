using InstallUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace umineko_cs_installer
{
    public class ExternalProgramRunner
    {
        // see https://stackoverflow.com/questions/240171/launching-an-application-exe-from-c
        /// <returns>Returns True if successful, False otherwise</returns>
        static public bool Run(string exePath, string arguments, Logger logger, string printOnSuccess, string printOnFail) => IRun(exePath, arguments, logger, printOnSuccess, printOnFail) == 0;

        ///Same as 'Run' above but returns the integer return value instead of true/false
        /// <returns>call this if you want the integer return value from a process</returns>
        static public int IRun(string exePath, string arguments, Logger logger, string printOnSuccess = null, string printOnFail = null)
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = arguments,
                    //UseShellExecute = false,  //color won't be displayed if this is enabled
                    //RedirectStandardOutput = true,
                    //CreateNoWindow = true
                }
            };

            //printout data 
            /*proc.OutputDataReceived += new DataReceivedEventHandler((sender, e) => {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    //con.Write(e.Data);
                    logger.LogColor(e.Data, ConsoleColor.White, ConsoleColor.Black, true, textDescription:"");
                }
            });*/

            logger.Log($"Running: '{exePath}' Args: '{arguments}'");

            // Run the external process & wait for it to finish
            proc.Start();
            //proc.BeginOutputReadLine(); //begin asynchronous read operations, see https://msdn.microsoft.com/en-us/library/system.diagnostics.process.beginoutputreadline(v=vs.110).aspx
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
        

        /// Extract a file to the specified extractionPath
        static public bool ExtractFile(string sevenZipPath, string archivePath, string extractionPath, Logger logger)
        {
            logger.Log($"Begin 7zip Extracting '{archivePath}' to '{extractionPath}'");
            return Run(exePath        : sevenZipPath, 
                       arguments      : $"x {archivePath} -aoa -o{extractionPath}", 
                       logger         : logger,
                       printOnSuccess : $"Extraction of {archivePath} OK", 
                       printOnFail    : $"Error during extraction of {archivePath}");
        }

        // Downlaod a metalink from the specified URL and place all files in the 'downloadFolder'.
        static public bool DownloadMetaLink(string aria2cPath, string metaLinkURL, string downloadFolder, Logger logger, int numConnections=8, int maxconcurrent=1)
        {
            logger.Log($"Begin Downloading Metalink '{metaLinkURL}' to '{downloadFolder}'");
            return Run(exePath          : aria2cPath,
                       arguments        : $"--file-allocation=none --continue=true --check-integrity=true --max-concurrent-downloads={maxconcurrent} " + 
                                          $"--retry-wait 5 -x {numConnections} --follow-metalink=mem --dir={downloadFolder} {metaLinkURL}",
                       logger           : logger,
                       printOnSuccess   : $"Download metalink {metaLinkURL} OK",
                       printOnFail      : $"Error during download of {metaLinkURL}");
        }
        

    }


}
