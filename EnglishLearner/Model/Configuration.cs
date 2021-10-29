using System;
using System.Collections.Generic;
using System.Text;

namespace EnglishLearner
{
    public class Configuration
    {
        //TODO: --1-- need to build up the config file as we go
        public string DictionaryPath { get; set; }
        public string ConfigPath { get; set; }
        public string SolutionDirectory { get; set; }
        public int ExitCode { get; set; }
        public List<string> ProjectFolderPaths { get; set; }
        public Configuration()
        {
            DictionaryPath = "";
            ConfigPath = "";
            SolutionDirectory = "";
            ExitCode = 1;
        } // Constructor; default
    }
}
