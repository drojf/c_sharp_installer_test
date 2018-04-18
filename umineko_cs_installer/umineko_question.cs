using InstallUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace umineko_cs_installer
{
    ///<summary>This class contains implementation shared by all the installers</summary>
    class InstallerBase
    {
        protected readonly string gameFolderPath;
        protected readonly Logger logger;

        protected InstallerBase(string _gamefolderPath)
        {
            gameFolderPath = _gamefolderPath;
            logger = new Logger(gameFolderPath);
        }

        protected string RelPath(string path) => Path.Combine(gameFolderPath, path);
        protected bool RelPathExists(string path) => File.Exists(RelPath(path));
        protected void RelFileMove(string sourceFileName, string destFileName)
        {
            File.Move(RelPath(sourceFileName), RelPath(destFileName));
        }
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
    }

    ///<summary>Returns true if install succeeded, false otherwise</summary>
    class UminekoQuestionInstaller : InstallerBase
    {
        public UminekoQuestionInstaller(string _gamefolderPath) : base(_gamefolderPath)
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
            catch(Exception e)
            {
                logger.LogWarn("Error backing up umineko1to4.exe or 0.utf - continuing install anyway");
                logger.LogCodeError(e.ToString());
            }

            logger.Log("Downloading and verifying all files. You can close and reopen this at any time, your progress will be saved.");

            
            logger.Log("Install Finished Successfully");
            return true;
        }
    }
}
