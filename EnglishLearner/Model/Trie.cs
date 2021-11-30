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
        public Dictionary<string, Phrase> DictOfPhrases = new Dictionary<string, Phrase>();
        public List<TrieNode> ListOfNodes = new List<TrieNode>();
        public int ChildNodeCount = 0;
        public TrieNode Root { get; private set; }

        #region Trie Generation
        public Trie(Phrase currentPhrase)
        {
            UniversalFunctions.LogToFile("Trie Constructor called...");

            this.DictOfPhrases.Add(currentPhrase.Sentence, currentPhrase);
            this.Root = new TrieNode(currentPhrase.First_Word, 0, currentPhrase.SentencePattern[0]);
            TrieNode Current = this.Root; // TODO: --4-- can most likely add the dictionary here instead of having it declared elsewhere
            TrieNode Next = null;

            for (int i = 1; i < currentPhrase.Split_Sentence.Length; i++)
            {
                TrieNode newNode = new TrieNode(currentPhrase.Split_Sentence[i], i, currentPhrase.SentencePattern[i]);
                Current.Children.Add(newNode.Word, newNode);
                Current.Children.TryGetValue(currentPhrase.Split_Sentence[i], out Next);
                this.ChildNodeCount++;

                while (Next != null)
                {
                    Current = Next;
                    Current.Children.TryGetValue(currentPhrase.Split_Sentence[i], out Next);
                } // while
            } // for; word in a sentence

            if (Next == null)
            {
                Current.CanBeLastWord = true;
                Current.TypesOfPunctuation += currentPhrase.Punctuation;
                Current.KnownResponses = new List<string>();
            }
        } // Constructor

        public void Append(Phrase currentPhrase)
        {
            this.DictOfPhrases.Add(currentPhrase.Sentence, currentPhrase);
            DFS_Append(currentPhrase, this.Root);
        } // function Append

        private void DFS_Append(Phrase currentPhrase, TrieNode Current)
        {
            if (currentPhrase.Split_Sentence.Length > Current.NodeDepth + 1)
            {
                TrieNode Next = null;
                Current.Children.TryGetValue(currentPhrase.Split_Sentence[Current.NodeDepth + 1], out Next); // TODO: --3-- can possibly eliminate the iterator by just using the NodeDepth. To catch for Root, you'd just have to do an if check that the NodeDepth > 0. If not, add a new root node.

                if (Next != null)
                { // recursively dive through Trie
                    DFS_Append(currentPhrase, Next);
                }
                else
                {
                    for (int i = Current.NodeDepth + 1; i < currentPhrase.Split_Sentence.Length; i++)
                    {
                        TrieNode newNode = new TrieNode(currentPhrase.Split_Sentence[i], i, currentPhrase.SentencePattern[i]);
                        Current.Children.Add(newNode.Word, newNode);
                        Next = Current.Children[newNode.Word];
                        this.ChildNodeCount++;

                        while (Next != null)
                        {
                            Current = Next;
                            Current.Children.TryGetValue(currentPhrase.Split_Sentence[i], out Next);
                        } // while
                    } // for; word in a currentPhrase.Split_Sentence

                    if (Next == null)
                    {
                        Current.CanBeLastWord = true;
                        Current.TypesOfPunctuation += (Current.TypesOfPunctuation.Contains(currentPhrase.Punctuation) ? "" : currentPhrase.Punctuation);
                        Current.KnownResponses = new List<string>();
                    }


                } // else; append Trie 
            } // if; iterator == sentence word length, that means everything is a duplicate
            else if (Current.CanBeLastWord == true)
            { // adds another ending punctuation if the same sentence with a different ending punctuation comes up
                Current.TypesOfPunctuation += (Current.TypesOfPunctuation.Contains(currentPhrase.Punctuation) ? "" : currentPhrase.Punctuation);
            }

        } // function DFS_Traversal; starts after root is chosen elsewhere


        #endregion Trie Generation

        #region Breadth Search

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

        /// <summary>
        /// Update a node by choosing the specific function
        /// </summary>
        /// <param name="fnNode"></param>
        /// <param name="updateWhat">Choice of "wordtype" or "node" and it will update accordingly</param>
        public void Update_Node(FindNode fnNode)
        {
            /*
            How to update a node in a trie

                             foreach (string key in Trie)
                             {
                                 foreach (KeyValuePair<string, Phrase> kvpPhrase in PhraseDictionary) // for would be : trieDict[key].ListOfPhrases[i] 
                                 { 
                                     LinkedListNode<TrieNode> lln = Trie.Get_Sentence_As_LinkedList(this.Trie, kvpPhrase.Value);
                                     while (lln.Next != null)
                                     {
                                        lln = lln.Next
                                     }
                                     FindNode node = new FindNode(kvpPhrase.Value, lln.Value);
                                     trieDict[key].Update_Node(node, "node");
            */
            DFS_Update_Node(fnNode, this.Root);

        } // function Find
/*
 * TODO: --4-- hold onto for now but i don't know that i'll keep this
        private void DFS_Update_WordType(FindNode fnNode, TrieNode whichNode)
        {
            UniversalFunctions.LogToFile("DFS_FW called...");

            if (whichNode.NodeDepth < fnNode.Index && fnNode.Index != 0)
            { // dive down
                DFS_Update_WordType(fnNode, whichNode.Children[fnNode.Phrase.Split_Sentence[whichNode.NodeDepth + 1]]);
            }
            else
            { // update node
                whichNode.WordType = fnNode.NodeMemberPayload.ToString();
            }
        } // function DFS_Find_Word
*/
        private void DFS_Update_Node(FindNode fnNode, TrieNode whichNode)
        {
            UniversalFunctions.LogToFile("DFS_FW called...");

            if (whichNode.NodeDepth < fnNode.Index && fnNode.Index != 0)
            { // dive down
                DFS_Update_Node(fnNode, whichNode.Children[fnNode.Phrase.Split_Sentence[whichNode.NodeDepth + 1]]);
            }
            else
            { // update node
                whichNode = fnNode.NodePayload;
                this.DictOfPhrases[fnNode.Phrase.Sentence] = fnNode.Phrase;
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

        private static void DFS_Find_Sentence(LinkedList<TrieNode> llSentence, ref LinkedListNode<TrieNode> lln, TrieNode whichNode)
        { // The reason .RecentlyTouched is in here three times is because it can't be touched
          // until it commits to further recursion or ending it. Otherwise it will always be true.
            UniversalFunctions.LogToFile("DFS_Find_Sentence called...");
            TrieNode Next = null;

            if (whichNode.Children.Keys.Count > 0)
            { // determine if next is null
                // TODO: --4-- after the first if, when trying to find sentences with multiple forms of ending punctuation, it will default to the else meaning it will go recursive when it should end but with the different type of ending punctuation. not very important but just something to try to fix at a later date.
                if (whichNode.CanBeLastWord == true && !whichNode.RecentlyTouched)
                { // found a node that can be a final word, but not as a final branch node
                    whichNode.RecentlyTouched = true;
                    lln = new LinkedListNode<TrieNode>(whichNode);
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
                            lln = new LinkedListNode<TrieNode>(whichNode);
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
                    lln = new LinkedListNode<TrieNode>(whichNode);
                    llSentence.AddLast(lln);
                }
            } // else
        } // function DFS_Find_Sentence

        private static void DFS_Find_Sentence_With_Phrase(LinkedList<TrieNode> llSentence, ref LinkedListNode<TrieNode> lln, TrieNode whichNode, Phrase zPhrase)
        { // The reason .RecentlyTouched is in here three times is because it can't be touched
          // until it commits to further recursion or ending it. Otherwise it will always be true.
            UniversalFunctions.LogToFile("DFS_Find_Sentence_With_Phrase called...");
 
            if (whichNode.NodeDepth + 1 < zPhrase.Split_Sentence.Length)
            {
                TrieNode Next = null;
                if (whichNode.Children.Keys.Count > 0)
                {
                    whichNode.Children.TryGetValue(zPhrase.Split_Sentence[whichNode.NodeDepth + 1], out Next);
                    DFS_Find_Sentence_With_Phrase(llSentence, ref lln, Next, zPhrase);
                }
            }

            lln = new LinkedListNode<TrieNode>(whichNode);
            llSentence.AddLast(lln);
        } // function DFS_Find_Sentence

        #endregion Depth Search

        // TODO: --1-- may need at Delete everything at this node in case someone types in gibberish So delete at the node past the pipe "|". Ex: I am the | want apples whereof who cats

        public void Find_All_Instances(string findThisWord, bool dfs_default = true)
        {
            // TODO: --3-- idk what todo but this isn't done
            if (dfs_default)
            {
                DFS_Find_All_Word_Instances(findThisWord, this.Root);
            }
            else
            {
                //BFS_FAI(findThisWord, this.Root);
            }
        }

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

        public static void Reset_Trie_Touches(Dictionary<string, Trie> trieDict, bool reset_trie)
        {
            foreach (string key in trieDict.Keys)
            { // resets trie for searching
                ResetTrieTouchedValues(trieDict[key].Root);
            }
        } // function Reset_Trie_Touches

        /// <summary>
        /// Finds a new sentence that hasn't been touched recently.
        /// </summary>
        /// <param name="trieDict"></param>
        /// <param name="reset_trie"></param>
        /// <example> var n = trieDict[key].Find_Sentence(trieDict, true); var f = trieDict[key].Find_Sentence(trieDict);</example>
        /// <returns>Returns a linked list containing the sentence, although from the tail of the LinkedList due to recursion. So remember, previous is actually the next word, next is the previous word.</returns>
        public static string Find_Sentence(Dictionary<string, Trie> trieDict)
        { // Returns the word and type association
            // TODO: --1-- what is this?
            LinkedList<TrieNode> llSentence = new LinkedList<TrieNode>();
            LinkedListNode<TrieNode> lln = null;

            string rebuildSentence = "";
            foreach (string key in trieDict.Keys)
            {
                for (int i = 0; i < trieDict[key].DictOfPhrases.Count; i++)
                { // each phrase  
                    DFS_Find_Sentence(llSentence, ref lln, trieDict[key].Root);
                    if (llSentence.Count > 0)  
                    {
                        rebuildSentence = lln.Value.Word;
                        while (lln.Previous != null)
                        {
                            rebuildSentence += " ";
                            rebuildSentence += lln.Previous.Value.Word;
                            lln = lln.Previous;
                        }

                        foreach (char punct in lln.Value.TypesOfPunctuation)
                        {
                            if (lln.Value.SearchedPunctuation.Contains(punct))
                            { // the retainer of an already searched node that has different punctuation
                                continue;
                            }
                            string temp = rebuildSentence;
                            temp += punct;

                            Phrase tTest = null;
                            trieDict[key].DictOfPhrases.TryGetValue(temp, out tTest);

                            if (tTest != null)
                            {
                                lln.Value.SearchedPunctuation += punct;

                                FindNode fnNode = new FindNode(tTest, lln.Value);
                                trieDict[key].Update_Node(fnNode);
                                rebuildSentence = temp;
                                goto GotSentence; // we got one and we're done
                            }
                        } // foreach
                    } // if count > 0
                } // for ListOfPhrases count
            } // foreach key
            GotSentence:
            return rebuildSentence;
        }

        public static LinkedListNode<TrieNode> Get_Sentence_As_LinkedList(Dictionary<string, Trie> trieDict, Phrase zPhrase)
        { // Returns the word and type association
            LinkedList<TrieNode> llSentence = new LinkedList<TrieNode>();
            LinkedListNode<TrieNode> lln = null;

            Trie tempTrie = null;
            trieDict.TryGetValue(zPhrase.First_Word, out tempTrie);

            if (tempTrie != null)
            {
                DFS_Find_Sentence_With_Phrase(llSentence, ref lln, tempTrie.Root, zPhrase);
                if (llSentence.Count > 0)
                {
                    while (lln.Previous != null)
                    {
                        lln = lln.Previous;
                    }
                }
            }

            return lln;
        } // function Get_Sentence_As_LinkedList

        private void PrintPhraseInfo(Dictionary<string, Trie> xTrie, Phrase zPhrase)
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
