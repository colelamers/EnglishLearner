using System;
using System.Collections.Generic;
using System.Text;

namespace EnglishLearner
{
    class FindNode
    {
        public Phrase Phrase { get; }
        public int Index { get; }
        public object Payload { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t_Sentence">Phrase sentence array</param>
        /// <param name="t_Index">Index of the word in the sentence array</param>
        /// <param name="UpdatePayload">Whatever at the node you wish to update</param>
        public FindNode(Phrase t_Phrase, int t_Index, object t_WhatToUpdate)
        {
            this.Phrase = t_Phrase;
            this.Index = t_Index;
            this.Payload = t_WhatToUpdate;
        }
    }
}
