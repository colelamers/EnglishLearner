using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Data.Sqlite; // NuGet package; Used to work with my Sqlite Code
using ProjectLogging;

namespace EnglishLearner
{
    class Main_View
    {
        Configuration _config = new Configuration();
        DebugLogging _debuglog = new DebugLogging();
        Sqlite_Actions _sql;

        static void Main(string[] args)
        {
            UniversalFunctions.LogToFile("Main Function Called");
            Main_View n = new Main_View();
            n.Run(); // Exists because we can't do some things within a static function like Main so handling everything in a non-static run function
        } // function Main;

        private void Run()
        {
            _debuglog.LogAction("Function Run called...");
            StartupActions();
            
            _config.ProjectFolderPaths = Directory.GetDirectories(_config.SolutionDirectory)
                .Select(d => new { Attr = new DirectoryInfo(d).Attributes, Dir = d })
                .Select(x => x.Dir)
                .ToList(); // Gives us the exact directory paths of all the folders within the the program.

            UniversalFunctions.SaveConfiguration(ref _config);
            // TODO: --3-- consider adding a .where clause that ignores the extra folders we don't care about
        } // function Run;

        private void StartupActions()
        {
            UniversalFunctions.LogToFile("Function StartupActions called...");
            //_debuglog.LogAction("Function StartupActions called...");
            UniversalFunctions.LoadConfiguration(ref _config);

            _config.ConfigPath = "Cole Test";
            _config.ExitCode = 0;
            //ProjectLogging.SetupConfigFile.LoadAndSaveFile(ref _config);
            _config.SolutionDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
            _sql = new Sqlite_Actions(_config.SolutionDirectory + "\\Data", "Dictionary");
        } // function StartupActions;




    }
}
