using System;
using System.Collections.Generic;
using System.Text;

namespace EnglishLearner
{
    public class WordTypeCollections
    {
        /*
 * Sentence Pattern:
 * A: Definite article
 * C: Conjunction
 * D: Adverb
 * J: Adjective
 * N: Noun
 * P: Preposition
 * V: Verb
 */
        public Dictionary<char, List<string>> DictOfLists { get; private set; } // I know _ first means private but i'm just getting annoyed it shows up last in the list when i dont want it to
        public List<string> DefiniteArticles = new List<string>();
        public List<string> Conjunctions = new List<string>();
        public List<string> Adverbs = new List<string>();
        public List<string> Adjectives = new List<string>();
        public List<string> Nouns = new List<string>();
        public List<string> Prepositions = new List<string>();
        public List<string> Verbs = new List<string>();
        public List<string> Unknown = new List<string>();
        private char[] arrayOfTypes = new char[8] { 'A', 'C', 'D', 'J', 'N', 'P', 'V', '?'};
        public int RandomType { get; private set; }

        public WordTypeCollections() 
        {
            this.DictOfLists = new Dictionary<char, List<string>>()
            {
                { 'A', DefiniteArticles },
                { 'C', Conjunctions },
                { 'D', Adverbs },
                { 'J', Adjectives },
                { 'N', Nouns },
                { 'P', Prepositions },
                { 'V', Verbs },
                { '?', Unknown },
            };

            Random rng = new Random();
            this.RandomType = rng.Next(0, this.arrayOfTypes.Length - 1);
        }

        private Dictionary<char, int> getRandomNumbers()
        { // returns random indexes that we can pull from in the list
            Dictionary<char, int> rngByKey = new Dictionary<char, int>();
            foreach (char type in this.arrayOfTypes)
            {
                if (DictOfLists[type].Count > 0)
                { // If the list contains words
                    Random rng = new Random();
                    rngByKey.Add(type, rng.Next(0, DictOfLists[type].Count - 1));
                }
                else
                { // if not return -1 for the type signifying nothing to randomize off of
                    rngByKey.Add(type, -1);
                }
            }
            return rngByKey;
        }

        public string GetRandomlyGeneratedSentence(char[] sentencePattern)
        {
            StringBuilder randSentence = new StringBuilder();

            for (int i = 0; i < sentencePattern.Length; i++)
            {
                var rng = getRandomNumbers(); // Every time you do this you should get another random sentence
                char wordType = sentencePattern[i];
                if (wordType.Equals("?"))
                { // fetches a random word type
                    Random randomType = new Random();
                    wordType = arrayOfTypes[randomType.Next(0, this.arrayOfTypes.Length - 1)];
                }

                // Add word
                randSentence.Append(DictOfLists[wordType][rng[wordType]]);

                if ((i + 1) < sentencePattern.Length)
                { // Add space if not last word
                    randSentence.Append(" ");
                }
            }


            return randSentence.ToString();
        }
    }
}
