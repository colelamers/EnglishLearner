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
     * 
     * 
     */
    class Tree
    {
        public TreeNode Root, Current, Next;
        // TODO: --3-- should probably be in model and not controller

        public Tree()
        {
            this.Root = null; // first word in a sentence
            this.Current = null;
            this.Next = null;
        }

        public void Build_Tree(string[] sentence)
        {
            foreach (string word in sentence)
            {
                TreeNode newNode = new TreeNode(word);
                if (this.Root == null)
                {
                    this.Root = newNode;
                } // if; root is empty
                else
                {
                    this.Current = this.Root;
                    this.Next = this.Current.Next_Node;
                    while (this.Next != null)
                    {
                        this.Current = this.Next;
                        this.Next = this.Current.Next_Node;
                    }
                    this.Current.Node_Children.Add(newNode.Node_Word, newNode);
                    this.Current.Next_Node = newNode;
                } // else; not root
            } // foreach; word in a sentence
            this.Root.Next_Node = null; // TODO: --1-- i'm not sure if i should be doing this, but it empties it out once it's been filled so that traversal can occur again for the dictionaries
        } // function Build_Tree

        public void AppendDictionaries(string[] sentence)
        {
            // This function assumes that the sentence passed in contains the same root word as the current tree
            // TODO: --1-- so here we need to do something similar to the build_tree, but it differs in that the starting node is different based on the keys. then the corresponding dictionaries get added to 

            // i starts at 1 because we don't care about the first word since it's the tree root
            for (int i = 1; i < sentence.Length; i++)
            {
                TreeNode newNode = new TreeNode(sentence[i]);

                if (this.Root.Node_Children.ContainsKey(sentence[i]))
                {
                    this.Root.Node_Children[sentence[i]].Node_Children.Add(sentence[i], newNode);
                }

                if (this.Root == null)
                {
                    this.Root = newNode;
                } // if; root is empty
                else
                {
                    this.Current = this.Root;
                    this.Next = this.Current.Next_Node;
                    while (this.Next != null)
                    {
                        this.Current = this.Next;
                        this.Next = this.Current.Next_Node;
                    }
                    this.Current.Node_Children.Add(newNode.Node_Word, newNode);
                    this.Current.Next_Node = newNode;
                } // else; not root
            }
        }

    } // Class Tree
} // namespace
