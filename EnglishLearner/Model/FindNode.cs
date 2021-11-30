using System;
using System.Collections.Generic;
using System.Text;

namespace EnglishLearner
{
    class FindNode
    {
        public Phrase Phrase { get; }
        public int Index { get; }
        public TrieNode NodePayload { get; set; }

        public FindNode(Phrase t_Phrase, TrieNode t_WhatToUpdate)
        {
            this.Phrase = t_Phrase;
            this.Index = t_WhatToUpdate.NodeDepth;
            this.NodePayload = t_WhatToUpdate;
        }
    }
}
