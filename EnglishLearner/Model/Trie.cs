using System;
using System.Collections.Generic;

namespace EnglishLearner
{
    /*
     * Created by Cole Lamers 
     * Date: 2021-11-04
     * 
     * == Purpose ==
     * To store sentence memories of sentences it has been taught. Future thoughts
     * are to store additional data for each node of word information so that the 
     * Trie can do DFS or BFS traversal looking for additional words it could
     * supplement in the event of an illegal or improper word.
     * 
     * Requires at least one word to traverse
     */
    [Serializable]
    class Trie
    {
        public List<Phrase> ListOfPhrases = new List<Phrase>(); // TODO: --1- SERIOUSLY CONSIDER REVISING THIS TO CONTAIN THE SENTENCE AND PUNCTUATION TO PULL PROPER PHRASE!
        public List<TrieNode> ListOfNodes = new List<TrieNode>();
        public int ChildNodeCount = 0;
        private TrieNode Root = null;

        #region Trie Generation
        public Trie(Phrase currentPhrase)
        {
            // TODO: --1-- consider making the end punctuation the keys for the dictionaryies. That way you'll always know what keys exist, and can easily retrieve the words and never have to be concerned with end punctuation as it's known at the top and can be ignored easily.
            UniversalFunctions.LogToFile("Trie Constructor called...");

            this.ListOfPhrases.Add(currentPhrase);
            string[] sentence = currentPhrase.Split_Sentence;
            this.Root = new TrieNode(currentPhrase.First_Word, 0, currentPhrase.SentencePattern[0]);
            TrieNode Current = this.Root; // TODO: --4-- can most likely add the dictionary here instead of having it declared elsewhere
            TrieNode Next = null;
            /*
             * TODO: --2-- YOU CAN PUT put .Next and .Current here instead of making them global. Look into refactoring that!
             * Current = x
             * Next = y
             */
            //Current = this.Root;
            for (int i = 1; i < sentence.Length; i++)
            {
                TrieNode newNode = new TrieNode(sentence[i], i, currentPhrase.SentencePattern[i]);
                Current.Children.Add(newNode.Word, newNode);
                Current.Children.TryGetValue(sentence[i], out Next);
                this.ChildNodeCount++;

                while (Next != null)
                {
                    Current = Next;
                    Current.Children.TryGetValue(sentence[i], out Next);
                } // while
            } // for; word in a sentence

            if (Next == null)
            {
                Current.CanBeLastWord = true;
                //Current.
            }
        } // Constructor

        public void Append(Phrase currentPhrase)
        {
            this.ListOfPhrases.Add(currentPhrase);
            DFS_Append(currentPhrase, this.Root);
        } // function Append

        private void DFS_Append(Phrase currentPhrase, TrieNode whichNode, int iterator = 1)
        {
            if (currentPhrase.Split_Sentence.Length > iterator)
            {
                TrieNode Next = null;
                whichNode.Children.TryGetValue(currentPhrase.Split_Sentence[iterator], out Next); // TODO: --3-- can possibly eliminate the iterator by just using the NodeDepth. To catch for Root, you'd just have to do an if check that the NodeDepth > 0. If not, add a new root node.

                if (Next != null)
                { // recursively dive through Trie
                    DFS_Append(currentPhrase, Next, iterator + 1);
                }
                else
                {
                    for (int i = iterator; i < currentPhrase.Split_Sentence.Length; i++)
                    {
                        TrieNode newNode = new TrieNode(currentPhrase.Split_Sentence[i], i, currentPhrase.SentencePattern[i]);
                        whichNode.Children.Add(newNode.Word, newNode);
                        Next = whichNode.Children[newNode.Word];
                        this.ChildNodeCount++;

                        while (Next != null)
                        {
                            whichNode = Next;
                            whichNode.Children.TryGetValue(currentPhrase.Split_Sentence[i], out Next);
                        } // while
                    } // for; word in a currentPhrase.Split_Sentence

                    if (Next == null)
                    {
                        whichNode.CanBeLastWord = true;
                    }
                } // else; append Trie 
            } // if; iterator == sentence word length, that means everything is a duplicate
        } // function DFS_Traversal; starts after root is chosen elsewhere


        #endregion Trie Generation

        #region Breadth Search

        private void BFS_FAI(string findThisWord, TrieNode whichNode)
        {
            // TODO: --1-- build this
        }


        public void BFS_Find_Word(string findThisWord)
        {
            //DFS_FAI(findThisWord, this.Root);
        } // function Find

        private void BFS_FW(string findThisWord, TrieNode whichNode)
        {
            UniversalFunctions.LogToFile("BFS_FW called...");

            this.ListOfNodes = new List<TrieNode>(); // empties it out and creates a new search value stored at the index
            Dictionary<string, TrieNode>.KeyCollection nodeKeys = whichNode.Children.Keys;
        }

        #endregion Breadth Search

        #region Depth Search

        public void DFS_Update_WordType(FindNode fnNode)
        {
            DFS_Find_Word(fnNode, this.Root);
        } // function Find

        private void DFS_Find_Word(FindNode fnNode, TrieNode whichNode)
        {
            UniversalFunctions.LogToFile("DFS_FW called...");
            if (fnNode.Index == 0)
            { // update root
                whichNode.WordType = fnNode.Payload.ToString();
            }
            else if (whichNode.NodeDepth < fnNode.Index)
            { // dive down
                DFS_Find_Word(fnNode, whichNode.Children[fnNode.Phrase.Split_Sentence[whichNode.NodeDepth + 1]]);
            }
            else
            { // update node down root path
                whichNode.WordType = fnNode.Payload.ToString();
            }
        } // function DFS_Find_Word

        private void DFS_Find_All_Word_Instances(string findThisWord, TrieNode whichNode)
        {
            UniversalFunctions.LogToFile("DFS_FAI called...");

            this.ListOfNodes = new List<TrieNode>(); // empties it out and creates a new search value stored at the index
            Dictionary<string, TrieNode>.KeyCollection nodeKeys = whichNode.Children.Keys;
            TrieNode Next = null;

            foreach (string key in nodeKeys)
            {
                whichNode.Children.TryGetValue(key, out Next);

                if (Next != null)
                {
                    if (whichNode.Children[key].Word.Equals(findThisWord) || whichNode.Children[key].Word.Contains(findThisWord))
                    {
                        this.ListOfNodes.Add(whichNode.Children[key]);
                    } // if; node equals or contains the word
                    DFS_Find_All_Word_Instances(findThisWord, Next);
                    // TODO: --3-- add a delegate function in here? that way if we want to, we can pass in different tasks like "Print everything" or "Find all of these words" or "find the first instance of this word" or "get the height of this word"
                } // if; next is not null
            } // foreach; key
        } // function DFS_Find_Word

        private static void DFS_Find_Sentence(LinkedList<SentenceWord> llSentence, ref LinkedListNode<SentenceWord> lln, TrieNode whichNode)
        {
            UniversalFunctions.LogToFile("DFS_Find_Sentence called...");
            TrieNode Next = null;

            if (whichNode.Children.Keys.Count > 0)
            { // determine if next is null

                if (whichNode.CanBeLastWord == true && whichNode.RecentlyTouched)
                { // found a node that can be a final word, but not as a final branch node
                    whichNode.RecentlyTouched = true;
                    lln = new LinkedListNode<SentenceWord>(new SentenceWord(whichNode.WordType, whichNode.Word));
                    llSentence.AddLast(lln);
                }
                else
                { // Not a final word from a phrase given
                    whichNode.RecentlyTouched = true;
                    foreach (string key in whichNode.Children.Keys)
                    {
                        whichNode.Children.TryGetValue(key, out Next);

                        DFS_Find_Sentence(llSentence, ref lln, Next);

                        if (llSentence.Count > 0)
                        { // means we found a false end node, now we can collect the sentence
                            lln = new LinkedListNode<SentenceWord>(new SentenceWord(whichNode.WordType, whichNode.Word));
                            llSentence.AddLast(lln);
                            break;
                        } // if
                    } // foreach
                }
            } // if
            else
            { // end of Trie branch
                if (whichNode.RecentlyTouched == false)
                { // if end node is false, then add to linked list, otherwise don't
                    whichNode.RecentlyTouched = true;
                    lln = new LinkedListNode<SentenceWord>(new SentenceWord(whichNode.WordType, whichNode.Word));
                    llSentence.AddLast(lln);
                }
            } // else
        } // function DFS_Find_Sentence

        private static void ResetTrieTouchedValues(TrieNode whichNode)
        {
            // TODO: --1-- need to perform after a search
            UniversalFunctions.LogToFile("ResetTrieTouchedValues called...");
            whichNode.RecentlyTouched = false;
            TrieNode Next = null;

            foreach (string key in whichNode.Children.Keys)
            {
                whichNode.Children.TryGetValue(key, out Next);
                ResetTrieTouchedValues(Next);
            } // foreach
        }

        #endregion Depth Search

        // TODO: --1-- may need at Delete everything at this node in case someone types in gibberish So delete at the node past the pipe "|". Ex: I am the | want apples whereof who cats
        // TODO: --1-- alter node function? might want to keep track of height/size/depth then so we can say "grab this phrase, and modify this word at height/depth 'x'". Would need a delete and rebuild node then.

        public void Find_All_Instances(string findThisWord, bool dfs_default = true)
        {
            // TODO: --3-- idk what todo but this isn't done
            if (dfs_default)
            {
                DFS_Find_All_Word_Instances(findThisWord, this.Root);
            }
            else
            {
                BFS_FAI(findThisWord, this.Root);
            }
        }

        /// <summary>
        /// Finds a new sentence that hasn't been touched recently.
        /// </summary>
        /// <param name="trieDict"></param>
        /// <param name="reset_trie"></param>
        /// <example> var n = trieDict[key].Find_Sentence(trieDict, true); var f = trieDict[key].Find_Sentence(trieDict);</example>
        /// <returns>Returns a linked list containing the sentence, although from the tail of the LinkedList due to recursion. So remember, previous is actually the next word, next is the previous word.</returns>
        public static string Find_Sentence(Dictionary<string, Trie> trieDict, bool reset_trie = false)
        { // Returns the word and type association
            LinkedList<SentenceWord> llSentence = new LinkedList<SentenceWord>();
            LinkedListNode<SentenceWord> lln = null;

            if (reset_trie == true)
            { // changes all "touched" bools to false
                foreach (string key in trieDict.Keys)
                { // resets trie for searching
                    ResetTrieTouchedValues(trieDict[key].Root);
                }
            }

            string rebuildSentence = "";
            foreach (string key in trieDict.Keys)
            {
                for (int i = 0; i < trieDict[key].ListOfPhrases.Count; i++)
                { // each phrase  
                    DFS_Find_Sentence(llSentence, ref lln, trieDict[key].Root);
                    if (llSentence.Count > 0)  
                    {
                        rebuildSentence = lln.Value.CurrentWord;
                        while (lln.Previous != null)
                        {
                            rebuildSentence += " ";
                            rebuildSentence += lln.Previous.Value.CurrentWord;
                            lln = lln.Previous;
                        }

                        List<string> punctuationTest = new List<string>() { ".", "?", "!" };
                        foreach (string punct in punctuationTest)
                        {
                            string temp = rebuildSentence;
                            temp += punct;

                            
                        }
                        break; 
                    }
                }
            }


            //while (lln.Previous != null) { lln = lln.Previous; } // while; lln reverted back to root
            return rebuildSentence;
        }

        private void PrintPhraseInfo(Phrase zPhrase)
        { // TODO: --3-- maybe ditch this
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

  
    } // Class Trie
} // namespace
