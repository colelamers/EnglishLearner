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
    [Serializable]
    class TrieNode
    {
        // TODO: --3-- should probably be in model and not controller

        public string Word { get; set; }
        // TODO: --3-- should we add subject, verb, objects
        public Dictionary<string, TrieNode> Children { get; set; } // string = key, TreeNode = new KvP list of potential nodes
        // public List<string> Illegal_Terms { get; set; } // TODO: --4--  this will be when a tree traversal occurs, it will look to see if other nodes have things it can use for another sentence and add it in. If it doesn't make sense or is incorrect, we can provide input to let it know. DFS or BFS search
        public TrieNode(string word)
        {
            this.Word = word;
            this.Children = new Dictionary<string, TrieNode>();
        }
    }
}
