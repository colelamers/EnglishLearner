using System;
using System.Collections.Generic;
using System.Text;

namespace EnglishLearner
{
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
