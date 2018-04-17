using System;
using System.IO;

namespace InstallUtils
{
    class Logger
    {
        public StreamWriter logFile;
        bool shouldLogToFile;
        string fileLogLinePrefix = "";

        public Logger() { }

        public Logger(string logPath)
        {
            try
            {
                logFile = new System.IO.StreamWriter(logPath, true);
                logFile.AutoFlush = true;
                shouldLogToFile = true;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Couldn't find file {logPath}");
            }

            //Append a date header to the file
            if(shouldLogToFile)
            {
                string currentDateTime = DateTime.Now.ToString();
                log($"\n\n>>>>>> Logging Begins {currentDateTime} <<<<<");
            }
        }

        //Not sure if this is needed
        public void Dispose()
        {  
            if(shouldLogToFile)
                logFile.Close();
        }

        public void log(string string_to_log)
        {
            Console.ResetColor();
            logLowLevel(string_to_log, "info");
        }

        public void logOK(string string_to_log)
        {
            fileLogLinePrefix = "[ ok ] ";
            logColor(string_to_log, ConsoleColor.White, ConsoleColor.Green, " ok ");
        }

        public void logWarn(string string_to_log)
        {
            fileLogLinePrefix = "[warn] ";
            logColor(string_to_log, ConsoleColor.Black, ConsoleColor.Yellow, "warn");
        }

        public void logError(string string_to_log)
        {
            fileLogLinePrefix = "[ERR ] ";
            logColor(string_to_log, ConsoleColor.Black, ConsoleColor.Red, "err ");
        }
        
        public void logColor(string string_to_log, ConsoleColor foreground_color, ConsoleColor background_color, string textDescription="misc")   
        {
            Console.ForegroundColor = foreground_color;
            Console.BackgroundColor = background_color;

            logLowLevel(string_to_log, textDescription);
        }

        private void logLowLevel(string string_to_log, string textDescription)
        {
            Console.WriteLine(string_to_log);

            if (shouldLogToFile)
            {
                logFile.WriteLine($"[{textDescription,4}] {string_to_log}");
            }
        }

    }
}
