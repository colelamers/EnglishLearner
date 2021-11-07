using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Data.Sqlite; // NuGet package;
using System.Data;

namespace EnglishLearner
{
    /*
     * Created by Cole Lamers, Eric Spoerl
     * Date: 2021-11-04
     * 
     * == Purpose ==
     * This is where the primary input from our application will occur.
     * The primary goal for this for the final is to develop an agent that learns from user input/a data source of sentences and
     * can learn to speak based off of that information utilizing a tree.
     * 
     */

    class Main_View
    {
        Configuration _config = null; // TODO: --1-- need to determine where this should live and be addressed
        Sqlite_Actions _sql; // talbe is called 'entries'

        private void Run()
        {
            UniversalFunctions.LogToFile("Function Run called...");
            StartupActions();
            Console.WriteLine("Please provide a sentence for me to learn from:\n");

            string s1 = "The quick brown fox jumped over the lazy dog.";
            string s2 = "the slow brown fox jumped over the lazy dog.";
            string s3 = "The quick red fox jumped over the lazy dog.";
            string s4 = "The quick brown turtle jumped over the lazy dog.";
            string s5 = "The quick brown fox ran over the lazy dog.";
            string s6 = "The quick brown fox jumped around the lazy dog.";
            string s7 = "The quick brown fox jumped over that lazy dog.";
            string s8 = "The quick brown fox jumped over the happy dog.";
            string s9 = "The quick brown fox jumped over the lazy cat.";
            string s10 = "A quick brown fox jumped over the lazy dog."; // TODO: --1-- need to perform the update based on root being different as well

            string[] sentences = { s1, s2, s3, s4, s5, s6, s7, s8, s9, s10 };
            Dictionary<string, Tree> treeDict = new Dictionary<string, Tree>(); // TODO: --3-- this may need to be stored in a brain class

            foreach (string sp in sentences)
            {
                var nsp = new Phrase(sp);

                Tree test;
                treeDict.TryGetValue(nsp.Phrase_First_Word, out test);

                if (test != null)
                {
                    treeDict[nsp.Phrase_First_Word].DFS_Append(nsp.Phrase_Split_Sentence, treeDict[nsp.Phrase_First_Word].Root);
                }
                else
                {
                    treeDict.Add(nsp.Phrase_First_Word, new Tree(nsp.Phrase_Split_Sentence));
                }
            }
        } // function Run;

        #region Startup_Functions
        private void StartupActions()
        {
            UniversalFunctions.LogToFile("Function StartupActions called...");
            UniversalFunctions.Load_Configuration(ref this._config);

            if (this._config != null)
            {
                this._sql = new Sqlite_Actions(_config.SolutionDirectory + "\\Data", "Dictionary");
            } // if; config is empty or does not exist, it will create it and then save it
            else
            {
                this._config = new Configuration();
                this._config.ConfigPath = "Cole Test";
                this._config.ExitCode = 0;
                this._config.SolutionDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
                this._sql = new Sqlite_Actions(_config.SolutionDirectory + "\\Data", "Dictionary");
                this._config.ProjectFolderPaths = Directory.GetDirectories(_config.SolutionDirectory)
                    .Select(d => new { Attr = new DirectoryInfo(d).Attributes, Dir = d })
                    .Select(x => x.Dir)
                    .ToList(); // Gives us the exact directory paths of all the folders within the the program.

                UniversalFunctions.Save_Configuration(ref this._config);
            }

        } // function StartupActions;

        static void Main(string[] args)
        {
            UniversalFunctions.LogToFile("Main Function Called");

            Main_View n_View = new Main_View();
            n_View.Run(); // Exists because we can't do some things within a static function like Main so handling everything in a non-static run function
        } // function Main;

        #endregion Startup

    }
}
