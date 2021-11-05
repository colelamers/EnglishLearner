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
        public string Node_Word { get; set; }
        public TreeNode Next_Node { get; set; } // For Traversal
        public Dictionary<string, TreeNode> Node_Children { get; set; } // string = key, TreeNode = new KvP list of potential nodes

        public TreeNode(string word)
        {
            this.Node_Word = word;
            this.Next_Node = null;
            this.Node_Children = new Dictionary<string, TreeNode>();
        }
    }
}
