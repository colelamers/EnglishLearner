using System;
using System.Collections.Generic;
using System.Text;

namespace EnglishLearner
{
    public static class StringExtension
    {
        public static string ToProper(this string st)
        {
            char[] word = st?.ToCharArray();
            word[0] = char.ToUpper(word[0]);
            return new string(word);
        } // function ToProper

/*
 * Another way of doing ToProper
        public static string ToProper(this string st)
        {
            char capitalFirstLetter = st.ToUpper().ToCharArray()[0]; // Capitalize First letter
            string modulusCharacters = st.Remove(0, 1);              // Remove first character from residual letters
            return capitalFirstLetter + modulusCharacters;           // Combine capitalized with original set
        } // function ToProper
*/
    } // String extension class
}
