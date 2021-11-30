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

        public FindNode(Phrase t_Phrase, TrieNode t_WhatToUpdate)
        {
            this.Phrase = t_Phrase;
            this.Index = t_WhatToUpdate.NodeDepth;
            this.NodePayload = t_WhatToUpdate;
        }


        public FindNode(Phrase t_Phrase, int t_Index, object t_WhatToUpdate = null)
        { // TODO: --3-- probably can ditch this. just sent the tirenode and pass it through
            this.Phrase = t_Phrase;
            this.Index = t_Index;
            this.NodeMemberPayload = t_WhatToUpdate;
        }
    }
}
