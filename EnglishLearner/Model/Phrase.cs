using System;
using System.Collections.Generic;
using System.Text;

namespace EnglishLearner
{
    class Phrase
    {
        string Phrase_Sentence { get; set; }
        string Phrase_Subject { get; set; }
        string Phrase_Verb { get; set; }
        string Phrase_Object { get; set; }
        List<object> RulesLearned = new List<object>(); // TODO: --1-- this will need to be revised to contain Func or delegates. this will things the brain learns and knows to apply processing logic to for additional setences. Talk to Cole for more info.

        public Phrase(string sentence)
        {
            this.Phrase_Sentence = sentence;
            this.Phrase_Subject = interpretSubject();
            this.Phrase_Verb = interpretVerb();
            this.Phrase_Object = interpretObject();
        }

        private string interpretSubject()
        {
            string the_subject = "";

            // TODO: --1-- interpretSubject logic here
            return the_subject;
        }

        private string interpretVerb()
        {
            string the_verb = "";
            // TODO: --1-- interpretVerb logic here
            return the_verb;
        }

        private string interpretObject()
        {
            string the_object = "";
            // TODO: --1-- interpretObject logic here
            return the_object;
        }
    }
}
