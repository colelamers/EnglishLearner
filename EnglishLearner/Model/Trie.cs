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
        //public Dictionary<string, Phrase> DictOfPhrases = new Dictionary<string, Phrase>();
        public List<string[]> ListOfSentenceArrays = new List<string[]>();
        //public List<TrieNode> ListOfNodes = new List<TrieNode>();
        public int MaxNodeLevel = 0;
        public TrieNode Root { get; private set; } // TODO: --4-- when you get the chance, fix how this works. dictionary of tries or something should most definitely be here.

        #region Trie Generation
        public Trie(Phrase currentPhrase)
        {
            UniversalFunctions.LogToFile("Trie Constructor called...");

            this.ListOfSentenceArrays.Add(currentPhrase.Split_Sentence);
            this.Root = new TrieNode(currentPhrase.First_Word, 0, currentPhrase.SentencePattern[0]);
            TrieNode Current = this.Root; // TODO: --4-- can most likely add the dictionary here instead of having it declared elsewhere
            TrieNode Next = null;

            for (int i = 1; i < currentPhrase.Split_Sentence.Length; i++)
            {
                TrieNode newNode = new TrieNode(currentPhrase.Split_Sentence[i], i, currentPhrase.SentencePattern[i]);
                Current.Children.Add(newNode.Word, newNode);
                Current.Children.TryGetValue(currentPhrase.Split_Sentence[i], out Next);

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
                Current.KnownResponses = new List<Phrase>();

                if (this.MaxNodeLevel < Current.NodeDepth)
                {
                    this.MaxNodeLevel = Current.NodeDepth;
                }
            }
        } // Constructor

        public void Append(Phrase currentPhrase)
        {
            this.ListOfSentenceArrays.Add(currentPhrase.Split_Sentence);
            DFS_Append(currentPhrase, this.Root);
        } // function Append

        private void DFS_Append(Phrase currentPhrase, TrieNode Current)
        {
            if (currentPhrase.Split_Sentence.Length > Current.NodeDepth + 1)
            {
                TrieNode Next = null;
                Current.Children.TryGetValue(currentPhrase.Split_Sentence[Current.NodeDepth + 1], out Next);
                // TODO: --2-- can combine this with the trie creation possibly?

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

                        while (Next != null)
                        {
                            Current = Next;
                            Current.Children.TryGetValue(currentPhrase.Split_Sentence[i], out Next);
                        } // while
                    } // for; word in a currentPhrase.Split_Sentence

                    if (Next == null)
                    { // Final Node level updates
                        Current.CanBeLastWord = true;
                        Current.TypesOfPunctuation += (Current.TypesOfPunctuation.Contains(currentPhrase.Punctuation) ? "" : currentPhrase.Punctuation);
                        Current.KnownResponses = new List<Phrase>();

                        if (this.MaxNodeLevel < Current.NodeDepth)
                        {
                            this.MaxNodeLevel = Current.NodeDepth;
                        }
                    } // if
                } // else; append Trie 
            } // if
            else if (Current.CanBeLastWord == true)
            { // adds another ending punctuation if the same sentence with a different ending punctuation comes up
                Current.TypesOfPunctuation += (Current.TypesOfPunctuation.Contains(currentPhrase.Punctuation) ? "" : currentPhrase.Punctuation);
            }

        } // function DFS_Traversal; starts after root is chosen elsewhere


        #endregion Trie Generation

        #region Breadth Search
        public static void UpdateAllWords(Dictionary<string, Trie> trieDict, string wordToUpdate, string wordTypeToUpdate)
        { // Due to the nature of how I have the trieDict set up, I have to do a DFS/BFS hybrid. DFS by Trie, but BFS by node. Hence the two foreach loops.
            Reset_Trie_Touches(trieDict);
            Queue<TrieNode> bfs_queue = new Queue<TrieNode>();

            foreach (string tKey in trieDict.Keys)
            { // update all node level keys
                if (tKey.Equals(wordToUpdate))
                {
                    trieDict[tKey].Root.WordType = wordTypeToUpdate;
                }

                bfs_queue.Enqueue(trieDict[tKey].Root);

                while (bfs_queue.Count > 0)
                {
                    var dequed = bfs_queue.Dequeue();

                    foreach (string childKey in dequed.Children.Keys)
                    {
                        if (dequed.Children[childKey].Word.Equals(wordToUpdate))
                        {
                            dequed.Children[childKey].WordType = wordTypeToUpdate.ToUpper();
                        }
                        bfs_queue.Enqueue(dequed.Children[childKey]);
                    } // each Queue item
                } // while
            }
        } // function Update All Words 

        private static void BFS_UpdateAllWords(TrieNode whichNode, string wordToUpdate, string wordTypeToUpdate)
        {
            Queue<TrieNode> bfs_queue = new Queue<TrieNode>();
            bfs_queue.Enqueue(whichNode);

            while (bfs_queue.Count > 0)
            {
                var dequed = bfs_queue.Dequeue();

                foreach (string childKey in dequed.Children.Keys)
                { // Update all child nodes at the key
                    if (dequed.Children[childKey].Word.Equals(wordToUpdate))
                    {
                        dequed.Children[childKey].WordType = wordTypeToUpdate;
                    }
                    bfs_queue.Enqueue(dequed.Children[childKey]);
                }
            }
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

            if (whichNode.NodeDepth < fnNode.NodePayload.NodeDepth && fnNode.NodePayload.NodeDepth != 0)
            { // dive down
                DFS_Update_Node(fnNode, whichNode.Children[fnNode.SentenceArray[whichNode.NodeDepth + 1]]);
            }
            else
            { // update node
                whichNode = fnNode.NodePayload;
            }
        } // function DFS_Find_Word

        private void DFS_Find_All_Word_Instances(string findThisWord, ref List<TrieNode> listOfNodes, TrieNode whichNode)
        {
            UniversalFunctions.LogToFile("DFS_FAI called...");

            Dictionary<string, TrieNode>.KeyCollection nodeKeys = whichNode.Children.Keys;
            TrieNode Next = null;

            foreach (string key in nodeKeys)
            {
                whichNode.Children.TryGetValue(key, out Next);

                if (Next != null)
                {
                    if (whichNode.Children[key].Word.Equals(findThisWord) || whichNode.Children[key].Word.Contains(findThisWord))
                    {
                        listOfNodes.Add(whichNode.Children[key]);
                    } // if; node equals or contains the word
                    DFS_Find_All_Word_Instances(findThisWord, ref listOfNodes, Next);
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

        private static void DFS_Find_Sentence_With_SentenceArray(LinkedList<TrieNode> llSentence, ref LinkedListNode<TrieNode> lln, TrieNode whichNode, string[]  sentenceArray)
        { // The reason .RecentlyTouched is in here three times is because it can't be touched
          // until it commits to further recursion or ending it. Otherwise it will always be true.
            UniversalFunctions.LogToFile("DFS_Find_Sentence_With_Phrase called...");
 
            if (whichNode.NodeDepth + 1 < sentenceArray.Length) // whichNode.NodeDepth + 1 = the first word after the root word initially passed in
            {
                TrieNode Next = null;
                if (whichNode.Children.Keys.Count > 0)
                {
                    whichNode.Children.TryGetValue(sentenceArray[whichNode.NodeDepth + 1], out Next);
                    DFS_Find_Sentence_With_SentenceArray(llSentence, ref lln, Next, sentenceArray);
                }
            }

            whichNode.RecentlyTouched = true;
            lln = new LinkedListNode<TrieNode>(whichNode);
            llSentence.AddLast(lln);
        } // function DFS_Find_Sentence

        #endregion Depth Search

        // TODO: --1-- may need at Delete everything at this node in case someone types in gibberish So delete at the node past the pipe "|". Ex: I am the | want apples whereof who cats

        public List<TrieNode> Find_All_Instances(string findThisWord, bool dfs_default = true)
        {
            List<TrieNode> listOfNodes = new List<TrieNode>();
            // TODO: --3-- idk what todo but this isn't done
            if (dfs_default)
            {
                DFS_Find_All_Word_Instances(findThisWord, ref listOfNodes, this.Root);
            }
            else
            {
                //BFS_FAI(findThisWord, this.Root);
            }
            return listOfNodes;
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

        /// <summary>
        /// Resets all touched values to false via DFS. USE THIS ONE FOR RESETTING!
        /// </summary>
        /// <param name="trieDict"></param>
        public static void Reset_Trie_Touches(Dictionary<string, Trie> trieDict)
        {
            foreach (string key in trieDict.Keys)
            { // resets trie for searching
                ResetTrieTouchedValues(trieDict[key].Root);
            }
        } // function Reset_Trie_Touches

/*
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
                for (int i = 0; i < trieDict[key].ListOfSentenceArrays.Count; i++)
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
                            // TODO: --3-- if i plan on using this again, you need to loop through the list unfortunately
                            //trieDict[key].ListOfSentenceArrays.TryGetValue(temp, out tTest);

                            if (tTest != null)
                            {
                                lln.Value.SearchedPunctuation += punct;

                                FindNode fnNode = new FindNode(tTest.Split_Sentence, lln.Value);
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
*/
        public static LinkedListNode<TrieNode> Get_Sentence_As_LinkedList(Dictionary<string, Trie> trieDict, string[] sentenceArray)
        { // Returns the word and type association
            LinkedList<TrieNode> llSentence = new LinkedList<TrieNode>();
            LinkedListNode<TrieNode> lln = null;

            Trie tempTrie = null;
            trieDict.TryGetValue(sentenceArray[0], out tempTrie);

            if (tempTrie != null)
            {
                DFS_Find_Sentence_With_SentenceArray(llSentence, ref lln, tempTrie.Root, sentenceArray);
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
  
    } // Class Trie
} // namespace
