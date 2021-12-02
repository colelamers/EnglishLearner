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



            if (trieDict.Count == 0)
            {
                List<string> listOfSentences = new List<string>()
                {
                    "Hi, how are you?",
                    "Hi, how are you.",
                    "Hi, how are you today?",
                    "Hi, how are you today sir?",
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
                    try
                    {
                        Phrase whatPhrase = new Phrase(sentence, sqlTransposed);
                        TrieAction(whatPhrase);
                        //this.trieDict = UniversalFunctions.LoadBinaryFile<Dictionary<string, Trie>>(_config.ProjectFolderPaths.ElementAt(2) + $"\\{_config.SaveFileName}");
                    }
                    catch (Exception e)
                    {
                        UniversalFunctions.LogToFile("Something went wrong loading the file...", e);
                    }
                } // foreach

                UniversalFunctions.SaveToBinaryFile(this._config.ProjectFolderPaths.ElementAt(2) + $"\\{this._config.SaveFileName}", this.trieDict);
            }


            Console.WriteLine("==================================================\n" +
                              "==================English Learner=================\n" +
                              "==================================================\n\n");

            bool interact = true;
            while (interact)
            {
                Trie.UpdateAllWords(this.trieDict, "agree", "p");
                MainMenu:
                UniversalFunctions.SaveToBinaryFile(this._config.ProjectFolderPaths.ElementAt(2) + $"\\{this._config.SaveFileName}", this.trieDict);

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
                            bool wantToContinue = true;
                            while (wantToContinue)
                            {
                                Console.WriteLine("Please provide for me a full sentence to learn from. End punctuation is not required, but it can help!\n");
                                Phrase whatPhrase = new Phrase(Console.ReadLine(), sqlTransposed);

                                if (IsSentenceCorrect(whatPhrase))
                                {
                                    TrieAction(whatPhrase);

                                    Console.WriteLine("Would you like to provide another sentence? y/n\n");

                                    char[] validYes = new char[] { 'y', 'Y' };
                                    char[] validNo = new char[] { 'n', 'N' };

                                    var userIn = Console.ReadKey().KeyChar;
                                    while (!validYes.Contains(userIn) || !validNo.Contains(userIn))
                                    {
                                        Console.WriteLine("That is not a proper input, please type \"y\" or \"n\"");
                                    }

                                    if (validNo.Contains(userIn))
                                    {
                                        break;
                                    }
                                } // if; else
                            } // while continuing

                            UniversalFunctions.SaveToBinaryFile(this._config.ProjectFolderPaths.ElementAt(2) + $"\\{this._config.SaveFileName}", this.trieDict);
                            break;
                        case '2':
                            Console.WriteLine("Type the index to correct the TYPE. Options:\n\t\"--exit\": Return to main menu.\n\t\"--help\": Display word types\n\t\"--skip\": Go to next one");
                            string[] wordTypes = new string[] { "A", "C", "D", "J", "N", "P", "V" };

                            // TODO: --2-- i kinda hate this but i'm running out of time so this is just gonna have to do...do it functionally returning trues/falses
                            foreach (string key in this.trieDict.Keys)
                            {
                                int i = 0;
                                while (i < this.trieDict[key].ListOfSentenceArrays.Count)
                                { // foreach sentence, incrementing is based on if there are no "?" in the sentence
                                    LinkedListNode<TrieNode> lln = Trie.Get_Sentence_As_LinkedList(this.trieDict, trieDict[key].ListOfSentenceArrays[i]); // TODO: --3-- might need a null check for this in the future
                                    var temp = lln;
                                    while (temp != null)
                                    {
                                        if (temp.Value.WordType.Equals("?"))
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            temp = temp.Next;
                                        }
                                    }

                                    if (temp == null)
                                    { // looked through list and did not find any "?" for word types, make next sentence 
                                        i++; // Where the only incrementing occurs. Allows for us to skip over searching an index again and verifying the word type has no "?" and then incrementing again
                                        continue;
                                    }

                                    Console.WriteLine("\nHere is a sentence that has some unknowns. Can you correct them?\n");
                                    PrintPhraseInfo(lln);
                                    try
                                    {
                                        Console.WriteLine("What index to correct?");
                                        string decision = Console.ReadLine();
                                        int indexUpdated = 0;

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
                                            PrintPhraseInfo(lln);
                                        }
                                        else if (int.TryParse(decision, out indexUpdated))
                                        { // find index
                                            bool findingNode = true;
                                            while (findingNode)
                                            {
                                                Console.WriteLine("What type should it be?");
                                                string userin = Console.ReadLine();
                                                string typeGiven = userin.ToUpper();

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
                                                { // replace and update node
                                                    var temp_lln = lln;
                                                    TrieNode tNode = null;
                                                    while (temp_lln != null)
                                                    {
                                                        if (temp_lln.Value.NodeDepth == indexUpdated)
                                                        {
                                                            tNode = temp_lln.Value;
                                                            break;
                                                        }
                                                        temp_lln = temp_lln.Next;
                                                    }
                                                    tNode.WordType = typeGiven;
                                                    FindNode fnLookup = new FindNode(this.trieDict[key].ListOfSentenceArrays[i], tNode);
                                                    this.trieDict[key].Update_Node(fnLookup);
                                                } // else
                                            } // while finding the node
                                        } // if
                                    } // try
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Something went wrong. Please see error log for details.\n");
                                        UniversalFunctions.LogToFile("Main Menu Choice 2: Error", e);
                                    } // catch

                                } // for each array item
                            }
                            break;
                        case '3':
                            Console.Write("Hey lets chat! You start.\n");
                            bool doneTalking = false;
                            while (!doneTalking)
                            {
                                string userSentence = Console.ReadLine();
                                Phrase whatPhrase = new Phrase(userSentence, sqlTransposed);

                                if (userSentence.Equals("--exit"))
                                {
                                    break;
                                }
                                else if (IsSentenceCorrect(whatPhrase))
                                {
                                    TrieAction(whatPhrase);
                                    LinkedListNode<TrieNode> lln = Trie.Get_Sentence_As_LinkedList(trieDict, whatPhrase.Split_Sentence); // should be right at the end so we can do the next check
                                    if (lln.Value.KnownResponses.Count > 0)
                                    {
                                        // TODO: --1-- pick a response. Randomly or contextually? idk...
                                        // TODO: spit out a sentence and use command --fix to then tell it certain words are incorrect
                                    }
                                    else
                                    {
                                        // TODO: --1-- read word pattern and then with that information, make an assumption on appropriate responses based on other word patterns.
                                        Console.WriteLine("Hmm, I don't know what to say to that. Can you teach me what I could reply with?");
                                        string responseSentence = Console.ReadLine();
                                        Easy_UpdateNode(responseSentence, lln);
                                    }
                                    // TODO: --1-- for a random response, we'd need a BFS search to look for word types and then traverse accordingly until we get a matching sentence pattern. return it as a sentence, and then create a new phrase for generation
                                } // if; else
                            } // while not done talking
                            
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

        private void Easy_UpdateNode(string createdSentence, LinkedListNode<TrieNode> lln)
        {
            Phrase responsePhrase = new Phrase(createdSentence, sqlTransposed);
            TrieAction(responsePhrase);
            lln.Value.KnownResponses.Add(responsePhrase);
            FindNode tNode = new FindNode(responsePhrase.Split_Sentence, lln.Value);
            trieDict[responsePhrase.First_Word].Update_Node(tNode);
        }

        private bool IsSentenceCorrect(Phrase whatPhrase)
        {
            List<bool> wordTypeUnknown = new List<bool>();
            foreach (string zPatternItem in whatPhrase.SentencePattern)
            {
                if (zPatternItem.Equals("?")) { wordTypeUnknown.Add(true); }
                else { wordTypeUnknown.Add(false); }
            }

            if (!wordTypeUnknown.Contains(false) && wordTypeUnknown.Count > 3)
            {
                Console.WriteLine("I don't think even I can understand that! Sentence has been logged in case there is an error in my skills :)\n");
                UniversalFunctions.LogToFile(whatPhrase.Sentence);
                return false;
            }
            return true;
        }

        private void PrintPhraseInfo(LinkedListNode<TrieNode> list)
        {
            //var lln = Trie.Get_Sentence_As_LinkedList(this.trieDict, zPhrase);
            var temp = list;

            int wordSpaceSizing = temp.Value.Word.Length;

            while (temp.Next != null)
            {
                if (temp.Next.Value.Word.Length > wordSpaceSizing)
                {
                    wordSpaceSizing = temp.Next.Value.Word.Length;
                }
                temp = temp.Next;
            }

            Console.WriteLine("\tIndex\t|\tWord\t|\tType\n");

            while (temp != null)
            {
                int i = temp.Value.NodeDepth;
                Console.Write($"\t{i}\t|    ");

                int differenceInLength = wordSpaceSizing - temp.Value.Word.Length;
                int tackOnExtra = differenceInLength % 2;
                int evenSpaces = (differenceInLength - tackOnExtra) / 2;

                for (int j = 0; j < evenSpaces; j++) { Console.Write(" "); }
                Console.Write(temp.Value.Word);
                for (int j = 0; j < evenSpaces; j++) { Console.Write(" "); }
                if (tackOnExtra > 0) { Console.Write(" "); }

                Console.Write($"   |\t{temp.Value.WordType}\n");
                temp = temp.Previous;
            }
        } // function PrintPhraseInfo (from a linked list)

        private void CombineTrieSaveFiles(Dictionary<string, Trie> otherSave)
        {
            foreach (KeyValuePair<string, Trie> xTries in otherSave)
            {
/*
                foreach (KeyValuePair<string, Phrase> kvpPhrase in otherSave[xTries.Key].DictOfPhrases)
                {
                    TrieAction(this.trieDict, kvpPhrase.Value);
                }*/
            }
        }

        /// <summary>
        /// This is for appending data from multiple save files since they'll contain already prebuilt datasets
        /// </summary>
        /// <param name="primaryTrie"></param>
        /// <param name="zPhrase"></param>
        private void TrieAction(Dictionary<string, Trie> primaryTrie, Phrase zPhrase)
        { // TODO: --1-- need to test. If it works then test appending learned data.
            try
            {
                Trie trieRoot;
                primaryTrie.TryGetValue(zPhrase.First_Word, out trieRoot);
                //this.trieDict.TryGetValue(zPhrase.First_Word, out trieRoot);

                if (trieRoot != null)
                {
                    //this.primaryTrie[zPhrase.First_Word].Append(zPhrase);
                } // if
                else
                {
                    primaryTrie.Add(zPhrase.First_Word, new Trie(zPhrase));
                    //this.trieDict.Add(zPhrase.First_Word, new Trie(zPhrase));
                } // else
            } // try
            catch (Exception e)
            {
                UniversalFunctions.LogToFile("Exception handling Trie. Are you sure your sentence is correct?", e);
            }
        }

        private void TrieAction(Phrase zPhrase)
        {
            try
            {
                if (zPhrase.Sentence_NoPunctuation.Length > 1)
                {
                    Trie trieRoot;
                    this.trieDict.TryGetValue(zPhrase.First_Word, out trieRoot);

                    /*
                     * TODO: --1-- to merge trie stuff
                     * just merge new stuff with mine. mine will be the definite one. all new info will be added to mine but none of my sentences will be overwritten.
                     * 
                     */


                    if (trieRoot != null)
                    {
                        this.trieDict[zPhrase.First_Word].Append(zPhrase);
                    } // if
                    else
                    {
                        this.trieDict.Add(zPhrase.First_Word, new Trie(zPhrase));
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
