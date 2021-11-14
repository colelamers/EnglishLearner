using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using System.Threading.Tasks;

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

    public class Phrase
    {
        public string Phrase_Sentence { get; set; }
        public string[] Phrase_Split_Sentence { get; set; }
        public string Phrase_First_Word { get; set; }
        public char Phrase_Punctuation { get; set; }
        public List<char> WordTypePattern { get; set; }

        public Phrase(string sentence, string configPath)
        {

            this.Phrase_Sentence = sentence;
            (this.Phrase_Split_Sentence, this.Phrase_Punctuation) = SentenceFunctions.GetSplitSentenceAndPunctuation(this.Phrase_Sentence);
            this.Phrase_First_Word = SentenceFunctions.GetFirstWordProper(this.Phrase_Split_Sentence);
            //this.Phrase_Punctuation = SentenceFunctions.GetPunctuation(this.Phrase_Split_Sentence[this.Phrase_Split_Sentence.Length - 1].ToCharArray());
            this.WordTypePattern = SentenceFunctions.GetSeteneceWordTypePattern(this.Phrase_Split_Sentence, configPath);
        } // constructor; Phrase

    }
}
