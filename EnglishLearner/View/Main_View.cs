using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Data.Sqlite; // NuGet package;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Text;

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
                this.trieDict = UniversalFunctions.LoadBinaryFile<Dictionary<string, Trie>>(_config.ProjectFolderPaths.ElementAt(2) + $"\\{_config.SaveFileName}");
            }
            catch (Exception e)
            {
                UniversalFunctions.LogToFile("Something went wrong loading the file...", e);
            }

            if (trieDict.Count == 0)
            {
                List<string> listOfSentences = new List<string>()
                {
                    "Hi, how are you?",
                    "Hey, how is it going?",
                    "What's your name?",
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


            Console.WriteLine("==================================================\n" +
                              "==================English Learner=================\n" +
                              "==================================================\n\n");

            bool interact = true;
            while (interact)
            {
                MainMenu:
                Console.WriteLine("Options:\n\n" +
                                  "1) Add individual Sentence\n" +
                                  "2) Help me with a sentence\n" +
                                  "3) Chat\n" +
                                  "4) Find a word I might know\n" +
                                  "5) Exit\n");
                try
                {
                    switch (Console.ReadKey().KeyChar)
                    {
                        case '1':
                            Console.Write("Please provide for me a full sentence to learn from. End punctuation is not required, but it can help!\n");
                            TrieAction(Console.ReadLine());
                            UniversalFunctions.SaveToBinaryFile(this._config.ProjectFolderPaths.ElementAt(2) + $"\\{this._config.SaveFileName}", this.trieDict);
                            break;
                        case '2':
                            Console.WriteLine("Type the index to correct the TYPE. Options:\n\t\"--exit\": Return to main menu.\n\t\"--help\": Display word types\n\t\"--skip\": Go to next one");
                            string[] wordTypes = new string[] { "A", "C", "D", "J", "N", "P", "V" };

                            // TODO: --2-- i kinda hate this but i'm running out of time so this is just gonna have to do...do it functionally returning trues/falses
                            foreach (string key in trieDict.Keys)
                            {
                                foreach (Phrase zPhrase in trieDict[key].ListOfPhrases) // for would be : trieDict[key].ListOfPhrases[i] 
                                { // Each Phrase
                                    int phraseIndex = trieDict[key].ListOfPhrases.IndexOf(zPhrase);
                                    bool updateHappened = false;
                                    int indexUpdated = 0;
                                    string typeGiven = "";

                                    // TODO: --1-- currently hardcoded only to fix sentences, not to revise them later. might need the DFS_Find() with the linked list for that one instead
                                    while (zPhrase.SentencePattern.Contains("?")) 
                                    {
                                        Console.WriteLine("\nHere is a sentence that has some unknowns. Can you correct them?\n");
                                        PrintPhraseInfo(zPhrase);
                                        try
                                        {
                                            Console.WriteLine("What index to correct?");
                                            string decision = Console.ReadLine();

                                            if (decision.ToLower().Equals("--exit"))
                                            { // checks in case the user wishes to exit prematurely
                                                goto MainMenu;
                                            }
                                            else if (decision.Equals("--skip"))
                                            { // skips to the next iteration
                                                continue;
                                            }
                                            else if (decision.ToLower().Equals("--help"))
                                            {
                                                Console.WriteLine("\n\tHelp: Select the index number of the word you wish to correct.\n");
                                            }
                                            else if (decision.ToLower().Equals("--print"))
                                            {
                                                PrintPhraseInfo(zPhrase);
                                            }
                                            else if (int.TryParse(decision, out indexUpdated))
                                            { // find index
                                                bool findingNode = true;
                                                while (findingNode)
                                                {
                                                    Console.WriteLine("What type should it be?");
                                                    string userin = Console.ReadLine();
                                                    typeGiven = userin.ToUpper();

                                                    if (typeGiven.ToLower().Equals("--help"))
                                                    {
                                                        Console.WriteLine("\n\tHelp: What type of word should this be?\nSentence Pattern:\nA: Definite article\nC: Conjugation\nD: Adverb\nJ: Adjective\nN: Noun\nP: Preposition\nV: Verb\n");
                                                    }
                                                    else if (typeGiven.ToLower().Equals("--exit"))
                                                    {
                                                        goto MainMenu;
                                                    }
                                                    else if (!wordTypes.Contains(typeGiven))
                                                    {
                                                        Console.WriteLine("Not a valid input. Please use --help to see valid TYPE keys.\n");
                                                    }
                                                    else
                                                    {
                                                        FindNode fnLookup = new FindNode(zPhrase, indexUpdated, typeGiven);
                                                        trieDict[key].DFS_Update_WordType(fnLookup);
                                                        findingNode = false;
                                                        updateHappened = true;
                                                    } // else
                                                } // while
                                            } // if
                                        } // try
                                        catch (Exception e)
                                        {
                                            Console.WriteLine("Something went wrong. Please see error log for details.\n");
                                            UniversalFunctions.LogToFile("Main Menu Choice 2: Error", e);
                                        } // catch

                                        if (updateHappened && indexUpdated != -1)
                                        { // Updates the trie
                                            trieDict[key].ListOfPhrases[phraseIndex].SentencePattern[indexUpdated] = typeGiven;
                                            zPhrase.SentencePattern[indexUpdated] = typeGiven;

                                            // Resets data
                                            updateHappened = false;
                                            indexUpdated = -1;
                                            typeGiven = "";
                                        }
                                    } // while
                                } // foreach
                                UniversalFunctions.SaveToBinaryFile(this._config.ProjectFolderPaths.ElementAt(2) + $"\\{this._config.SaveFileName}", this.trieDict);
                            } // foreach; trieDict key
                            Console.WriteLine("All indexes updated!\n");
                            break;
                        case '3':
                            Console.Write("Hey lets chat!\n");
                            bool doneTalking = false;
                            while (!doneTalking)
                            {
                                // TODO: --1-- should hanlde the removing/deleting of a word before implementing
                                TrieAction(Console.ReadLine());
                                //var reply = trieDict[]
                            }
                            
                            UniversalFunctions.SaveToBinaryFile(this._config.ProjectFolderPaths.ElementAt(2) + $"\\{this._config.SaveFileName}", this.trieDict);
                            break;
                        case '4':
                            // TODO: --1-- work on this
                            break;
                        case '5':
                            interact = false;
                            break;
                    } // switch
                } // try
                catch (Exception e)
                {
                    Console.WriteLine("Uh oh, it seems like you entered an improper input. Please try again.");
                    UniversalFunctions.LogToFile("Exception at Main Menu.", e);
                }
            } // while; main menu
        } // function Run;

        private void PrintPhraseInfo(Phrase zPhrase)
        {
            int wordSpaceSizing = 0;
            foreach (string word in zPhrase.Split_Sentence)
            {
                if (word.Length > wordSpaceSizing)
                {
                    wordSpaceSizing = word.Length;
                }
            } // foreach; gets the largest sized word to make pretty printing

            Console.WriteLine("\tIndex\t|\tWord\t|\tType\n");

            for (int i = 0; i < zPhrase.Split_Sentence.Length; i++)
            {
                Console.Write($"\t{i}\t|    ");

                int differenceInLength = wordSpaceSizing - zPhrase.Split_Sentence[i].Length;
                int tackOnExtra = differenceInLength % 2;
                int evenSpaces = (differenceInLength - tackOnExtra) / 2;

                for (int j = 0; j < evenSpaces; j++) { Console.Write(" "); }
                Console.Write(zPhrase.Split_Sentence[i]);
                for (int j = 0; j < evenSpaces; j++) { Console.Write(" "); }
                if (tackOnExtra > 0) { Console.Write(" "); }

                Console.Write($"   |\t{zPhrase.SentencePattern[i]}\n");
            } // for; calculates difference between max word size and current word size to print pretty
        }

        private void TrieAction(string sentence)
        {
            try
            {
                if (sentence.Length > 1)
                {
                    var nsp = new Phrase(sentence, this.sqlTransposed);
                    Trie trieRoot;
                    this.trieDict.TryGetValue(nsp.First_Word, out trieRoot);

                    /*
                     * TODO: --1-- to merge trie stuff
                     * just merge new stuff with mine. mine will be the definite one. all new info will be added to mine but none of my sentences will be overwritten.
                     * 
                     */


                    if (trieRoot != null)
                    {
                        this.trieDict[nsp.First_Word].Append(nsp);
                    } // if
                    else
                    {
                        this.trieDict.Add(nsp.First_Word, new Trie(nsp));
                    } // else
                } // if; sentence length > 1
            } // try
            catch (Exception e)
            {
                UniversalFunctions.LogToFile("Exception handling Trie. Are you sure your sentence is correct?", e);
            }
        }

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
    } // class
} // namespace
