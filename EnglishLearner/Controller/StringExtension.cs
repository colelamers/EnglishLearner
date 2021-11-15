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
    } // String extension class
}
