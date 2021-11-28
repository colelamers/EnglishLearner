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
        public List<Phrase> ListOfPhrases = new List<Phrase>();
        public List<TrieNode> ListOfNodes = new List<TrieNode>();
        public int ChildNodeCount = 0;
        private TrieNode Root = null, Current = null, Next = null;

        // TODO: --1-- to get sibling nodes at that level, you'll need to grab all the parent keys and the parent dictionary at each child node though...hmm, and point to the same dictionary of the parent node to grab/search a sibling
        // TODO: --4--  this will be when a Trie traversal occurs, it will look to see if other nodes have things it can use for another sentence and add it in. If it doesn't make sense or is incorrect, we can provide input to let it know. DFS or BFS search

        #region Trie Generation
        public Trie(Phrase currentPhrase)
        {
            UniversalFunctions.LogToFile("Trie Constructor called...");

            this.ListOfPhrases.Add(currentPhrase);
            string[] sentence = currentPhrase.Split_Sentence;
            this.Root = new TrieNode(currentPhrase.First_Word, 0, currentPhrase.SentencePattern[0]);
            /*
             * TODO: --2-- YOU CAN PUT put .Next and .Current here instead of making them global. Look into refactoring that!
             * Current = x
             * Next = y
             */
            this.Current = this.Root;
            for (int i = 1; i < sentence.Length; i++)
            {
                TrieNode newNode = new TrieNode(sentence[i], i, currentPhrase.SentencePattern[i]);
                this.Current.Children.Add(newNode.Word, newNode);
                this.Current.Children.TryGetValue(sentence[i], out this.Next);
                this.ChildNodeCount++;

                while (this.Next != null)
                {
                    this.Current = this.Next;
                    this.Current.Children.TryGetValue(sentence[i], out this.Next);
                } // while
            } // for; word in a sentence
            this.Next = null;
            this.Current = null;
        } // Constructor

        public void Append(Phrase currentPhrase, bool dfs_default = true)
        {
            if (this.ListOfPhrases.IndexOf(currentPhrase) < 0)
            { // Means the sentence does not exist
                this.ListOfPhrases.Add(currentPhrase);
            }
            DFS_Append(currentPhrase, this.Root);

            /*          if (dfs_default)
                      {
                      }
                      else
                      {
                          BFS_Append(currentPhrase, this.Root); // TODO: --3-- unsure if this is needed
                      }*/
        } // function Append

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
            DFS_FW(fnNode, this.Root);
        } // function Find

        private void DFS_FW(FindNode fnNode, TrieNode whichNode)
        {
            UniversalFunctions.LogToFile("DFS_FW called...");
            if (fnNode.Index == 0)
            { // update root
                whichNode.WordType = fnNode.Payload.ToString();
            }
            else if (whichNode.NodeDepth < fnNode.Index)
            { // dive down
                DFS_FW(fnNode, whichNode.Children[fnNode.Phrase.Split_Sentence[whichNode.NodeDepth + 1]]);
            }
            else
            { // update node down root path
                whichNode.WordType = fnNode.Payload.ToString();
            }
        } // function DFS_Find_Word

        private void DFS_Find_All_Instances(string findThisWord, TrieNode whichNode)
        {
            UniversalFunctions.LogToFile("DFS_FAI called...");

            this.ListOfNodes = new List<TrieNode>(); // empties it out and creates a new search value stored at the index
            Dictionary<string, TrieNode>.KeyCollection nodeKeys = whichNode.Children.Keys;

            foreach (string key in nodeKeys)
            {
                whichNode.Children.TryGetValue(key, out this.Next);

                if (this.Next != null)
                {
                    if (whichNode.Children[key].Word.Equals(findThisWord) || whichNode.Children[key].Word.Contains(findThisWord))
                    {
                        this.ListOfNodes.Add(whichNode.Children[key]);
                    } // if; node equals or contains the word
                    DFS_Find_All_Instances(findThisWord, this.Next);
                    // TODO: --3-- add a delegate function in here? that way if we want to, we can pass in different tasks like "Print everything" or "Find all of these words" or "find the first instance of this word" or "get the height of this word"
                } // if; next is not null
            } // foreach; key
            this.Next = null;
        } // function DFS_Find_Word

        private void DFS_Find_Sentence(LinkedList<SentenceWord> llSentence, ref LinkedListNode<SentenceWord> lln, TrieNode whichNode)
        {
            UniversalFunctions.LogToFile("DFS_Find_Sentence called...");

            if (whichNode.Children.Keys.Count > 0)
            { // determine if next is null essentially
                foreach (string key in whichNode.Children.Keys)
                {
                    whichNode.Children.TryGetValue(key, out this.Next);
                    whichNode.RecentlyTouched = true;
                    DFS_Find_Sentence(llSentence, ref lln, this.Next);

                    if (llSentence.Count > 0)
                    {
                        lln = new LinkedListNode<SentenceWord>(new SentenceWord(whichNode.WordType, whichNode.Word));
                        llSentence.AddLast(lln);
                        break;
                    } // means we found a false end node, now we can collect the sentence
                } // foreach
            }
            else
            { // end of trie section
                if (whichNode.RecentlyTouched == false)
                { // if end node is false, then add to linked list, otherwise don't
                    whichNode.RecentlyTouched = true;
                    lln = new LinkedListNode<SentenceWord>(new SentenceWord(whichNode.WordType, whichNode.Word));
                    llSentence.AddLast(lln);
                }
            }

            
            this.Next = null;
            //return lln;
        }

        private void ResetTrieTouchedValues(TrieNode whichNode)
        {
            // TODO: --1-- need to perform after a search
            UniversalFunctions.LogToFile("ResetTrieTouchedValues called...");
            whichNode.RecentlyTouched = false;

            foreach (string key in whichNode.Children.Keys)
            {
                whichNode.Children.TryGetValue(key, out this.Next);
                ResetTrieTouchedValues(this.Next);

            } // foreach

            this.Next = null;
            //return lln;
        }

        private void DFS_Append(Phrase currentPhrase, TrieNode whichNode, int iterator = 1)
        {
            // TODO: --1-- when it checks the exact same sentence, it will generate an index out of bounds exception because once it gets to the last node, it will still be recursive so it will be at the max index + 1 when it does the final recursive call and will hit the OOB exception there. if we can resolve this, we can ditch the try-catch block. ??? Could do an array comparison but they'd have to always be consistent.

            if (currentPhrase.Split_Sentence.Length > iterator)
            {
                this.Current = whichNode;
                this.Current.Children.TryGetValue(currentPhrase.Split_Sentence[iterator], out this.Next); // TODO: --3-- can possibly eliminate the iterator by just using the NodeDepth. To catch for Root, you'd just have to do an if check that the NodeDepth > 0. If not, add a new root node.

                if (this.Next != null)
                {
                    DFS_Append(currentPhrase, this.Next, iterator + 1);
                } // if; recursively dive through Trie
                else
                {
                    for (int i = iterator; i < currentPhrase.Split_Sentence.Length; i++)
                    {
                        TrieNode newNode = new TrieNode(currentPhrase.Split_Sentence[i], i, currentPhrase.SentencePattern[i]);
                        this.Current.Children.Add(newNode.Word, newNode);
                        this.Next = this.Current.Children[newNode.Word];
                        this.ChildNodeCount++;

                        while (this.Next != null)
                        {
                            this.Current = this.Next;
                            this.Current.Children.TryGetValue(currentPhrase.Split_Sentence[i], out this.Next);
                            //this.Next = this.Current.Next;// TODO: --1-- need to fix this. we don't want a next node in the TrieNode
                        } // while
                    } // for; word in a currentPhrase.Phrase_Split_Sentence
                } // else; append Trie 
                this.Next = null;
                this.Current = null;
            } // if; iterator == sentence word length, that means everything is a duplicate
        } // function DFS_Traversal; starts after root is chosen elsewhere

        #endregion Depth Search

        // TODO: --1-- may need at Delete everything at this node in case someone types in gibberish So delete at the node past the pipe "|". Ex: I am the | want apples whereof who cats
        // TODO: --1-- alter node function? might want to keep track of height/size/depth then so we can say "grab this phrase, and modify this word at height/depth 'x'". Would need a delete and rebuild node then.

        public void Find_All_Instances(string findThisWord, bool dfs_default = true)
        {
            if (dfs_default)
            {
                DFS_Find_All_Instances(findThisWord, this.Root);
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
        public LinkedListNode<SentenceWord> Find_Sentence(Dictionary<string, Trie> trieDict, bool reset_trie = false)
        { // Returns the word and type association
            LinkedList<SentenceWord> llSentence = new LinkedList<SentenceWord>();
            LinkedListNode<SentenceWord> lln = null;

            if (reset_trie == true)
            {
                foreach (string key in trieDict.Keys)
                { // resets trie for searching
                    ResetTrieTouchedValues(trieDict[key].Root);
                }
            }

            foreach (string key in trieDict.Keys)
            {
                DFS_Find_Sentence(llSentence, ref lln, trieDict[key].Root);
                if (llSentence.Count > 0) { break; }
            }
            while (lln.Previous != null) { lln = lln.Previous; } // while; lln reverted back to root
            return lln;
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
