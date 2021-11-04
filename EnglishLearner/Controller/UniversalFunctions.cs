using System;
using System.Collections.Generic;
using System.Text;
using ProjectLogging;

namespace EnglishLearner
{
    /*
     * Created by Cole Lamers 
     * Date: 2021-11-04
     * 
     * == Purpose ==
     * Function calls that can be utilized throughout the program without affecting other peoples work.
     * 
     * 
     */
    public static class UniversalFunctions
    {
        private static DebugLogging _debuglog = new DebugLogging();

        public static void Load_Configuration(ref Configuration config)
        {
            try
            {
                ProjectLogging.SetupConfigFile.LoadFromFile(ref config);
            } // try
            catch (Exception e)
            {
                throw e;
            } // catch; config is null can't be loaded
        } // function LoadConfiguration;

        public static void Save_Configuration(ref Configuration config)
        {
            try
            {
                LogToFile("Saving Config Data to file...");
                Configuration fileConfig = new Configuration();
                ProjectLogging.SetupConfigFile.SaveToFile(ref config);

                //Load_Configuration(ref fileConfig);
                /*
                    if (fileConfig != config)
                    {
                        // TODO: --1-- this is not final...must determine a way for people to save to a config file without conflicting with one another as current code prevents any saving
                        LogToFile("Existing Configuration and new Configuration File do not match! Configuration not saved");
                        throw new Exception("Current Config and Config being saved do not match");
                    } // if; performs a check if the current Congif is the exact same as the one passed in.
                */
            } // try
            catch (Exception e)
            {
                throw e;
            } // catch
        } // function SaveConfiguration;

        public static void LogToFile(string whatToLog)
        {
            try
            {
                _debuglog.LogAction(whatToLog);
            } // try
            catch (Exception e)
            {
                throw e;
            } // catch
        } // function LogToFile;

        public static void LogToFile(string whatToLog, Exception ex)
        {
            _debuglog.LogAction($"{whatToLog}{ex}");
        } // function LogToFile; easy exception add to logging

    } // class UniversalFunctions;
} // namespace
