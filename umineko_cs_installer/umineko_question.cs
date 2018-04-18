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
    ///<summary>This class contains implementation shared by all the installers</summary>
    class InstallerBase
    {
        protected readonly string gameFolderPath;       //root directory of game folder
        protected readonly string sevenZipPath;
        protected readonly string downloadFolderPath;   //temporary location where files are downloaded
        protected readonly Logger logger;
        
        protected InstallerBase(string _gamefolderPath, string _downloadFolder)
        {
            gameFolderPath = _gamefolderPath;
            downloadFolderPath = _downloadFolder;
            logger = new Logger(Path.Combine(gameFolderPath, "seventh_mod_patcher.log"));
            sevenZipPath = @"C:\temp\installer_test\temp\7za.exe";

            //try and create the download folder
            TryCreateDir(downloadFolderPath);
        }

        protected bool TryCreateDir(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    logger.Log($"Created Directory {path}");
                }
                else
                {
                    logger.Log($"Directory {path} already exists");
                }
                return true;
            }
            catch(Exception e)
            {
                logger.LogWarn($"Couldn't create directory {path}");
                logger.LogCodeError(e.ToString());
                return false;
            }
        }
        protected string RelPath(string path) => Path.Combine(gameFolderPath, path);
        protected bool RelPathExists(string path) => File.Exists(RelPath(path));
        protected void RelFileMove(string sourceFileName, string destFileName)
        {
            File.Move(RelPath(sourceFileName), RelPath(destFileName));
        }
        //TODO: Maybe just move the files to be deleted to a separate folder, for safety.

        protected bool userAnsweredYes(string promptString)
        {
            logger.LogWarn(promptString, end: false);
            while(true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                logger.Log("\n");
                if (keyInfo.Key == ConsoleKey.Y)
                {
                    return true;
                }
                else if (keyInfo.Key == ConsoleKey.N)
                {
                    return false;
                }

                logger.LogWarn("Please type 'Y' or 'N'");
            }
        }

        //see https://stackoverflow.com/questions/240171/launching-an-application-exe-from-c
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exePath"></param>
        /// <param name="arguments"></param>
        /// <returns>Returns True if successful, False otherwise</returns>
        protected bool Run(string exePath, string arguments) => IRun(exePath, arguments) == 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exePath"></param>
        /// <param name="arguments"></param>
        /// <returns>call this if you want the integer return value from a process</returns>
        protected int IRun(string exePath, string arguments)
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
            
            return proc.ExitCode;
        }

        /// <summary>
        /// Extract a file to the root directory of the game folder
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected bool ExtractFile(string filePath)
        {
            string arguments = $"x {filePath} -aoa -o{gameFolderPath}";
            logger.Log($"Begin 7zip Extracting '{filePath}' to '{gameFolderPath}'");
            bool extractionSuccessful = Run(sevenZipPath, arguments);
            if(extractionSuccessful)
            {
                logger.LogOK($"Extraction of {filePath} OK");
                return true;
            }
            else
            {
                logger.LogError($"Error during extraction of {filePath}");
                return false;
            }
        }

        protected bool ExtractDownloadedFile(string filename)
        {
            return ExtractFile(Path.Combine(downloadFolderPath, filename));
        }

        //TODO: DownloadMetafile

        //TODO: DownloadFile

    }

    ///<summary>Returns true if install succeeded, false otherwise</summary>
    class UminekoQuestionInstaller : InstallerBase
    {
        public UminekoQuestionInstaller(string _gamefolderPath, string _downloadFolder) : base(_gamefolderPath, _downloadFolder)
        {

        }
        
        public bool doInstall()
        {

                //check correct installation folder
                //TODO: list some files from the game folder? or open EXPLORER window?
                if (!RelPathExists("arc.nsa"))
                {
                    logger.LogWarn($"It looks like you selected the wrong install folder (you chose '{gameFolderPath}')");
                    if (!userAnsweredYes("Do you want to install anyway? [y / n]"))
                    {
                        logger.LogError("Install Cancelled - Wrong Directory");
                        return false;
                    }
                }

                //rename Umineko1to4 and 0.utf to keep a backup
                try
                {
                    RelFileMove("Umineko1to4.exe", "Umineko1to4_old.exe");
                    RelFileMove("0.utf", "0_old.utf");
                }
                catch (Exception e)
                {
                    logger.LogWarn("Error backing up umineko1to4.exe or 0.utf - continuing install anyway");
                    logger.LogCodeError(e.ToString());
                }

                logger.Log("Downloading and verifying all files. You can close and reopen this at any time, your progress will be saved.");


                logger.Log("Extracting Files");
                ExtractDownloadedFile("test.zip");

                //TODO: use this to copy one directory to another: https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories


                //TODO: Maybe just move the files to be deleted to a separate folder, for safety.


                logger.Log("Install Finished Successfully");
            
            /*catch(FileNotFoundException fileNotFoundException)
            {
                logger.Log("Missing file");
                logger.LogCodeError(fileNotFoundException.ToString());
            }*/
            return true;
        }
    }
}
