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
        public string DictionaryPath { get; set; }
        public string ConfigPath { get; set; }
        public string SolutionDirectory { get; set; }
        public List<string> ProjectFolderPaths { get; set; }
        public string SaveFileName { get; set; }

        public Configuration()
        {
            this.DictionaryPath = "";
            this.ConfigPath = "";
            this.SolutionDirectory = "";
            this.SaveFileName = "englishlearnersavestate.bin";
        } // constructor; default
    }
}
