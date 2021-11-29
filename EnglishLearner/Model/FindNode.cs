using System;
using System.Collections.Generic;
using System.Text;

namespace EnglishLearner
{
    class FindNode
    {
        public Phrase Phrase { get; }
        public int Index { get; }
        public object NodeMemberPayload { get; set; }
        public TrieNode NodePayload { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t_Sentence">Phrase sentence array</param>
        /// <param name="t_Index">Index of the word in the sentence array</param>
        /// <param name="UpdatePayload">Whatever at the node you wish to update</param>
        public FindNode(Phrase t_Phrase, int t_Index, object t_WhatToUpdate = null)
        {
            this.Phrase = t_Phrase;
            this.Index = t_Index;
            this.NodeMemberPayload = t_WhatToUpdate;
        }

        public FindNode(Phrase t_Phrase, TrieNode t_WhatToUpdate)
        {
            this.Phrase = t_Phrase;
            this.Index = t_WhatToUpdate.NodeDepth;
            this.NodePayload = t_WhatToUpdate;
        }
    }
}
