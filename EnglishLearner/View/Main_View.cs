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
        private Dictionary<string, TrieNode> correctedNodes = new Dictionary<string, TrieNode>(); // TODO: --4-- my cheating way to take care of nodes that get fixed. we just consult this and fix any new ones generated.

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

            Console.WriteLine("==================================================\n" +
                              "==================English Learner=================\n" +
                              "==================================================\n\n");

            // Load

            this.correctedNodes = UniversalFunctions.LoadBinaryFile<Dictionary<string, TrieNode>>(_config.ProjectFolderPaths.ElementAt(2) + "\\correctedNodes.bin");
            this.trieDict = UniversalFunctions.LoadBinaryFile<Dictionary<string, Trie>>(_config.ProjectFolderPaths.ElementAt(2) + $"\\{_config.SaveFileName}");
            bool interact = true;
            while (interact)
            {
                Console.WriteLine("Options:\n\n" +
                                  "1) Add individual Sentence\n" +
                                  "2) Help me with a sentence\n" +
                                  "3) Chat\n" +
                                  "4) Exit\n");
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
                                    wantToContinue = yesOrNo("Would you like to provide another sentence? y/n\n");

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
                                Trie.Reset_Trie_Touches(trieDict);
                                LinkedListNode<TrieNode> lln = Trie.Get_Sentence_As_LinkedList(this.trieDict, new LinkedList<TrieNode>());

                                while (lln != null)
                                {
                                    int llnDepth = lln.List.Count;

                                    Console.WriteLine("\nHere is a sentence that has some unknowns. Can you correct them?\n");
                                    PrintPhraseInfo(lln);
                                    try
                                    {
                                        UserHitenter:
                                        Console.WriteLine("What index to correct?");
                                        string decision = Console.ReadLine();
                                        int indexUpdated = 0;

                                        if (decision.Equals(""))
                                        {
                                            Console.WriteLine("Improper Input! Please type something else.");
                                            goto UserHitenter;
                                        }
                                        else if (decision.ToLower().Equals("--exit"))
                                        { // checks in case the user wishes to exit prematurely
                                            goto EndTask;
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
                                        else if (int.TryParse(decision, out indexUpdated) && indexUpdated < llnDepth)
                                        { // find index
                                            bool findingNode = true;
                                            while (findingNode)
                                            {
                                                RetryNodeFix:
                                                Console.WriteLine("What type should it be?");
                                                string userin = Console.ReadLine();
                                                string typeGiven = userin.ToUpper();

                                                if (userin.ToLower().Equals("--help"))
                                                {
                                                    Console.WriteLine("\n\tHelp: What type of word should this be?\nSentence Pattern:\nA: Definite article\nC: Conjunction\nD: Adverb\nJ: Adjective\nN: Noun\nP: Preposition\nV: Verb\n");
                                                }
                                                else if (userin.ToLower().Equals("--exit"))
                                                {
                                                    goto EndTask;
                                                }
                                                else if (userin.Equals("") || !wordTypes.Contains(userin.ToUpper()))
                                                {
                                                    Console.WriteLine("Improper Input! Please type something else.");
                                                    goto RetryNodeFix;
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
                                                        temp_lln = temp_lln.Previous;
                                                    }
                                                    tNode.WordType = typeGiven;

                                                    this.correctedNodes.Add(tNode.Word, tNode);
                                                    Trie.UpdateWordTypes(this.trieDict, tNode); // update all words means we don't have to just update the singular node
                                                    lln = null;
                                                    findingNode = false;
                                                } // else
                                            } // while finding the node
                                        } // if
                                    } // try
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Something went wrong. Please see error log for details.\n");
                                        UniversalFunctions.LogToFile("Main Menu Choice 2: Error", e);
                                    } // catch
                                }
                            }
                            EndTask:
                            UniversalFunctions.SaveToBinaryFile(this._config.ProjectFolderPaths.ElementAt(2) + "\\correctedNodes.bin", this.correctedNodes);
                            UniversalFunctions.SaveToBinaryFile(this._config.ProjectFolderPaths.ElementAt(2) + $"\\{this._config.SaveFileName}", this.trieDict);
                            break;
                        case '3':
                            Console.Write("\nHey lets chat! You start.\n");
                            bool doneTalking = false;
                            while (!doneTalking)
                            {
                                Console.WriteLine("\nYour turn:\n");
                                UserTypedEnter:
                                string userSentence = Console.ReadLine();
                                Phrase whatPhrase = new Phrase(userSentence, sqlTransposed);

                                try
                                {
                                    if (userSentence.Equals("--exit"))
                                    {
                                        doneTalking = false;
                                        break;
                                    }
                                    else if (userSentence.Equals("") || whatPhrase.Sentence.Length < 1)
                                    {
                                        Console.WriteLine("\nImproper Input! Please type something else.\n");
                                        goto UserTypedEnter;
                                    }
                                    else
                                    {
                                        TrieAction(whatPhrase);

                                        LinkedListNode<TrieNode> lln = Trie.Get_Sentence_As_LinkedList(this.trieDict, whatPhrase.Split_Sentence); // should be right at the end so we can do the next check

                                        if (lln != null)
                                        {
                                            if (userSentence.Equals("--exit"))
                                            {
                                                doneTalking = false;
                                                break;
                                            }
                                            else if (lln.Value.Legal_KnownResponses != null && lln.Value.Legal_KnownResponses.Count > 0)
                                            {
                                                Random rng = new Random();
                                                int rngNumber = rng.Next(0, lln.Value.Legal_KnownResponses.Count);

                                                Console.Write("Random Response: ");
                                                var randomSentence = Trie.ReturnRandomSentenceFromPattern(this.trieDict, string.Join("", whatPhrase.SentencePattern).ToCharArray()); // update all words means we don't have to just update the singular node

                                                Console.WriteLine(randomSentence + "\n");
                                                if(yesOrNo("Was that sentence any good?\n"))
                                                {
                                                    Console.WriteLine("\nNice! I'll remember that one.\n");
                                                    AddKnownResponse(randomSentence, lln);
                                                }
                                                else
                                                {
                                                    Console.WriteLine("\nWow...English is not an easy language!\n");
                                                    AddIllegalResponse(randomSentence, lln);
                                                }

                                                Console.WriteLine("\nHe're is a sentence I do know how to respond with!\n");
                                                Console.WriteLine(lln.Value.Legal_KnownResponses[rngNumber].Sentence + "\n");

                                                Console.WriteLine("\nCan you teach me another way to respond?\n");
                                                string responseSentence = Console.ReadLine();
                                                if (responseSentence.Equals("\n--exit\n"))
                                                {
                                                    doneTalking = false;
                                                    break;
                                                }
                                                AddKnownResponse(responseSentence, lln);
                                            }
                                            else
                                            {
                                                Console.Write("\nRandom Response: \n");
                                                var randomSentence = Trie.ReturnRandomSentenceFromPattern(this.trieDict, string.Join("", whatPhrase.SentencePattern).ToCharArray()); // update all words means we don't have to just update the 
                                                Console.WriteLine(randomSentence);

                                                if (yesOrNo("Was that sentence any good?"))
                                                {
                                                    Console.WriteLine("Nice! I'll remember that one.\n");
                                                    AddKnownResponse(randomSentence, lln);
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Wow...English is not an easy language!\n");
                                                    AddIllegalResponse(randomSentence, lln);
                                                }

                                                Console.WriteLine("\nHmm, I don't know what to say to that. Can you teach me what I could reply with?\n");
                                                string responseSentence = Console.ReadLine();
                                                AddKnownResponse(responseSentence, lln);
                                            }
                                        } // if

                                    } // if; else
                                }
                                catch (Exception e)
                                {
                                    break;
                                    Console.WriteLine("Something went wring with Case 3: Chatting. See log file for details.");
                                    UniversalFunctions.LogToFile("Case 3: Chatting Exception:", e);
                                }
                                
                            } // while not done talking
                            UniversalFunctions.SaveToBinaryFile(this._config.ProjectFolderPaths.ElementAt(2) + $"\\{this._config.SaveFileName}", this.trieDict);
                            break;
                        case '4':
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
            UniversalFunctions.SaveToBinaryFile(this._config.ProjectFolderPaths.ElementAt(2) + $"\\{this._config.SaveFileName}", this.trieDict);
        } // function Run;

        /// <summary>
        /// True = yes, False = no;
        /// </summary>
        /// <param name="consoleOutput"></param>
        /// <returns></returns>
        private bool yesOrNo(string consoleOutput)
        {
            Console.WriteLine(consoleOutput);

            char[] validYes = new char[] { 'y', 'Y' };
            char[] validNo = new char[] { 'n', 'N' };
            bool wantToContinue = true;

            var userIn = Console.ReadKey().KeyChar;
            while (wantToContinue)
            {
                if (validNo.Contains(userIn))
                {
                    wantToContinue = false;
                    break;
                }
                else if (validYes.Contains(userIn))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("That is not a proper input, please type \"y\" or \"n\"");
                    userIn = Console.ReadKey().KeyChar;
                }
            }
            return wantToContinue; // basically only finishes when you don't want to continue
        }

        private void AddKnownResponse(string createdSentence, LinkedListNode<TrieNode> lln)
        {
            Phrase responsePhrase = new Phrase(createdSentence, sqlTransposed);
            TrieAction(responsePhrase);
            if (lln.Value.Legal_KnownResponses == null)
            {
                lln.Value.Legal_KnownResponses = new List<Phrase>();
            }
            lln.Value.Legal_KnownResponses.Add(responsePhrase);
            FindNode tNode = new FindNode(responsePhrase.Split_Sentence, lln.Value);
            trieDict[responsePhrase.First_Word].Update_Node(tNode);
        }

        private void AddIllegalResponse(string createdSentence, LinkedListNode<TrieNode> lln)
        {
            Phrase responsePhrase = new Phrase(createdSentence, sqlTransposed);
            TrieAction(responsePhrase);
            if (lln.Value.Legal_KnownResponses == null)
            {
                lln.Value.Illegal_KnownResponses = new List<Phrase>();
            }
            lln.Value.Illegal_KnownResponses.Add(responsePhrase);
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

            // Determines the proper order of the list 
            if (list.Previous == null)
            { // list is in reverse order due to recursion adding
                while (temp.Next != null)
                {
                    if (temp.Next.Value.Word.Length > wordSpaceSizing)
                    {
                        wordSpaceSizing = temp.Next.Value.Word.Length;
                    }
                    temp = temp.Next;
                }
            }
            else
            { // list is in proper order
                while (temp.Previous != null)
                {
                    if (temp.Previous.Value.Word.Length > wordSpaceSizing)
                    {
                        wordSpaceSizing = temp.Previous.Value.Word.Length;
                    }
                    temp = temp.Previous;
                }
                temp = list; 
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

                if (list.Previous == null)
                {
                    temp = temp.Next;
                }
                else
                {
                    temp = temp.Previous;

                }
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

                    if (trieRoot != null)
                    {
                        this.trieDict[zPhrase.First_Word].Append(zPhrase);
                    } // if
                    else
                    {
                        this.trieDict.Add(zPhrase.First_Word, new Trie(zPhrase));
                    } // else
                } // if; sentence length > 1

                // Fixes word types if they've been revised before
                for (int i = 0; i < zPhrase.Split_Sentence.Length; i++)
                { // Updates all the nodes for the new phrase. Very bad O() but short on time...
                    if (zPhrase.SentencePattern[i].Equals("?"))
                    { // Find an unknown and check if the word has been updated before
                        TrieNode test = null;
                        correctedNodes.TryGetValue(zPhrase.Split_Sentence[i], out test);

                        if (test != null)
                        { // if it has a correction, update the Trie again
                            Trie.UpdateWordTypes(this.trieDict, test);
                        }
                    }
                } // for

                UniversalFunctions.SaveToBinaryFile(this._config.ProjectFolderPaths.ElementAt(2) + $"\\{this._config.SaveFileName}", this.trieDict);

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
