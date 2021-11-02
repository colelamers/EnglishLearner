using System;
using System.Collections.Generic;
using System.Text;
using ProjectLogging;

namespace EnglishLearner
{
    public static class UniversalFunctions
    {
        private static DebugLogging _debuglog = new DebugLogging();
        public static void LoadConfiguration(ref Configuration config)
        {
            ProjectLogging.SetupConfigFile.LoadFromFile(ref config);
        } // function LoadConfiguration;

        public static void SaveConfiguration(ref Configuration config)
        {
            LogToFile("Saving Config Data to file...");

            Configuration fileConfig = new Configuration();
            LoadConfiguration(ref fileConfig);

            if (fileConfig != config)
            {
                // TODO: --1-- must determine a way for people to save to a config file without conflicting with one another as current code prevents any saving
                LogToFile("Existing Configuration and new Configuration File do not match! Configuration not saved");
                throw new Exception("Current Config and Config being saved do not match");
            } // if; performs a check if the current Congif is the exact same as the one passed in.

            ProjectLogging.SetupConfigFile.SaveToFile(ref config);
        } // function SaveConfiguration;

        public static void LogToFile(string whatToLog)
        {
            _debuglog.LogAction(whatToLog);
        } // function LogToFile;

    } // class UniversalFunctions;
} // namespace
