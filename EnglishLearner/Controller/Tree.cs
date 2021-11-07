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
    class Tree
    {
        public TreeNode Root = null, Current = null, Next = null;

        //public Dictionary<Phrase, Phrase> Sentence_Info { get; set; } // TODO: --3-- most likely will need this at each tree root for info about corresponding sentences
        // TODO: --1-- should probably be in model and not controller. make a brain class again but contain the tree and certain functions?
        // public List<string> Illegal_Terms { get; set; } // TODO: --4--  this will be when a tree traversal occurs, it will look to see if other nodes have things it can use for another sentence and add it in. If it doesn't make sense or is incorrect, we can provide input to let it know. DFS or BFS search.
        // TODO: --4-- need to retain illegal values in nodes specifically because a single word does not negate the legitimacy of an entire sentence.
        // TODO: --4-- add lists of known subject, verb, objects that it can interpret from if there is an illegal value somewhere.
        // NOTE: Key based traversal is DFS, Deciding on which available child nodes 

        public Tree(string[] sentence)
        {
            SetTreeRoot(sentence);
            this.Current = this.Root;

            for (int i = 1; i < sentence.Length; i++)
            {
                TreeNode newNode = new TreeNode(sentence[i]);
                this.Current.Children.Add(newNode.Word, newNode);
                this.Current.Children.TryGetValue(sentence[i], out this.Next);
                while (this.Next != null)
                {
                    this.Current = this.Next;
                    this.Current.Children.TryGetValue(sentence[i], out this.Next);
                }
            } // foreach; word in a sentence
            this.Next = null;
            this.Current = null;
        } // Constructor

        private void SetTreeRoot(string[] sentence)
        {
            this.Root = new TreeNode(sentence[0]);
        } // function SetTreeRoot

/*
 * TODO: --1-- cole needs to fix. i was originally using the Next member of the TreeNode class and that should never have happened
        public void DFS_Find_Word(string findThisWord)
        {
            // TODO: --1-- need to implement
            List<object> occurances = new List<object>();

            this.Current = this.Root;
            this.Next = this.Current.Next;
            while (this.Next != null)
            {
                this.Current = this.Next;
                this.Next = this.Current.Next;// TODO: --1-- need to fix this. we don't want a next node in the TreeNode

                if (this.Current.Word.Equals(findThisWord))
                {
                    // occurances.Add(this.Current.)
                }
            }

            this.Next = null;
            this.Current = null;
        }
*/

        public void DFS_Append(string[] sentence, TreeNode whichNode, int iterator = 1)
        {
            // TODO: --1-- when it checks the exact same sentence, it will generate an index out of bounds exception because once it gets to the last node, it will still be recursive so it will be at the max index + 1 when it does the final recursive call and will hit the OOB exception there
            this.Current = whichNode;
            this.Current.Children.TryGetValue(sentence[iterator], out this.Next);
            try
            {
                if (this.Next != null)
                {
                    DFS_Append(sentence, this.Next, iterator + 1);
                } // if; recursively dive through tree
                else
                {
                    for (int i = iterator; i < sentence.Length; i++)
                    {
                        TreeNode newNode = new TreeNode(sentence[i]);
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
            }
            catch(Exception e)
            {
                // TODO: --4-- for my logging program, add a function that converts enumerables into a string so i can just one line add all parameters to the log file
                UniversalFunctions.LogToFile("Error", e);
            }
        } // function DFS_Traversal; starts after root is chosen elsewhere
/*
 * TODO: --1-- would like to split up the Traversal and Appending so that we can grab specific nodes and alter them. Currently we can only append at specific keys/child-nodes.
        public void AppendTree(TreeNode whichNode)
        {
            TreeNode newNode = new TreeNode(sentence[iterator]);
            this.Current.Children.Add(sentence[iterator], newNode);
        }
*/
    } // Class Tree
} // namespace
