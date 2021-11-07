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
            foreach (string word in sentence)
            {
                TreeNode newNode = new TreeNode(word);
                if (this.Root != null)
                {
                    this.Current = this.Root;
                    this.Next = this.Current.Next;
                    while (this.Next != null)
                    {
                        this.Current = this.Next;
                        this.Next = this.Current.Next;
                    }
                    this.Current.Children.Add(newNode.Word, newNode);
                    this.Current.Next = newNode;
                } // if; root is not empty
                else
                {
                    this.Root = newNode;
                } // else; root is empty
            } // foreach; word in a sentence
            this.Next = null;
        } // Constructor


        public void DFS_Append(string[] sentence, TreeNode whichNode, int iterator = 1)
        {
            // TODO: --1-- what happens when we add the exact same phrase twice? need to check for that
            try
            {
                this.Current = whichNode;
                this.Next = this.Current.Children[sentence[iterator]];
                DFS_Append(sentence, this.Next, iterator + 1);
            }
            catch
            {
                //TreeNode newNode = new TreeNode(sentence[iterator]);
                //this.Current.Children.Add(sentence[iterator], newNode);
                this.Current = whichNode;

                for (int i = iterator; i < sentence.Length; i++)
                {
                    TreeNode newNode = new TreeNode(sentence[i]);
                    this.Current.Children.Add(newNode.Word, newNode);
                    this.Next = this.Current.Children[newNode.Word];

                    while (this.Next != null)
                    {
                        this.Current = this.Next;
                        this.Next = this.Current.Next;
                    } // while
                } // for; word in a sentence
            }
        } // function DFS_Traversal; starts after root is chosen elsewhere
/*
        public void AppendTree(TreeNode whichNode)
        {
            TreeNode newNode = new TreeNode(sentence[iterator]);
            this.Current.Children.Add(sentence[iterator], newNode);
        }
*/
    } // Class Tree
} // namespace
