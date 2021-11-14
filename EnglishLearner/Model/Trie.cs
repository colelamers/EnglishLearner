using System;
using System.Collections.Generic;
using System.Text;

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
        public Phrase NodePhrase = null;
        private TrieNode Root = null;
        private TrieNode Current = null, Next = null;
        public List<TrieNode> listOfNodes = new List<TrieNode>();

        public List<Phrase> ListOfPhrases = new List<Phrase>();
        //public int Depth = 0; // TODO: --3-- implement a depth/height/length/size?
        //public Dictionary<Phrase, Phrase> Sentence_Info { get; set; } // TODO: --3-- most likely will need this at each tree root for info about corresponding sentences
        // TODO: --1-- should probably be in model and not controller. make a brain class again but contain the tree and certain functions?
        // public List<string> Illegal_Terms { get; set; } // TODO: --4--  this will be when a tree traversal occurs, it will look to see if other nodes have things it can use for another sentence and add it in. If it doesn't make sense or is incorrect, we can provide input to let it know. DFS or BFS search.
        // TODO: --4-- need to retain illegal values in nodes specifically because a single word does not negate the legitimacy of an entire sentence.
        // TODO: --4-- add lists of known subject, verb, objects that it can interpret from if there is an illegal value somewhere.
        // TODO: --1-- were going to want the program to learn from a pattern of types of words. as it builds up the amount of those patterns that are acceptable, it will inference those as appropriate word order patterns that it can speak in other ways with other words of that type. So like a list or something that is ADV-N-V-P-C-P-V, PN-C-ADJ-V-N, etc
        // NOTE: Key based traversal is DFS, Deciding on which available child nodes 
        // TODO: --1-- to get sibling nodes at that level, you'll need to grab all the parent keys and the parent dictionary at each child node though...hmm, and point to the same dictionary of the parent node to grab/search a sibling
        // TODO: --1-- look into an adjacency matrix
        // TODO: --4--  this will be when a tree traversal occurs, it will look to see if other nodes have things it can use for another sentence and add it in. If it doesn't make sense or is incorrect, we can provide input to let it know. DFS or BFS search

        public Trie(string[] sentence)
        {
            //this.NodePhrase = new Phrase(sentence);
            // TODO: --1-- instead of the sentence array, pass in the phrase, then we can retain the phrases at each tree root
            UniversalFunctions.LogToFile("Tree Constructor called...");
            SetTreeRoot(sentence);

            this.Current = this.Root;
            for (int i = 1; i < sentence.Length; i++)
            {
                TrieNode newNode = new TrieNode(sentence[i], i);
                this.Current.Children.Add(newNode.Word, newNode);
                this.Current.Children.TryGetValue(sentence[i], out this.Next);

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

        private void SetTreeRoot(string[] sentence)
        {
            UniversalFunctions.LogToFile("SetTreeRoot called...");
            this.Root = new TrieNode(sentence[0], 0);
        } // function SetTreeRoot

        // TODO: --3-- may need at Delete everything at this node in case someone types in gibberish So delete at the node past the pipe "|". Ex: I am the | want apples whereof who cats
        // TODO: --3-- alter node function? might want to keep track of height/size/depth then so we can say "grab this phrase, and modify this word at height/depth 'x'". Would need a delete and rebuild node then.
        public void Find(string findThisWord)
        {
            DFS_Find_Word(findThisWord, this.Root);
        } // function Find

        private void DFS_Find_Word(string findThisWord, TrieNode whichNode)
        {
            UniversalFunctions.LogToFile("DFS_Find_Word called...");
            Dictionary<string, TrieNode>.KeyCollection nodeKeys = whichNode.Children.Keys;

            foreach (string key in nodeKeys)
            {
                whichNode.Children.TryGetValue(key, out this.Next);

                if (this.Next != null)
                {
                    if (whichNode.Children[key].Word.Equals(findThisWord) || whichNode.Children[key].Word.Contains(findThisWord))
                    {
                        this.listOfNodes.Add(whichNode.Children[key]);
                    } // if; node equals or contains the word
                    DFS_Find_Word(findThisWord, this.Next);
                    // TODO: --3-- add a delegate function in here? that way if we want to, we can pass in different tasks like "Print everything" or "Find all of these words" or "find the first instance of this word" or "get the height of this word"
                } // if; next is not null
            } // foreach; key
            this.Next = null;
        } // function DFS_Find_Word

        public void Append(string[] sentence)
        {
            DFS_Append(sentence, this.Root);
        } // function Append

        private void DFS_Append(string[] sentence, TrieNode whichNode, int iterator = 1)
        {
            // TODO: --1-- when it checks the exact same sentence, it will generate an index out of bounds exception because once it gets to the last node, it will still be recursive so it will be at the max index + 1 when it does the final recursive call and will hit the OOB exception there. if we can resolve this, we can ditch the try-catch block. ??? Could do an array comparison but they'd have to always be consistent.
            try
            {
                this.Current = whichNode;
                this.Current.Children.TryGetValue(sentence[iterator], out this.Next);

                if (this.Next != null)
                {
                    DFS_Append(sentence, this.Next, iterator + 1);
                } // if; recursively dive through tree
                else
                {
                    for (int i = iterator; i < sentence.Length; i++)
                    {
                        TrieNode newNode = new TrieNode(sentence[i], i);
                        this.Current.Children.Add(newNode.Word, newNode);
                        this.Next = this.Current.Children[newNode.Word];

                        while (this.Next != null)
                        {
                            this.Current = this.Next;
                            this.Current.Children.TryGetValue(sentence[i], out this.Next);
                            //this.Next = this.Current.Next;// TODO: --1-- need to fix this. we don't want a next node in the TreeNode
                        } // while
                    } // for; word in a sentence
                } // else; append tree
                this.Next = null;
                this.Current = null;
            } // try
            catch(Exception e)
            {
                // TODO: --4-- for my logging program, add a function that converts enumerables into a string so i can just one line add all parameters to the log file
                UniversalFunctions.LogToFile("Error", e);
            } // catch
        } // function DFS_Traversal; starts after root is chosen elsewhere
    } // Class Tree
} // namespace
