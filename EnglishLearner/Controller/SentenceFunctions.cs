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
        private static readonly char[] endPunctuation = new char[] 
        {
            '.',
            '?',
            '!'
        };
        private static readonly string[] pronouns = new string[] 
        { 
            "i",
            "im",
            "me",
            "her",
            "him",
            "his",
            "himself",
            "herself",
            "itself",
            "themself",
            "themselves",
            "he", 
            "she", 
            "it", 
            "its",
            "they",
            "theyre",
            "them", 
            "we", 
            "us"
        };
        private static readonly string[] wordTypes = new string[]
        {
            "n.",               //noun
            "pron.",            //pronoun
            "object.",          //another type for pronoun like "I"
            "obj.",             //he, she, it

            "v.",               //verb
            "imp.",             //Past tense word; -ed
            "p.p.",             //Past tense word; -ed

            "adj.",             //adjective
            "superl.",          //adjective
            "a.",               //adjective

            "adv.",             //adverb don't need because v. will grab this
            "conj.",            //and, both, because, or, then, until, if
            "prep.",            //preposition; on, onto, in, to, through, from, at
            "definite article." //articles such as the
        };
        private static readonly string[] prepositions = new string[] 
        { 
            "about",
            "above",
            "across",
            "after",
            "against",
            "among",
            "around",
            "at",
            "before",
            "behind",
            "below",
            "beside ",
            "between",
            "by",
            "down",
            "during",
            "for",
            "from",
            "in",
            "inside",
            "into",
            "near",
            "of",
            "off",
            "on",
            "out",
            "over",
            "through",
            "to",
            "toward",
            "under",
            "up",
            "with"
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
            
            int j = 0;
            for (int i = 0; i < splitSentence.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(splitSentence[i])) { j++; } // if; string is not null increment j
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

            return (sentenceAsArray, GetPunctuation(sentenceAsArray));
        } // function GetSplitSentence

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

        public static string[] GetSeteneceWordTypePattern(string[] splitSentence, Dictionary<string, string[]> sqlAsDict)
        {
            UniversalFunctions.LogToFile("GetSeteneceWordTypePattern called...");
            string[] wordPattern = new string[splitSentence.Length];
            try
            {
                LinkedList<Word> llSentence = new LinkedList<Word>();
                LinkedListNode<Word> lln = null; // root

                for (int i = 0; i < splitSentence.Length; i++)
                {

                    string[] dictTypes = null; // initialize

                    if (!splitSentence[i].Equals(""))
                    {
                        sqlAsDict.TryGetValue(splitSentence[i].ToProper(), out dictTypes);
                    }

                    if (dictTypes == null && splitSentence[i].Length > 3)
                    {
                        string wordCheck = "";
                        string lastTwoLetters = splitSentence[i].Substring(splitSentence[i].Length - 2).ToLower();

                        if (splitSentence[i].Substring(splitSentence[i].Length - 1).ToLower().Equals("s"))
                        {
                            if (splitSentence[i].Length >= 4)
                            {
                                if (!pluralSuffixes.Contains(splitSentence[i].Substring(splitSentence[i].Length - 3)))
                                {
                                    wordCheck = splitSentence[i].Remove(splitSentence[i].Length - 1); // removes final "s"
                                } // if; the last 3 chars do not match anything stored in common english suffixes containing the letter "s"
                            } // if; minimum letter count in splitSentence[i] for rule to generally apply
                        } // if; plural check
                        else if (lastTwoLetters.Equals("en") || lastTwoLetters.Equals("ed"))
                        {
                            wordCheck = splitSentence[i].Remove(splitSentence[i].Length - 2);
                        } // if; specifically because our dictionary does not contain all past tense words


                        if (!splitSentence[i].Equals(wordCheck) && !wordCheck.Equals(""))
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
                    try
                    {
                        lln = new LinkedListNode<Word>(new Word(dictTypes, splitSentence[i]));
                        llSentence.AddLast(lln);
                    }
                    catch (Exception e)
                    {
                        UniversalFunctions.LogToFile("Exception", e);
                    }

                } // for; word in the sentence

                // TODO: --2-- might be able to combine this with the code following it
                foreach (Word hWord in llSentence)
                {
                    for (int i = 0; i < hWord.WordTypes.Length; i++)
                    {
                        foreach (string wType in wordTypes)
                        {
                            if (hWord.WordTypes[i].Length > wType.Length)
                            {
                                string shortenedType = hWord.WordTypes[i].Substring(0, wType.Length);
                                if (shortenedType.ToCharArray().SequenceEqual(wType.ToCharArray()))
                                {
                                    hWord.WordTypes[i] = shortenedType;
                                } // if; WordType was split at the length of the word type being checked to verify if they match, then it will update the index with the standardized syntax
                            } // if; WordTypes is longer than the specific word type being checked
                        } // foreach; wordType found within the Word object
                    } // foreach; word and it's corresponding object
                    hWord.WordTypes = hWord.WordTypes.Distinct().ToArray();
                } // foreach; kvp

                while (lln.Previous != null) { lln = lln.Previous; } // while; lln reverted back to root
                string[] sentencePattern = new string[llSentence.Count];
                int patternIndex = 0;

                // TODO: --1-- put all the code below in a function. that way we can check the next word/node, retain it, then put it in place for the current and grab the next nodes values.
                while (lln != null)
                {

                    // TODO: --1-- continue adding rules to be checked for each word and preceeding words. NEED: concrete "this is typically a noun" or "this is typically a verb" etc rules that we can use to check the next word. currently only checking the previous word.

                    List<string> whichIsIt = new List<string>();

                    // Every single word gets checked for these as they are typically retain this type consistently throughout english
                    if (lln.Value.CurrentWord.ToLower().Equals("the") || lln.Value.CurrentWord.ToLower().Equals("a"))
                    { // is definite article
                        sentencePattern[patternIndex] = "A";
                        goto WeContinued;
                    }
                    else if (pronouns.Contains(lln.Value.CurrentWord.ToLower()))
                    { // is pronoun
                        sentencePattern[patternIndex] = "N";
                        goto WeContinued;
                    }
                    else if (prepositions.Contains(lln.Value.CurrentWord.ToLower()))
                    { // is preposition
                        sentencePattern[patternIndex] = "P";
                        goto WeContinued;
                    }
                    else if (lln.Value.CurrentWord.Length > 3)
                    { // is adverb
                        string lastTwoLetters = lln.Value.CurrentWord.Substring(lln.Value.CurrentWord.Length - 2).ToLower();

                        if (lastTwoLetters.Equals("ly"))
                        { // is an adverb
                            sentencePattern[patternIndex] = "D";
                            goto WeContinued;
                        }
                    }
                    else if (lln.Value.CurrentWord.Length > 4)
                    {
                        string lastTwoLetters = lln.Value.CurrentWord.Substring(lln.Value.CurrentWord.Length - 2).ToLower();
                        string lastThreeLetters = lln.Value.CurrentWord.Substring(lln.Value.CurrentWord.Length - 3).ToLower();
                        if (lastTwoLetters.Equals("ed") || lastThreeLetters.Equals("ing"))
                        { // is past or a present-tense verb
                            sentencePattern[patternIndex] = "V";
                            goto WeContinued;
                        }
                    }

                    if (lln.Value.WordTypes.Length > 1)
                    { // Word Checking
                        string previousLetter = "?";
                        if (patternIndex > 0)
                        {
                            previousLetter = sentencePattern[patternIndex - 1];
                        }

                        foreach (string wdType in lln.Value.WordTypes)
                        { // Type Checking
                            switch (wdType)
                            {
                                case "n.":
                                case "pron.":
                                case "object.":
                                case "obj.":
                                    if (previousLetter.Equals("J") || previousLetter.Equals("A"))
                                    { // previous letter is an adjective or definite article, next is likely a noun
                                        whichIsIt.Add("N");
                                        break;
                                    }
                                    goto default;
                                case "v.":
                                case "p.p.":
                                case "imp.":
                                    if (!previousLetter.Equals("V"))
                                    { // previous letter is an adjective or definite article, next is likely a noun
                                        whichIsIt.Add("V");
                                        break;
                                    }
                                    goto default;
                                case "adj.":
                                case "superl.":
                                case "a.":
                                    whichIsIt.Add("J");
                                    break;
                                case "adv.":
                                    whichIsIt.Add("D");
                                    break;
                                case "conj.":
                                    whichIsIt.Add("C");
                                    break;
                                case "prep.":
                                    whichIsIt.Add("P");
                                    break;
                                case "definite article.":
                                    if (!previousLetter.Equals("A") && !lln.Value.CurrentWord.ToLower().Equals("the") && !lln.Value.CurrentWord.ToLower().Equals("a"))
                                    { // previous letter is an adjective or definite article, next is likely a noun
                                        whichIsIt.Add("A");
                                        break;
                                    }
                                    goto default;
                                default:
                                    whichIsIt.Add("?");
                                    break;
                            } // switch
                        } // foreach; word type

                        if (whichIsIt.Count == 1)
                        {
                            sentencePattern[patternIndex] = whichIsIt[0];
                        }
                        else
                        {
                            sentencePattern[patternIndex] = "?"; // cannot verify so left as unknown
                        }
                    } // if
                    else
                    {
                        switch (lln.Value.WordTypes[0])
                        {
                            case "n.":
                            case "pron.":
                            case "object.":
                            case "obj.":
                                sentencePattern[patternIndex] = "N";
                                break;
                            case "v.":
                            case "p.p.":
                            case "imp.":
                                sentencePattern[patternIndex] = "V";
                                break;
                            case "adj.":
                            case "superl.":
                            case "a.":
                                sentencePattern[patternIndex] = "J";
                                break;
                            case "adv.":
                                sentencePattern[patternIndex] = "D";
                                break;
                            case "conj.":
                                sentencePattern[patternIndex] = "C";
                                break;
                            case "prep.":
                                sentencePattern[patternIndex] = "P";
                                break;
                            case "definite article.":
                                sentencePattern[patternIndex] = "A";
                                break;
                            default:
                                sentencePattern[patternIndex] = "?";
                                break;
                        } // switch
                    } // else
                    WeContinued:
                        patternIndex++;
                        lln = lln.Next;
                } // while
                wordPattern = sentencePattern;
            } // try
            catch (Exception e)
            {
                UniversalFunctions.LogToFile("Exception", e);
            } // catch

            return wordPattern;
        } // function; GetSeteneceWordTypePattern

    } // class SentenceFunctions
} // namespsace
