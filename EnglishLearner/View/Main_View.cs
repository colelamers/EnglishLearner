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
    class Main_View
    {
        private Configuration _config = null;
        private Dictionary<string, Trie> trieDict = new Dictionary<string, Trie>(); // TODO: --3-- this may need to be stored in a brain class
        private Dictionary<string, string[]> sqlTransposed = new Dictionary<string, string[]>();

        private void Run()
        {
            // TODO: --3-- trap in a loop to take user input and other things
            UniversalFunctions.LogToFile("Function Run called...");
            StartupActions();
            using (Sqlite_Actions _sql = new Sqlite_Actions(_config.SolutionDirectory + "\\Data", "Dictionary"))
            {
                _sql.ExecuteQuery(@"SELECT word, wordtype FROM entries WHERE word NOT LIKE '''%' AND word NOT LIKE '-%' AND word NOT LIKE ',%' AND word NOT LIKE '\%' ORDER BY word");

                this.sqlTransposed = _sql.ActiveQueryResults.AsEnumerable()
                    .GroupBy(x => new string(x.Field<string>("word")), y => y.Field<string>("wordtype"))
                    .ToDictionary(x => x.Key, y => y.Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToArray()); // Converts datatable into a Dictionary in the way you want it based on the column fields in the datatable
            } // Using Sqlite_Actions to get sql data but feed uniques into a dictionary

            try
            {
                //this.trieDict = UniversalFunctions.LoadBinaryFile<Dictionary<string, Trie>>(_config.ProjectFolderPaths.ElementAt(2) + $"\\{_config.SaveFileName}");
            }
            catch (Exception e)
            {
                UniversalFunctions.LogToFile("Something went wrong loading the file...", e);
            }

            if (trieDict.Count == 0)
            {
                List<string> listOfSentences = new List<string>()
                {
/*                    "Hi, how are you?",
                    "Hey, how is it going?",
                    "What's your name?",*/
                    "Hello, I am a sentence learning AI.",
                    "I know a few things already, so lets chat and increase my memory!",
                    "Yeah.",
                    "Uh huh",
                    "He happilly sang in the choir!",
                    "  It's the, preciou's food!",
                    "...",
                    "I like the food here?",
                    "I like the food here?",  //Since it's a dupe, it doesn't add
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
                    "If I spent my 50 pence on a Mars Bar, it wouldn't be making enough for the day.",  //uses a unique noun "mars bar"
                    "No don't worry about it, it's just easy.",
                    "I'm still struggling with the async functions cause the way my.",
                    "I have very good posture."
                };

                foreach (string sentence in listOfSentences)
                {
                    TrieAction(sentence);
                } // foreach

                UniversalFunctions.SaveToBinaryFile(this._config.ProjectFolderPaths.ElementAt(2) + $"\\{this._config.SaveFileName}", this.trieDict);
            }

            bool interact = true;
            while (interact)
            {

                Console.WriteLine("==================================================\n" +
                                  "==================English Learner=================\n" +
                                  "==================================================\n\n");
                MainMenu: // go back here
                Console.WriteLine("...Options:\n\n" +
                                  "1) Add individual Sentence\n" +
                                  "2) Help me with a sentence\n" +
                                  "3) Chat\n" +
                                  "4) Find a word I might know\n" + // TODO: --1-- the find function may not be great right now
                                  "5) Exit\n");

                try
                {
                    switch (Console.ReadKey().KeyChar)
                    {
                        case '1':
                            Console.Write("Please provide for me a full sentence to learn from. End punctuation is not required, but it can help!\n");
                            TrieAction(Console.ReadLine());
                            break;
                        case '2':
                            bool fixedASentence = false;

                            foreach (string key in trieDict.Keys)
                            {
                                foreach (Phrase zPhrase in trieDict[key].ListOfPhrases)
                                {
                                    Console.WriteLine("\nHere is a sentence that has some unknowns. Can you correct them?\n");
                                    if (zPhrase.SentencePattern.Contains("?"))
                                    {
                                        int wordSpaceSizing = 0;
                                        foreach (string word in zPhrase.Phrase_Split_Sentence)
                                        {
                                            if (word.Length > wordSpaceSizing)
                                            {
                                                wordSpaceSizing = word.Length;
                                            }
                                        } // foreach; gets the largest sized word to make pretty printing

                                        Console.WriteLine("\tIndex\t|\tWord\t|\tType\n");

                                        for (int i = 0; i < zPhrase.Phrase_Split_Sentence.Length; i++)
                                        {
                                            Console.Write($"\t{i}\t|    ");

                                            int differenceInLength = wordSpaceSizing - zPhrase.Phrase_Split_Sentence[i].Length;
                                            int tackOnExtra = differenceInLength % 2;
                                            int evenSpaces = (differenceInLength - tackOnExtra) / 2;

                                            for (int j = 0; j < evenSpaces; j++) { Console.Write(" "); }
                                            Console.Write(zPhrase.Phrase_Split_Sentence[i]);
                                            for (int j = 0; j < evenSpaces; j++) { Console.Write(" "); }
                                            if (tackOnExtra > 0) { Console.Write(" "); }

                                            Console.Write($"   |\t{zPhrase.SentencePattern[i]}\n");
                                        } // for; calculates difference between max word size and current word size to print pretty
                                    } // if; contains "?"

                                    Console.WriteLine("Type the word or the index to correct that word. Type \"exit\" to leave.\n");
                                    while (zPhrase.SentencePattern.Contains("?") || !Console.ReadLine().Equals("exit"))
                                    {
                                        // TODO: --1-- come back to this to figure out what to do when processing a word. Need to find the node in the trie, then update the phrase, and the node. need to revise find to only find one occurance at a location, not every single one.
                                        try
                                        {
                                            var decision = Console.ReadLine();

                                            if (decision.Length == 1)
                                            { // find index

                                            }
                                            else
                                            { // find word

                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine("Something went wrong. Please see error log for details.\n");
                                            UniversalFunctions.LogToFile("Main Menu Choice 2: Error", e);
                                        }

                                    } // while; pattern contains "?"
                                } // foreach; Phrase
                            } // foreach; trieDict key
                            break;
                        case '3':
                            // TODO: --1-- provide a "how would you respond to this statement/question?" and then cache a Trie with direct responses to something in particular maybe.
                            break;
                        case '4':
                            break;
                        case '5':
                            interact = false;
                            break;
                    } // switch
                    goto MainMenu; // goes back to the beginning of the app again
                }
                catch (Exception e)
                {
                    Console.WriteLine("Uh oh, it seems like you entered an improper input. Please try again.");
                    UniversalFunctions.LogToFile("Exception at Main Menu.", e);
                }
                
            } // while

        } // function Run;

        private void TrieAction(string sentence)
        {
            try
            {
                if (sentence.Length > 1)
                {
                    var nsp = new Phrase(sentence, this.sqlTransposed);
                    Trie trieRoot;
                    trieDict.TryGetValue(nsp.Phrase_First_Word, out trieRoot);

                    if (trieRoot != null)
                    {
                        trieDict[nsp.Phrase_First_Word].Append(nsp);
                    } // if
                    else
                    {
                        trieDict.Add(nsp.Phrase_First_Word, new Trie(nsp));
                    } // else
                } // if; sentence length > 1
            } // try
            catch (Exception e)
            {
                UniversalFunctions.LogToFile("Exception handling Trie. Are you sure your sentence is correct?", e);
            }
        }

        #region Startup_Functions
        private void StartupActions()
        {
            UniversalFunctions.LogToFile("Function StartupActions called...");
            UniversalFunctions.Load_Configuration(ref this._config);

            if (this._config == null)
            {
                this._config = new Configuration();
                this._config.ConfigPath = "Cole Test";
                this._config.SolutionDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
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
