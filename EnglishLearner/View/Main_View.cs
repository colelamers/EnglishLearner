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
     * SQLite database is rudimentary at best and may not have the correct info
     */
    // TODO: --1-- OPTIMIZATIONS WILL BE NECESSARY LATER ON 
    class Main_View
    {
        Configuration _config = null; // TODO: --1-- need to determine where this should live and be addressed
        Dictionary<string, Trie> trieDict = new Dictionary<string, Trie>(); // TODO: --3-- this may need to be stored in a brain class
        private void Run()
        {
            // TODO: --3-- trap in a loop to take user input and other things
            UniversalFunctions.LogToFile("Function Run called...");
            StartupActions();
            Console.WriteLine("Please provide a sentence for me to learn from:\n");

            // TODO: --1-- ADD TO THIS WHENEVER YOU WANT WITH AS MUCH AS YOU WANT
            List<string> listOfSentences = new List<string>()
            {
                "  It's the, preciou's food!",
                "...", // TODO: --1-- need to catch for elipses, non sentences. so maybe need to do that Is_Sentence assertion somewhere
                "I like the food here?",
                "I like the food here?", // Since it's a dupe, it doesn't add
                "The quick brown fox jumped over the lazy dog.",
                "the slow brown fox jumped over the lazy dog.",
                "The crazy red fox jumped over the lazy dog.",
                "What are you going to do about it boy?",
                "How are you today?",
                "What did you like about the movie?",
                "I really hated that film.",
                "It was just so stupid.",
                "Well I agree with all those points.",
                "The idea of getting 6 million points became a thing, because that's all there was to do.",
                "It's true.",
                "I own several.",
                "Yeah.",
                "You're playing that game still?",
                "It just violates all these laws.",
                "Like it's doing it at the same time, and literally, these particles disappear.",
                "And I go, well it's an open door.",
                "You should totally open a club called Houstons.",
                "They see themselves as global citizens divorced from any allegience.",
                "You know she ran off with Ken, right.",
                "So help yourself.",
                "If I spent my 50 pence on a Mars Bar, it wouldn't be making enough for the day.", // uses a unique noun "mars bar"
                "No don't worry about it, it's just easy.",
                "I'm still struggling with the async functions cause the way my.",
                "I have very good posture."
            };

            DateTime startTime = DateTime.Now; // DO NOT DELETE; logs time to complete

            // Loads in the Trie Data. Do not do until we finalize the Phrase class
            //this.trieDict = UniversalFunctions.LoadBinaryFile<Dictionary<string, Trie>>(_config.ProjectFolderPaths.ElementAt(2) + $"\\{_config.SaveFileName}");

            Dictionary<string, string[]> sqlTransposed = new Dictionary<string, string[]>();
            using (Sqlite_Actions _sql = new Sqlite_Actions(_config.SolutionDirectory + "\\Data", "Dictionary"))
            {
                _sql.ExecuteQuery(@"SELECT word, wordtype FROM entries WHERE word NOT LIKE '''%' AND word NOT LIKE '-%' AND word NOT LIKE ',%' AND word NOT LIKE '\%' ORDER BY word");

                sqlTransposed = _sql.ActiveQueryResults.AsEnumerable()
                    .GroupBy(x => new string(x.Field<string>("word")), y => y.Field<string>("wordtype"))
                    .ToDictionary(x => x.Key, y => y.ToArray()); // Converts datatable into a Dictionary in the way you want it based on the column fields in the datatable
            } // Using Sqlite_Actions to get sql data but feed uniques into a dictionary


            foreach (string sentence in listOfSentences)
            {
                if (sentence.Length > 1) // Catches for blanks or empty strings
                {
                    try
                    {
                        var nsp = new Phrase(sentence, sqlTransposed);
                        //var nsp = new Phrase(sentence, _config.SolutionDirectory + "\\Data");

                        Trie trieRoot;
                        trieDict.TryGetValue(nsp.Phrase_First_Word, out trieRoot);

                        if (trieRoot != null)
                        {
                            trieDict[nsp.Phrase_First_Word].Append(nsp);
                        }
                        else
                        {
                            trieDict.Add(nsp.Phrase_First_Word, new Trie(nsp));
                        }
                    }
                    catch (Exception e)
                    {
                        UniversalFunctions.LogToFile($"Error creating phrase: {sentence}", e);
                    }
                } // if;
            } // foreach
              //UniversalFunctions.SaveToBinaryFile(this._config.ProjectFolderPaths.ElementAt(2) + $"\\{this._config.SaveFileName}", this.trieDict);

            UniversalFunctions.LogToFile($"Time to Complete Trie", startTime);

        } // function Run;

        #region Startup_Functions
        private void StartupActions()
        {
            UniversalFunctions.LogToFile("Function StartupActions called...");
            UniversalFunctions.Load_Configuration(ref this._config);

            if (this._config != null)
            {
                // TODO: --1-- not sure what to do here
            } // if; config is not null
            else
            {
                this._config = new Configuration();
                this._config.ConfigPath = "Cole Test";
                this._config.ExitCode = 0;
                this._config.SolutionDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
                this._config.ProjectFolderPaths = Directory.GetDirectories(this._config.SolutionDirectory)
                    .Select(d => new { Attr = new DirectoryInfo(d).Attributes, Dir = d })
                    .Select(x => x.Dir)
                    .ToList(); // Gives us the exact directory paths of all the folders within the the program.
                UniversalFunctions.Save_Configuration(ref this._config);
            }
/*
            this._sql = new Sqlite_Actions(_config.SolutionDirectory + "\\Data", "Dictionary");
            this._sql.ExecuteQuery(@"SELECT word, wordtype FROM entries WHERE wordtype IS NOT NULL; GROUP BY word ORDER BY word;");
*/
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
