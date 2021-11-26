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
     * tree can do DFS or BFS traversal looking for additional words it could
     * supplement in the event of an illegal or improper word.
     * 
     * Requires at least one word to traverse
     */
    [Serializable]
    class Trie
    {
        public List<string[]> ListOfPatterns = new List<string[]>(); // TODO: --3-- this is just going to be used for a quick inference by the program to go "hey this seems similar, let me try this!"
        public List<TrieNode> ListOfNodes = new List<TrieNode>();
        public List<Phrase> ListOfPhrases = new List<Phrase>();
        public int ChildNodeCount = 0;
        private TrieNode Root = null, Current = null, Next = null;

        //public Dictionary<Phrase, Phrase> Sentence_Info { get; set; } // TODO: --3-- most likely will need this at each tree root for info about corresponding sentences
        // TODO: --1-- to get sibling nodes at that level, you'll need to grab all the parent keys and the parent dictionary at each child node though...hmm, and point to the same dictionary of the parent node to grab/search a sibling
        // TODO: --4--  this will be when a tree traversal occurs, it will look to see if other nodes have things it can use for another sentence and add it in. If it doesn't make sense or is incorrect, we can provide input to let it know. DFS or BFS search

        public Trie(Phrase currentPhrase)
        {
            UniversalFunctions.LogToFile("Tree Constructor called...");

            ListOfPhrases.Add(currentPhrase);
            string[] sentence = currentPhrase.Phrase_Split_Sentence;
            this.Root = new TrieNode(sentence[0], 0, currentPhrase.SentencePattern[0]);

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

            // TODO: --3-- not sure these are needed
            this.Next = null;
            this.Current = null;
        } // Constructor


        public void FindBFS(string findThisWord)
        {
            DFS_Find_Word(findThisWord, this.Root);
        } // function Find

        private void BFS_Find_Word(string findThisWord, TrieNode whichNode)
        {
            UniversalFunctions.LogToFile("DFS_Find_Word called...");

            this.ListOfNodes = new List<TrieNode>(); // empties it out and creates a new search value stored at the index
            Dictionary<string, TrieNode>.KeyCollection nodeKeys = whichNode.Children.Keys;


        }

        // TODO: --3-- may need at Delete everything at this node in case someone types in gibberish So delete at the node past the pipe "|". Ex: I am the | want apples whereof who cats
        // TODO: --3-- alter node function? might want to keep track of height/size/depth then so we can say "grab this phrase, and modify this word at height/depth 'x'". Would need a delete and rebuild node then.
        public void FindDFS(string findThisWord)
        {
            DFS_Find_Word(findThisWord, this.Root);
        } // function Find



        private void DFS_Find_Word(string findThisWord, TrieNode whichNode)
        {
            UniversalFunctions.LogToFile("DFS_Find_Word called...");

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
                    DFS_Find_Word(findThisWord, this.Next);
                    // TODO: --3-- add a delegate function in here? that way if we want to, we can pass in different tasks like "Print everything" or "Find all of these words" or "find the first instance of this word" or "get the height of this word"
                } // if; next is not null
            } // foreach; key
            this.Next = null;
        } // function DFS_Find_Word

        public void Append(Phrase currentPhrase)
        {
            this.ListOfPatterns.Add(currentPhrase.SentencePattern);
            this.ListOfPhrases.Add(currentPhrase); // TODO: --1-- need to not add these if there is a duplicate

            DFS_Append(currentPhrase, this.Root);
        } // function Append

        private void DFS_Append(Phrase currentPhrase, TrieNode whichNode, int iterator = 1)
        {
            // TODO: --1-- when it checks the exact same sentence, it will generate an index out of bounds exception because once it gets to the last node, it will still be recursive so it will be at the max index + 1 when it does the final recursive call and will hit the OOB exception there. if we can resolve this, we can ditch the try-catch block. ??? Could do an array comparison but they'd have to always be consistent.
            
            if (currentPhrase.Phrase_Split_Sentence.Length > iterator)
            {
                this.Current = whichNode;
                this.Current.Children.TryGetValue(currentPhrase.Phrase_Split_Sentence[iterator], out this.Next);

                if (this.Next != null)
                {
                    DFS_Append(currentPhrase, this.Next, iterator + 1);
                } // if; recursively dive through tree
                else
                {
                    for (int i = iterator; i < currentPhrase.Phrase_Split_Sentence.Length; i++)
                    {
                        TrieNode newNode = new TrieNode(currentPhrase.Phrase_Split_Sentence[i], i, currentPhrase.SentencePattern[i]);
                        this.Current.Children.Add(newNode.Word, newNode);
                        this.Next = this.Current.Children[newNode.Word];
                        this.ChildNodeCount++;

                        while (this.Next != null)
                        {
                            this.Current = this.Next;
                            this.Current.Children.TryGetValue(currentPhrase.Phrase_Split_Sentence[i], out this.Next);
                            //this.Next = this.Current.Next;// TODO: --1-- need to fix this. we don't want a next node in the TreeNode
                        } // while
                    } // for; word in a currentPhrase.Phrase_Split_Sentence
                } // else; append tree 
                this.Next = null;
                this.Current = null;
            } // if; iterator == sentence word length, that means everything is a duplicate
        } // function DFS_Traversal; starts after root is chosen elsewhere
    } // Class Tree
} // namespace
