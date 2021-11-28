using System;
using System.Collections.Generic;
using System.Text;

namespace EnglishLearner
{
    class SentenceWord
    {
        public string WordType { get; set; }
        public string CurrentWord { get; set; }

        public SentenceWord(string type, string word)
        {
            this.WordType = type;
            this.CurrentWord = word;
        }
    }
}
