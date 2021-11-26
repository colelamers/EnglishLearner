using System;
using System.Collections.Generic;

namespace EnglishLearner
{
    /*
     * Created by Cole Lamers & Hunter Van de Water
     * Date: 2021-11-04
     * 
     * == Purpose ==
     * The purpsoe of this is to contain basic information of a sentence to develop basic caveman speaking.
     * 
     * 
     */

    [Serializable]
    public class Phrase
    {
        public string Phrase_Sentence { get; set; }
        public string[] Phrase_Split_Sentence { get; set; }
        public string Phrase_First_Word { get; set; }
        public char Phrase_Punctuation { get; set; }
        public string[] SentencePattern { get; set; } // TODO: --3-- consider updating this to a char[] instead because of performance reasons
        public string HowIsItUsed { get; set; } // TODO: --1-- make some sentences that i can fully flesh out by default in memory

        /*
         * Sentence Pattern:
         * A: Definite article
         * C: Conjugation
         * D: Adverb
         * J: Adjective
         * N: Noun
         * P: Preposition
         * V: Verb
         */

        // TODO: --1-- if a correction happens, perform the Trie find and update all others, maybe only if they are at that location or if the preceeding and ensuing patter matches?

        public Phrase(string sentence, Dictionary<string, string[]> sqlAsDict)
        {
            UniversalFunctions.LogToFile($"Logging new Phrase sentence:\n\t{sentence}\n");
            this.Phrase_Sentence = sentence;
            (this.Phrase_Split_Sentence, this.Phrase_Punctuation) = SentenceFunctions.GetSplitSentenceAndPunctuation(this.Phrase_Sentence);
            this.Phrase_First_Word = this.Phrase_Split_Sentence[0].ToProper();
            this.SentencePattern = SentenceFunctions.GetSeteneceWordTypePattern(this.Phrase_Split_Sentence, sqlAsDict);          
        }
    }
}
