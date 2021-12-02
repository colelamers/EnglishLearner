using System;
using System.Collections.Generic;
using System.Text;

namespace EnglishLearner
{
    // TODO: --3-- kinda think this a is a tad overkill atm
    class FindNode
    {
        public string[] SentenceArray { get; }
        public TrieNode NodePayload { get; set; }

        public FindNode(string[] SentenceArray, TrieNode t_WhatToUpdate)
        {
            this.SentenceArray = SentenceArray;
            this.NodePayload = t_WhatToUpdate;
        }
    }
}
