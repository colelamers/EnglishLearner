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
     * Holds the nodes for the trees
     * 
     */
    class TreeNode
    {
        // TODO: --3-- should probably be in model and not controller

        public string Word { get; set; }
        // TODO: --3-- should we add subject, verb, objects
        public TreeNode Next { get; set; } // For Traversal
        public Dictionary<string, TreeNode> Children { get; set; } // string = key, TreeNode = new KvP list of potential nodes

        public TreeNode(string word)
        {
            this.Word = word;
            this.Next = null;
            this.Children = new Dictionary<string, TreeNode>();
        }
    }
}
