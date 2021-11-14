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
        public string Word { get; set; }
        public Dictionary<string, TrieNode> Children { get; set; } // string = key, TreeNode = new KvP list of potential nodes
        int row { get; set; }
        public bool IsItLegal { get; set; } // legal by default
 

        public TrieNode(string word, int iRow)
        {
            this.Word = word;
            this.Children = new Dictionary<string, TrieNode>();
            this.IsItLegal = true;
            this.row = iRow;
        } // set Child Node
    }
}
