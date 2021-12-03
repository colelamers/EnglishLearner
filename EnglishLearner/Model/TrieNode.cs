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
        public Dictionary<string, TrieNode> Children { get; set; } // Basically Next
        public string Word { get; set; }
        public string WordType { get; set; }
        public string TypesOfPunctuation { get; set; }
        public string SearchedPunctuation { get; set; }
        public int NodeDepth { get; set; }
        public bool RecentlyTouched { get; set; }
        public bool CanBeLastWord { get; set; }
        public List<Phrase> Legal_KnownResponses { get; set; } // node so we can get sentence patterns
        public List<Phrase> Illegal_KnownResponses { get; set; }


        public TrieNode(string word, int currentDepth, string wordTypeAsLetter)
        {
            this.Word = word;
            this.Children = new Dictionary<string, TrieNode>();
            this.NodeDepth = currentDepth;
            this.WordType = wordTypeAsLetter.ToUpper();
            this.RecentlyTouched = false;
            this.CanBeLastWord = false;
            this.TypesOfPunctuation = ""; // blank by default
            this.SearchedPunctuation = "";
        } // set Child Node
    }
}
