using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Data.Sqlite; // NuGet package;
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
        Sqlite_Actions _sql;

        private void Run()
        {
            UniversalFunctions.LogToFile("Function Run called...");
            StartupActions();
            Console.WriteLine("Please provide a sentence for me to learn from:\n");

            string sentence = Console.ReadLine();

            /*
             * You can modify anything below here to test your code
             */

            if (SentenceFunctions.Is_Sentence(sentence))
            {
                char[] inputCharArray = sentence.ToCharArray();
            }
            else
            {
                var n = 0;
            }
            // TODO: --3-- consider adding a .where clause that ignores the extra folders we don't care about
        } // function Run;

        #region Startup_Functions
        private void StartupActions()
        {
            UniversalFunctions.LogToFile("Function StartupActions called...");
            UniversalFunctions.Load_Configuration(ref this._config);

            if (this._config == null)
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
            } // if; config is empty or does not exist, it will create it and then save it


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
