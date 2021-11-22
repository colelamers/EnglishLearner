using System;
using System.Collections.Generic;
using System.Text;

namespace EnglishLearner
{
    class Word
    {
        public string[] WordTypes { get; set; }
        public string CurrentWord { get; set; }

        public Word() { } // default constructor
        public Word(string[] types, string word)
        {
            this.WordTypes = types;
            this.CurrentWord = word;
        }

/*
        public string NextWord { get; set; }
        public string PreceedingWord { get; set; }
*/
    }
}
