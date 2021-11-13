using System;
using System.Collections.Generic;
using System.Text;

namespace EnglishLearner
{
    /*
     * Created by Cole Lamers 
     * Date: 2021-11-04
     * 
     * == Purpose ==
     * Contains the configuration data for the program
     * 
     * 
     */
    public class Configuration
    {
        //TODO: --1-- need to build up the config file as we go
        public string DictionaryPath { get; set; }
        public string ConfigPath { get; set; }
        public string SolutionDirectory { get; set; }
        public int ExitCode { get; set; } // TODO: --3-- i forgot why i added this in but there was a particular reason
        public List<string> ProjectFolderPaths { get; set; }
        public string SaveFileName { get; set; }

        public Configuration()
        {
            this.DictionaryPath = "";
            this.ConfigPath = "";
            this.SolutionDirectory = "";
            this.ExitCode = 0;
            this.SaveFileName = "englishlearnersavestate.bin";
        } // constructor; default
    }
}
