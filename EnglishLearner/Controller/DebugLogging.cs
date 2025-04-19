using System;
using System.IO;

namespace EnglishLearner
{
    /*
     * Created by Cole Lamers 
     * Date: 2020-12-06
     * 
     * == Purpose ==
     * This code sets up the debug logging library to be used  
     * 
     * Changes: (date,  comment)
     * 2020-12-15,  Revised the program to have a default constructor.
     *              Removed static from all functions. 
     * 2020-12-21,  Revised the LogFilePath to just be \Logs\ because \Debug\Logs was being placed in a new solutions \Debug\ folder
     *              Removed colons in LogAction function between yyyMMdd and put hyphens
     *              
     */

    /*
     * == Global Task List ==
     * 
     */
    public class DebugLogging
    {
        static string LogFilePath { get; set; }
        static string LogFileName { get; set; }
        static string LogFilePathAndName { get; set; }

        /// <summary>
        /// Default Constructor; Sets the path values and then creates the log file or verifies it exists.
        /// </summary>
        public DebugLogging()
        {
            LogFilePath = @"..\Logs\";//Default Path starts in Debug folder of solutions
            LogFileName = $"{DateTime.Now:yyyyMMdd}_Log.txt";
            LogFilePathAndName = Path.Combine(LogFilePath, LogFileName);
            CreateDebugLogger();
        }

        /// <summary>
        /// Creates/Verifies a path and log file.
        /// </summary>
        public void CreateDebugLogger()
        {
            if (!Directory.Exists(LogFilePath))
            {//TODO: --2-- need to fix this so that it doesn't check if a directory exist but it just makes it instead.
                Directory.CreateDirectory(LogFilePath);
            }//creates the log directory if it doesn't exist

            if (!File.Exists(Path.GetFullPath(LogFilePathAndName)))
            {
                using (StreamWriter sw = File.CreateText(LogFilePathAndName)) { }
            }//creates the debug file for specific day the program is run
        }
        /// <summary>
        /// Accepts a string that will be written to the log file.
        /// </summary>
        /// <param name="status">Text to be logged.</param>
        public void LogAction(string status)
        {//writes to the debug logging file
            using (StreamWriter streamWriter = File.AppendText(Path.GetFullPath(LogFilePathAndName)))
            {
                streamWriter.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff}, {status}");
            }
        }
    }
}
