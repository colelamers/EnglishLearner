using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EnglishLearner
{
    /*
     * Created by Cole Lamers 
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
        public string Phrase_Subject { get; set; }
        public string Phrase_Verb { get; set; }
        public string Phrase_Object { get; set; }
        //public string Phrase_Tense_Verb { get; set; } //TODO: --4-- for future implementation
        //public bool Phrase_Subject_Plural { get; set; } // TODO: --4-- for future implementation
        public char Phrase_Punctuation { get; set; }
        //public List<object> RulesLearned = new List<object>(); // TODO: --1-- this will need to be revised to contain Func or delegates. this will things the brain learns and knows to apply processing logic to for additional setences. Talk to Cole for more info.
        // TODO: --1-- will need to make functions asyncronous so that they run independent of new Phrases being created.
        public Dictionary<string, bool> Phrase_Legalties { get; set; }

        public Phrase(string sentence)
        {
            // TODO: --3-- could make all interpret functions asyncronously running for each phrase initialized. could speed things up down the road.

            this.Phrase_Sentence = sentence;
            this.Phrase_Split_Sentence = interpretSplitSentence();
            this.Phrase_First_Word = interpretFirstWord(); // relies on Phrase_Split_Sentence to occur before this
            this.Phrase_Subject = interpretSubject();
            this.Phrase_Verb = interpretVerb();
            this.Phrase_Object = interpretObject();
            this.Phrase_Punctuation = interpretPunctuation();
            this.Phrase_Legalties = interpretLegalities();
        } // constructor; Phrase

        private string[] interpretSplitSentence()
        {
            string[] array = this.Phrase_Sentence.Split(" ");
            int num;
            array = array.Where(x => !string.IsNullOrWhiteSpace(x)).Where(x => !int.TryParse(x, out num)).ToArray();

            return array;
        }

        private string interpretFirstWord()
        {
            // TODO: --1-- running into an issue when parsing the split sentences to find the first letter a space or a number. also if it's only one word. always causes the program to crash around here.
            char[] capitalizedFirstLetter = this.Phrase_Split_Sentence[0].ToCharArray();
            capitalizedFirstLetter[0] = char.ToUpper(capitalizedFirstLetter[0]);
            return new string(capitalizedFirstLetter);
        } // function interpretFirstWord

        private Dictionary<string, bool> interpretLegalities()
        {
            Dictionary<string, bool> word_legalVal = new Dictionary<string, bool>();


            // TODO: --1-- interpretLegality logic here; is the word legal (meaning proper English)
            /*
             * Legality of a term is determined by the context of the sentence. Typically can only be determined by a native;
             * loop through words and determine if they're legal
             */
            // this.Phrase_Legalities.Add("Them", true); this word is legal
            // this.Phrase_Legalities.Add("crazys", false); this word is illegal
            return word_legalVal;
        }

        //Run queries to find out if an object in the sentece is subject, verb, object, and any punctuation.
        //Test out the insertInto function within the SQLite Actions tab to handle words not found in the database

        private char interpretPunctuation()
        {
            char the_punctuation = ' ';
            // TODO: --1-- need to remove the punctuation from the sentence as well
            // TODO: --1-- interpretPunctuation logic here
            return the_punctuation;
        } // function; interpretPunctuation

        private string interpretSubject()
        {
            string the_subject = "";

            // TODO: --1-- interpretSubject logic here
            return the_subject;
        } // function; interpretSubject

        private string interpretVerb()
        {
            string the_verb = "";
            // TODO: --1-- interpretVerb logic here
            return the_verb;
        } // function; interpretVerb

        private string interpretObject()
        {
            string the_object = "";
            // TODO: --1-- interpretObject logic here
            return the_object;
        } // function; interpretObject

/*
        // TODO: --4-- Touch at a later time
        private bool interpretSubjectPlural()
        {
            // TODO: --4-- utilize this function as a basis for other functions when determining tense and other things
            // TODO: --4-- this is not correct because words like "US, zealous, cross, pelvis, taxes, grass." Can add a check for 's, ' at the end, double 's', -ous, -ass/-ess/-iss/-oss/-uss
            char[] letterArray = this.Phrase_Subject.ToLower().ToCharArray();
            // TODO: --4-- we could also utilize regex for this instead
            // NOTE: char array is faster than looping through the entire array since we know that plurality in English is defined by the last few characters typically when context is not established
            if (letterArray[letterArray.Length - 1].Equals('s'))
            {
                return true;
            } // if;

            return false;
        } // function; interpretSubjectPlural
*/

    }
}
