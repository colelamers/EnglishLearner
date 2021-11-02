using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Data.Sqlite; // NuGet package; Used to work with my Sqlite Code

namespace EnglishLearner
{
    class Main_View
    {
        Configuration _config = new Configuration(); // TODO: --1-- need to determine where this should live and be addressed
        Sqlite_Actions _sql;

        static void Main(string[] args)
        {
            UniversalFunctions.LogToFile("Main Function Called");

            Main_View n_View = new Main_View();
            n_View.Run(); // Exists because we can't do some things within a static function like Main so handling everything in a non-static run function
        } // function Main;

        private void Run()
        {
            UniversalFunctions.LogToFile("Function Run called...");
            StartupActions();
            
            this._config.ProjectFolderPaths = Directory.GetDirectories(_config.SolutionDirectory)
                .Select(d => new { Attr = new DirectoryInfo(d).Attributes, Dir = d })
                .Select(x => x.Dir)
                .ToList(); // Gives us the exact directory paths of all the folders within the the program.

            UniversalFunctions.Save_Configuration(ref _config);

            // TODO: --3-- consider adding a .where clause that ignores the extra folders we don't care about
        } // function Run;

        private void StartupActions()
        {
            UniversalFunctions.LogToFile("Function StartupActions called...");
            UniversalFunctions.Load_Configuration(ref _config);


            this._config.ConfigPath = "Cole Test";
            this._config.ExitCode = 0;
            this._config.SolutionDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
            this._sql = new Sqlite_Actions(_config.SolutionDirectory + "\\Data", "Dictionary");
        } // function StartupActions;




    }
}
