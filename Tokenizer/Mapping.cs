using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tokenizer
{
    class Mapping
    {

        private String[] javastr = new String[]{
            "import", "final", "out", "package", "boolean",
            "extends", "instanceof", "super", "main", "println",
            "print", "setter", "getter"
        };
        private String[] csharpstr = new String[]{
            "using //未知，原包为", "sealed", "Console", "namespace", "bool",
            ":", "is", "base", "Main", "WriteLine",
            "Write", "set", "get"
        };

        protected String WordChange(String word)
        {
            for (int i = 0; i < javastr.Length; i++)
                if (word.Equals(javastr[i]))
                    return csharpstr[i];
            return word;
        }

        public String TextChange(String Text)
        {
            String newText = "";
            String word = "";
            foreach(char c in Text)
            {
                if (c == '\\' || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) word = word + c;
                else
                {
                    if (!word.Equals(""))
                        newText = newText + WordChange(word);
                    newText = newText + c.ToString();
                    word = "";
                }
            }
            return newText;
        }
    }
}
