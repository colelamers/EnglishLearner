using System;
using System.Collections.Generic;
using System.Text;

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
        public string Phrase_Subject { get; set; }
        public string Phrase_Verb { get; set; }
        public string Phrase_Object { get; set; }
        //public string Phrase_Tense_Verb { get; set; } //TODO: --4-- for future implementation
        //public bool Phrase_Subject_Plural { get; set; } // TODO: --4-- for future implementation
        public char Phrase_Punctuation { get; set; }
        //public List<object> RulesLearned = new List<object>(); // TODO: --1-- this will need to be revised to contain Func or delegates. this will things the brain learns and knows to apply processing logic to for additional setences. Talk to Cole for more info.

        public Phrase(string sentence)
        {
            this.Phrase_Sentence = sentence;
            this.Phrase_Split_Sentence = sentence.Split(" "); // TODO: --1-- not sure this works yet
            this.Phrase_Subject = interpretSubject();
            this.Phrase_Verb = interpretVerb();
            this.Phrase_Object = interpretObject();
            this.Phrase_Punctuation = interpretPunctuation();
        } // constructor; Phrase

        //Run queries to find out if an object in the sentece is subject, verb, object, and any punctuation.
        //Test out the insertInto function within the SQLite Actions tab to handle words not found in the database



        private char interpretPunctuation()
        {
            char the_punctuation = ' ';

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






        // TODO: --4-- Touch at a later time
        //private bool interpretSubjectPlural()
        //{
        //    // TODO: --4-- utilize this function as a basis for other functions when determining tense and other things
        //    // TODO: --4-- this is not correct because words like "US, zealous, cross, pelvis, taxes, grass." Can add a check for 's, ' at the end, double 's', -ous, -ass/-ess/-iss/-oss/-uss
        //    char[] letterArray = this.Phrase_Subject.ToLower().ToCharArray();
        //    // TODO: --4-- we could also utilize regex for this instead
        //    // NOTE: char array is faster than looping through the entire array since we know that plurality in English is defined by the last few characters typically when context is not established
        //    if (letterArray[letterArray.Length - 1].Equals('s'))
        //    {
        //        return true;
        //    } // if;

        //    return false;
        //} // function; interpretSubjectPlural
    }
}
