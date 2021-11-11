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
            LogToFile("Load_Configuration called...");
            try
            {
                ProjectLogging.SetupConfigFile.LoadFromFile(ref config);
            } // try
            catch (Exception e)
            {
                LogToFile("Load_Configuration error:", e);
            } // catch; config can't be loaded
        } // function LoadConfiguration;

        public static void Save_Configuration(ref Configuration config)
        {
            LogToFile("Saving_Configuration called...");
            try
            {
                ProjectLogging.SetupConfigFile.SaveToFile(ref config);
            } // try
            catch (Exception e)
            {
                LogToFile("Error in Save_Configuration:", e);
            } // catch
        } // function SaveConfiguration;

        public static void LogToFile(string whatToLog)
        {
            _debuglog.LogAction(whatToLog);
        } // function LogToFile;

        public static void LogToFile(string whatToLog, Exception ex)
        {
            _debuglog.LogAction($"{whatToLog}{ex}");
        } // function LogToFile; easy exception add to logging

        public static void SaveToBinaryFile<T>(string filePath, T objectToWrite)
        {
            LogToFile("SaveToBinaryFile called...");
            ProjectLogging.Serialization.Binary_WriteToFile(filePath, objectToWrite);
        } // function LogToFile

        public static T LoadBinaryFile<T>(string filePath)
        {
            LogToFile("LoadBinaryFile called...");
            return ProjectLogging.Serialization.Binary_ReadFromFile<T>(filePath);
        } // function LoadBinaryFile

    } // class UniversalFunctions;
} // namespace
