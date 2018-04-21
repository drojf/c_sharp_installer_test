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
    class InstallSettings
    {
        public readonly string gameFolderPath;       //root directory of game folder
        public readonly string sevenZipPath;
        public readonly string aria2cPath;
        public readonly string downloadFolderPath;   //temporary location where files are downloaded

        public InstallSettings(string gameFolderPath, string downloadFolderPath, string sevenZipPath, string aria2cPath)
        {
            this.gameFolderPath = gameFolderPath;
            this.downloadFolderPath = downloadFolderPath;
            this.sevenZipPath = sevenZipPath;   //path to seven zip exe (including exe, as if calling on command line)
            this.aria2cPath = aria2cPath;       //path to aria2c exe (including exe, as if calling on command line)
        }
    }

    /// This class contains implementation shared by all the installers
    class InstallerBase
    {
        protected readonly InstallSettings settings;
        protected readonly Logger logger;
        
        protected InstallerBase(InstallSettings settings)
        {
            this.settings = settings;

            //must set the logger immediately so that it can be used by the other functions
            this.logger = new Logger(Path.Combine(settings.gameFolderPath, "seventh_mod_patcher.log"));

            //try and create the download folder
            TryCreateDir(settings.downloadFolderPath);
        }

        // Tries to create a directory with the given path
        // If the folder exists, true returned, no action taken.
        // If folder doesn't exist, the function will try to create it;
        // true returned if creation successful, false otherwise.
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

        // creates an absolute path string, given a relative path string from the game folder
        // eg 'temp\test.exe' will convert to '{gamefolder}\temp\test.exe'
        protected string RelPath(string path) => Path.Combine(settings.gameFolderPath, path);

        // checks if a path exists, where 'path' is a path relative to the game folder
        protected bool RelPathExists(string path) => File.Exists(RelPath(path));

        // moves a file, where both the source and destination paths are relative to the game folder
        // TODO: make this work on folders as well?
        protected void RelFileMove(string sourceFileName, string destFileName)
        {
            File.Move(RelPath(sourceFileName), RelPath(destFileName));
        }

        //TODO: Maybe just move the files to be deleted to a separate folder, for safety.
        

        // Prompts the user with 'promptString' to answer Y/N. 
        // Returns true if Y is pressed, false if N is pressed
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
        
        /// Extracts a downloaded file to the game folder path (assumes it is in the download folder)
        protected bool ExtractDownloadedFile(string filename)
        {
            return ExternalProgramRunner.ExtractFile(settings.sevenZipPath, Path.Combine(settings.downloadFolderPath, filename), settings.gameFolderPath, logger);
        }

        /// Begins Download of a metalink file. All downloaded files are placed in the download folder.
        protected bool DownloadToDownloadFolder(string metaLinkURL)
        {
            return ExternalProgramRunner.DownloadMetaLink(settings.aria2cPath, metaLinkURL, settings.downloadFolderPath, logger);
        }

        //TODO: DownloadFile

    }

    /// Returns true if install succeeded, false otherwise
    class UminekoQuestionInstaller : InstallerBase
    {
        public UminekoQuestionInstaller(InstallSettings settings) : base(settings)
        {

        }
        
        public bool doInstall()
        {
            //check correct installation folder
            //TODO: list some files from the game folder? or open EXPLORER window?
            if (!RelPathExists("arc.nsa"))
            {
                logger.LogWarn($"It looks like you selected the wrong install folder (you chose '{settings.gameFolderPath}')");
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
            DownloadToDownloadFolder(@"https://github.com/07th-mod/resources/raw/master/umineko-question/umi_full.meta4");

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
