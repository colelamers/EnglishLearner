using System;
using System.Collections.Generic;
using System.Text;

namespace EnglishLearner
{
    // TODO: --1-- the elements of this class may need to be static so they are ALWAYS the same in any child classes that extend it. REMEMBER THIS FOR LATER!
    class Brain
    {
        // TODO: --1-- gotta implement a Tree somehow...
        public List<Phrase> Sentence_Memory { get; set; }
        // TODO: --1-- should contain all the memory? or just functions. haven't hashed this out yet.
        // TODO: --1-- something like this? List<Func<string, string>> item = new List<Func<string, string>();
        // TODO: --1-- need implement functional logic here and establish what a "Brain" is comprised of. This will control the logic of phrases and handle parsing, etc.
    }
}
