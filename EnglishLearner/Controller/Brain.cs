using System;
using System.Collections.Generic;
using System.Text;

namespace EnglishLearner
{
    // TODO: --1-- the elements of this class may need to be static so they are ALWAYS the same in any child classes that extend it. REMEMBER THIS FOR LATER!
    class Brain
    {
        // TODO: --1-- gotta implement a Tree in here for it access the data
        // TODO: --1-- utilize regex to determine what words are what
        // TODO: --3-- will need to implement an IDispoable somewhere either in a Phrase or in a Brain as we'll need to free resources as they're no longer needed or overwritten. This should be considered, however, it will most likely not be important until/unless we're running out of memory. My suggestion is if the object grows beyond 2GB's with many removals or changes, we need to start disposing.
        // TODO: --1-- should contain all the memory? or just functions. haven't hashed this out yet.
        // TODO: --1-- something like this? List<Func<string, string>> item = new List<Func<string, string>();
        // TODO: --1-- need implement functional logic here and establish what a "Brain" is comprised of. This will control the logic of phrases and handle parsing, etc.
        public List<Phrase> Sentence_Memory { get; set; }

        public Brain()
        {
            this.Sentence_Memory = new List<Phrase>();
        } // Constructor


    }
}
