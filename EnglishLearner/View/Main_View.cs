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

        private void Run()
        {
            _debuglog.LogAction("Function Run called...");
            StartupActions();

            
            List<string> subDirs = Directory.GetDirectories(_config.SolutionDirectory)
                .Select(d => new { Attr = new DirectoryInfo(d).Attributes, Dir = d })
                .Select(x => x.Dir)
                .ToList(); // TODO: --3-- might want this might not. In case we need to reference to the exact directory location of a file in a folder

            Console.WriteLine("Hello World!");
        }

        private void StartupActions()
        {
            _debuglog.LogAction("Function StartupActions called...");
            SetupConfigFile.LoadAndSaveFile(ref _config);
            _config.SolutionDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
            _sql = new Sqlite_Actions(_config.SolutionDirectory + "\\Model", "Dictionary");
        }


        static void Main(string[] args)
        {
            Main_View n = new Main_View();
            n.Run();
        } // Main


    }
}
