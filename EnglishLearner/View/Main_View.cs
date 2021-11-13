using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Data.Sqlite; // NuGet package;
using System.Data;
using EpubSharp;

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
        Sqlite_Actions _sql; // table is called 'entries'
        Dictionary<string, Trie> trieDict = new Dictionary<string, Trie>(); // TODO: --3-- this may need to be stored in a brain class

        private void Run()
        {
            // TODO: --3-- trap in a loop to take user input and other things
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
            string s8 = "The quick brown cat jumped over the happy dog.";
            string s9 = "The quick brown fox jumped over the lazy cat.";
            string s10 = "A quick brown fox jumped over the lazy dog.";
            string s11 = "That quick brown fox jumped over the lazy dog.";
            string s12 = "The eager brown fox jumped over the lazy dog.";
            string s13 = "The crazy red fox jumped over the lazy dog.";
            string s14 = "The quick blue turtle jumped over the lazy dog.";
            string s15 = "The quick purple fox ran over the lazy dog.";
            string s16 = "The quick brown cat jumped around the lazy cat.";
            string s17 = "The quick brown dog jumped over that lazy dog.";
            string s18 = "The quick brown fox leaped over the happy dog.";
            string s19 = "The quick brown fox skipped over the lazy cat.";

            string[] sentences = { s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15, s16, s17, s18, s19 };

            EpubBook book = EpubReader.Read($"{_config.ProjectFolderPaths.ElementAt(0)}\\Burmese Days - George Orwell.epub");
            //char[] splitPunctuation = new char { '.', '!', '?' };
            string[] text = book.ToPlainText().Replace("\n", "").Split(new char[] { '.', '!', '?' });
            //string[] words = book.ToPlainText().Split(" ");

            if (!(trieDict.Count > 0))
            {
                foreach (string sp in text)
                {
                    if (sp.Length > 1)
                    {
                        var nsp = new Phrase(sp);

                        Trie test;
                        trieDict.TryGetValue(nsp.Phrase_First_Word, out test);

                        if (test != null)
                        {
                            trieDict[nsp.Phrase_First_Word].Append(nsp.Phrase_Split_Sentence);
                        }
                        else
                        {
                            trieDict.Add(nsp.Phrase_First_Word, new Trie(nsp.Phrase_Split_Sentence));
                        }
                    }
                }
                UniversalFunctions.SaveToBinaryFile(this._config.ProjectFolderPaths.ElementAt(2) + $"\\{this._config.SaveFileName}", this.trieDict);
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
                this.trieDict = UniversalFunctions.LoadBinaryFile<Dictionary<string, Trie>>(_config.ProjectFolderPaths.ElementAt(2) + $"\\{_config.SaveFileName}");
            } // if; config is empty or does not exist, it will create it and then save it
            else
            {
                this._config = new Configuration();
                this._config.ConfigPath = "Cole Test";
                this._config.ExitCode = 0;
                this._config.SolutionDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
                this._sql = new Sqlite_Actions(this._config.SolutionDirectory + "\\Data", "Dictionary");
                this._config.ProjectFolderPaths = Directory.GetDirectories(this._config.SolutionDirectory)
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
