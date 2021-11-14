using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EnglishLearner
{
    /*
     * Created by Cole Lamers 
     * Date: 2021-11-04
     * 
     * == Purpose ==
     * Handling of sentence parsing, asserting, and checking
     * 
     * 
     */
    public static class SentenceFunctions
    {
        private static readonly char[] endPunctuation = new char [3] { '.', '?', '!' };

        private static readonly List<string> wordTypes = new List<string> 
        { 
            "n.",      //noun
            "pron.",   //pronoun
            "object",  //another type for pronoun like "I"
            "v.",      //verb
            "adv.",    //adverb
            "imp.",    //Past tense word; -ed
            "p.p.",    //Past tense word; -ed
            "conj.",   //and, both, because, or, then, until, if
            "adj.",    //adjective
            "superl.", //adjective
            "a.",      //adjective
            "prep."    //preposition; on, onto, in, to, through, from, at

        }; // TODO: --1-- consider changing into a dictionary instead so we can return what we want for the values? KISS for word types.

        // TODO: --3-- might want to rename this class because it handles paragraph parsing too

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
            catch(Exception e)
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

        
        public static string ToProper(string word)
        {
            // TODO: --1-- make a function that makes a word First letter captialized
            return word;
        }

        public static string[] RemovePunctuation(string[] sentence)
        {
            // TODO: --1-- make a functiont that removes the { ., !, ?  } at the end of a sentence, but also then retains it or something...idk. ignore it for now but make a note of it either way.
            return sentence;
        }

        public static (string[], char) GetSplitSentenceAndPunctuation(string sentence)
        {
            // TODO: --1-- hunter; verify this is correct

            string[] sentenceAsArray = sentence.Split(" ");
            int num;
            sentenceAsArray = sentenceAsArray.Where(x => !string.IsNullOrWhiteSpace(x)).Where(x => !int.TryParse(x, out num)).ToArray();

            string finalWord = sentenceAsArray[sentenceAsArray.Length - 1];
            int finalWordPunctuationIndex = finalWord.Length - 1;
            char punctuation = '.';

            if (finalWord.IndexOfAny(endPunctuation) >= 0)
            {
                punctuation = finalWord[finalWordPunctuationIndex];
                finalWord = finalWord.Remove(finalWordPunctuationIndex);
                sentenceAsArray[sentenceAsArray.Length - 1] = finalWord;
            } // if; checks for end sentence punctuation and updates it if it's different and removes it from the split setence

            return (sentenceAsArray, punctuation);
        } // function GetSplitSentence

        public static string GetFirstWordProper(string[] splitSentence)
        {
            // TODO: --1-- hunter; verify this is correct

            char[] capitalizedFirstLetter = splitSentence[0].ToCharArray();
            capitalizedFirstLetter[0] = char.ToUpper(capitalizedFirstLetter[0]);
            return new string(capitalizedFirstLetter);
        } // function GetFirstWordProper

        private static DataRowCollection CreateAndExecuteQuery(string[] sentence, string configPath)
        {
            // TODO: --1-- need to make dynamically create table - cole will do this
            Sqlite_Actions _sql = new Sqlite_Actions(configPath, "Dictionary");
            string buildQuery = $"SELECT DISTINCT word, wordtype FROM entries WHERE word in ('{sentence[0]}'";

            for(int i = 1; i < sentence.Length; i++)
            {
                buildQuery += $",'{sentence[i]}'";
            }
            buildQuery += ");";

            _sql.ExecuteQuery(@$"{buildQuery}");
            return _sql.ActiveQueryResults.Rows;
        } // function CreateAndExecuteQuery

        public static List<char> GetSeteneceWordTypePattern(string[] sentence, string configPath)
        {
            List<char> wordPattern = new List<char>();

           DataRowCollection queryResults = CreateAndExecuteQuery(sentence, configPath);
            foreach (DataRow dRow in queryResults)
            {
                // TODO: --1-- Hunter
                //dRow["word"]
                //dRow["wordType"]
            }

            return wordPattern;
        } 
    }
}
