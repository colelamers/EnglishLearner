using System;
using System.Collections.Generic;
                                                 
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
        public string WordType { get; set; } // TODO: --1-- need to add these at every time the node is created
        public int NodeDepth { get; set; } // TODO: --1-- need this implemented
        public bool IsItLegal { get; set; } // legal by default
        // TODO: --1-- i think i'm supposed to have current and next here...hmm
 
        public TrieNode(string word, int currentDepth, string wordTypeAsLetter)
        {
            this.Word = word;
            this.Children = new Dictionary<string, TrieNode>();
            this.NodeDepth = currentDepth;
            this.WordType = wordTypeAsLetter;
            if (wordTypeAsLetter.Equals("?"))
            {
                this.IsItLegal = false;
            }
        } // set Child Node
    }
}
