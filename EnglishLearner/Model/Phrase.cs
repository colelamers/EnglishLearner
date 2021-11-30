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
        public string Sentence { get; }
        public string Sentence_NoPunctuation { get; }
        public string[] Split_Sentence { get; set; }
        public string First_Word { get; set; } // TODO: --3-- think about removing cause it's not really necessary, just enhances readability
        public string Punctuation { get; set; }
        public string[] SentencePattern { get; set; } // TODO: --3-- consider removing this or something after node creation?

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
            (this.Split_Sentence, this.Punctuation) = SentenceFunctions.GetSplitSentenceAndPunctuation(sentence);
            this.Sentence_NoPunctuation = string.Join("", sentence.Split(new char[] { '.', '!', '?', ',' }));

            this.Sentence = string.Join("", sentence.Split(new char[] { '.', '!', '?', ',' })) + this.Punctuation;
            this.First_Word = this.Split_Sentence[0].ToProper();
            this.SentencePattern = SentenceFunctions.GetSeteneceWordTypePattern(this.Split_Sentence, sqlAsDict);          
        }
    }
}
