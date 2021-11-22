using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EnglishLearner
{
    /*
     * Created by Cole Lamers & Hunter Van de Water
     * Date: 2021-11-04
     * 
     * == Purpose ==
     * Handling of sentence parsing, asserting, and checking
     * 
     * 
     */
    public static class SentenceFunctions
    {
        private static readonly char[] endPunctuation = new char[3] { '.', '?', '!' };

        private static readonly string[] wordTypes = new string[]
        {
            "n.",       //noun
            "pron.",    //pronoun
            "object.",  //another type for pronoun like "I"
            "v.",       //verb
            "adv.",     //adverb don't need because v. will grab this
            "imp.",     //Past tense word; -ed
            "p.p.",     //Past tense word; -ed
            "conj.",    //and, both, because, or, then, until, if
            "adj.",     //adjective
            "superl.",  //adjective
            "a.",       //adjective
            "prep.",    //preposition; on, onto, in, to, through, from, at
            "definite article." //articles such as the
        };

        private static readonly string[] pluralSuffixes = new string[]
        {
            "ous",
            "een",
            "ass",
            "ess",
            "iss",
            "oss",
            "uss",
            "sis",
            "ics",
            "sus",
            "ies",
            "ics"
        };

        public static bool Is_Sentence(string consoleInput)
        {
            bool tof = false;
            try
            {
                char[] inputCharArray = consoleInput.ToCharArray();
                if (endPunctuation.Contains(inputCharArray[consoleInput.Length - 1]))
                {
                    tof = true;
                } // if; last character is a period
                else
                {
                    Console.WriteLine("Sentence does not contain a period. Please add it to the end of the sentence."); // required because all future parsing is contingent upon grammar of reading in sentences
                    tof = false;
                } // else
            }
            catch (Exception e)
            {
                UniversalFunctions.LogToFile("Exception:", e);
            }
            return tof;
        } // function Is_Sentence

        public static string[] Split_Paragraph(string paragraph)
        {
            try
            {
                string[] sentences = paragraph.Split('.'); // Splits at period
                for (int i = 0; i < sentences.Length; i++)
                {
                    if (sentences[i].ToCharArray().Equals(' '))
                    {
                        sentences[i] = sentences[i].Remove(0, 1);
                    } // if; removes first char if it's a space
                } // for; loops through all sentences in the paragraph
                return sentences;
            } // try
            catch (Exception e)
            {
                UniversalFunctions.LogToFile("Exception:", e);
                throw e;
            } // catch
        } // function Split_Paragraph

        // TODO: --3-- add a function that replaces & with "and" or + with "plus" or - with "minus" and things like that
        public static (string[], char) GetSplitSentenceAndPunctuation(string sentence)
        {
            UniversalFunctions.LogToFile("GetSplitSentenceAndPunctuation called...");
            /*
             * string[] splitSentence = Regex.Replace(sentence, "[^A-z ]", "").Split(" "); // retain only A-z and spaces
             * passing in char[] is ~2.5x faster at the split join than regex. however consider the regex if things get too complicated or there are additional chars we need to eliminate
             */
            char[] removeChars = new char[] { '\'', '"', '/', ';', ':', '*', '^', '&', '@', ',', '[', ']', '|', '>', '<' };
            sentence = string.Join("", sentence.Split("..."));
            string[] splitSentence = string.Join("", sentence.Split(removeChars)).Split(" "); //https://stackoverflow.com/questions/7411438/remove-characters-from-c-sharp-string fastest way
            string[] sentenceAsArray = RemoveNullOrWhiteSpaceIndexes(splitSentence);

            return (sentenceAsArray, GetPunctuation(sentenceAsArray));
        } // function GetSplitSentence

        private static string[] RemoveNullOrWhiteSpaceIndexes(string[] splitSentence)
        {
            int j = 0;
            for (int i = 0; i < splitSentence.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(splitSentence[i]))
                {
                    j++;
                } // if; string is not null
            } // // for; each array index

            string[] sentenceAsArray = new string[j];
            int k = 0;
            for (int i = 0; i < splitSentence.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(splitSentence[i]))
                {
                    sentenceAsArray[k] = splitSentence[i];
                    k++; // new array index
                } // if; index is not null
            } // for; each word from the passed in string[]

            return sentenceAsArray;
        } // function RemoveNullOrWhiteSpaceIndexes

        private static char GetPunctuation(string[] sentenceAsArray)
        {
            UniversalFunctions.LogToFile("GetPunctuation called...");

            string finalWord = sentenceAsArray[sentenceAsArray.Length - 1];
            int finalWordPunctuationIndex = finalWord.Length - 1;
            char punctuation = '.';

            if (finalWord.IndexOfAny(endPunctuation) >= 0)
            {
                punctuation = finalWord[finalWordPunctuationIndex];
                finalWord = finalWord.Remove(finalWordPunctuationIndex);
                sentenceAsArray[sentenceAsArray.Length - 1] = finalWord;
            } // if; checks for end sentence punctuation and updates it if it's different and removes it from the split setence

            return punctuation;
        }

        private static string RemoveTenseOrPlural(string word)
        {
            // Since English breaks so many rules, this is not 100% accurate for words like "When" or "Seen" or "Mess"
            string lastLetter = word.Substring(word.Length - 1).ToLower();
            if (lastLetter.Equals("s"))
            {
                if (word.Length >= 4)
                {
                    if (!pluralSuffixes.Contains(word.Substring(word.Length - 3)))
                    {
                        return word.Remove(word.Length - 1); // removes final "s"
                    } // if; the last 3 chars do not match anything stored in common english suffixes containing the letter "s"
                } // if; minimum letter count in word for rule to generally apply
            } // if; plural check

            string lastTwoLetters = word.Substring(word.Length - 2).ToLower();
            if (lastTwoLetters.Equals("en") || lastTwoLetters.Equals("ed"))
            {
                return word.Remove(word.Length - 2);
            } // if; plural check
            return word;
        } // function RemoveTenseOrPlural

        public static string[] GetSeteneceWordTypePattern(string[] splitSentence, Dictionary<string, string[]> sqlAsDict)
        {
            UniversalFunctions.LogToFile("GetSeteneceWordTypePattern called...");
            string[] wordPattern = new string[splitSentence.Length];
            try
            {
                Dictionary<string, Word> wordAndTheirTypes = new Dictionary<string, Word>();
                for (int i = 0; i < splitSentence.Length; i++)
                {

                    string[] dictTypes = null; // initialize
                    sqlAsDict.TryGetValue(splitSentence[i].ToProper(), out dictTypes);

                    if (dictTypes == null)
                    {
                        string wordCheck = RemoveTenseOrPlural(splitSentence[i]);
                        if (!splitSentence[i].Equals(wordCheck))
                        {
                            sqlAsDict.TryGetValue(wordCheck.ToProper(), out dictTypes);
                        } // if; wordCheck doesn't equal the same word again, retry the search
                    } // if; dictTypes were not found in the dictionary

                    Word metadata_word = new Word();

                    if (dictTypes == null)
                    {
                        dictTypes = new string[] { "?" };
                    } // if; no dictionary data found for word

                    if (dictTypes.Length == 0)
                    {
                        dictTypes = new string[] { "?" };
                    } // if; key is not in dictionary

                    metadata_word = new Word(dictTypes, splitSentence[i]);
                    /*
                                        if ((i + 1) < splitSentence.Length)
                                        {
                                            metadata_word.NextWord = splitSentence[i + 1];
                                        } //  if; there is an index ensuing the current

                                        if ((i - 1) > 0)
                                        {
                                            metadata_word.PreceedingWord = splitSentence[i - 1];
                                        } // if; there is an index preceeding the current
                    */
                    wordAndTheirTypes.Add(splitSentence[i], metadata_word); // ad the list with the word

                } // for; word in the sentence

                foreach (KeyValuePair<string, Word> kvp in wordAndTheirTypes)
                {
                    for (int i = 0; i < kvp.Value.WordTypes.Length; i++)
                    {
                        foreach (string wType in wordTypes)
                        {
                            // TODO: --3-- might need to check in this part of the loop in case it starts pulling non standard values
                            if (kvp.Value.WordTypes[i].Length > wType.Length)
                            {
                                string shortenedType = kvp.Value.WordTypes[i].Substring(0, wType.Length);
                                if (shortenedType.ToCharArray().SequenceEqual(wType.ToCharArray()))
                                {
                                    wordAndTheirTypes[kvp.Key].WordTypes[i] = shortenedType;
                                } // if; WordType was split at the length of the word type being checked to verify if they match, then it will update the index with the standardized syntax
                            } // if; WordTypes is longer than the specific word type being checked
                        } // foreach; wordType found within the Word object
                    } // foreach; word and it's corresponding object
                } // foreach; kvp

                string subject = "";
                string verb = "";
                string obj = "";

                LinkedList<Word> llWord = new LinkedList<Word>();
                LinkedListNode<Word> lln = null; // root

                foreach (KeyValuePair<string, Word> kvp in wordAndTheirTypes)
                {
                    lln = new LinkedListNode<Word>(kvp.Value);
                    llWord.AddLast(lln);
                }

                while (lln.Previous != null) { lln = lln.Previous; } // while; revert back to root

                // TODO: --1-- find a noun, if you have multiple, determine based on previous/next or something, make subject, then do the same for the verb, object
                // TODO: --1-- build the word patter at this point.
            } // try
            catch (Exception e)
            {
                UniversalFunctions.LogToFile("Exception", e);
            } // catch

            return wordPattern;
        } // function; GetSeteneceWordTypePattern



    } // class SentenceFunctions
} // namespsace
