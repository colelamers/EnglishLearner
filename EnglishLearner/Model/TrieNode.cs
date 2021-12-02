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
        public Dictionary<string, TrieNode> Children { get; set; } // string = key, TreeNode = new KvP list of potential nodes
        public string Word { get; set; }
        public string WordType { get; set; } // TODO: --1-- need to add these at every time the node is created
        public int NodeDepth { get; set; }
        public bool RecentlyTouched { get; set; }
        public bool CanBeLastWord { get; set; }
        public string TypesOfPunctuation { get; set; }
        public string SearchedPunctuation { get; set; }
        public List<Phrase> KnownResponses { get; set; }


        public TrieNode(string word, int currentDepth, string wordTypeAsLetter)
        {
            this.Word = word;
            this.Children = new Dictionary<string, TrieNode>();
            this.NodeDepth = currentDepth;
            this.WordType = wordTypeAsLetter;
            this.RecentlyTouched = false;
            this.CanBeLastWord = false;
            this.TypesOfPunctuation = ""; // blank by default
            this.SearchedPunctuation = "";
            this.KnownResponses = null; // null by default unless initialized as an ending node
        } // set Child Node
    }
}
