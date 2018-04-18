using System;
using System.IO;

namespace InstallUtils
{
    class Logger
    {
        public StreamWriter logFile;
        bool shouldLogToFile;

        public Logger() { }

        public Logger(string logPath)
        {
            try
            {
                logFile = new System.IO.StreamWriter(logPath, true);
                logFile.AutoFlush = true;
                shouldLogToFile = true;
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine($"LOGGER: Couldn't find part of directory {logPath} - no log file written.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"LOGGER: Unknown exception creating log file at {logPath} - no log file written");
                Console.WriteLine(e);
            }

            //Append a date header to the file
            if(shouldLogToFile)
            {
                string currentDateTime = DateTime.Now.ToString();
                Log($"\n\n>>>>>> Logging Begins {currentDateTime} <<<<<");
            }
        }

        //Not sure if this is needed
        public void Dispose()
        {
            Console.ResetColor();

            if (shouldLogToFile)
                logFile.Close();
        }

        public void Log(string string_to_log, bool end=true)
        {
            LogLowLevel(string_to_log, "info", end);
        }

        public void LogOK(string string_to_log, bool end = true)
        {
            LogColor(string_to_log, ConsoleColor.White, ConsoleColor.Green, end, " ok ");
        }

        public void LogWarn(string string_to_log, bool end = true)
        {
            LogColor(string_to_log, ConsoleColor.Black, ConsoleColor.Yellow, end, "warn");
        }

        public void LogError(string string_to_log, bool end = true)
        {
            LogColor(string_to_log, ConsoleColor.Black, ConsoleColor.Red, end, "err ");
        }

        public void LogCodeError(string string_to_log, bool end = true)
        {
            LogColor(string_to_log, ConsoleColor.Green, ConsoleColor.Black, end, "err ");
        }

        public void LogColor(string string_to_log, ConsoleColor foreground_color, ConsoleColor background_color, bool end, string textDescription="misc")   
        {
            Console.ForegroundColor = foreground_color;
            Console.BackgroundColor = background_color;

            LogLowLevel(string_to_log, textDescription, end);
        }

        private void LogLowLevel(string string_to_log, string logType, bool end)
        {
            // Output something like "[ERR ] File was not found"
            string string_to_log_with_type = $"[{logType,4}] {string_to_log}";

            Console.Write(string_to_log_with_type);
            if(end)
            {
                Console.WriteLine("");
            }

            if (shouldLogToFile)
            {
                logFile.WriteLine(string_to_log_with_type);
            }

            Console.ResetColor();
        }

    }
}
