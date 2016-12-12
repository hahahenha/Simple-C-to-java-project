using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tokenizer
{
    class ChangeFile : IVisitable
    {
        public void Accept(ITokenVisitor visitor)
        {
            foreach (IVisitableToken token in tokens)
            {
                token.Accept(visitor);
            }
        }

        public void insert(String str)          //读入c#数据
        {
            int z = 0, cnt = 0;
            bool f = false, con = false, fs = false, fse = false, fstr = false, ff = false, fc = false, fi = true, ft = false;
            String word = "";
            foreach (char c in str)
            {
                if (fi && c != 'u') continue;
                if (fi)
                {
                    judge(z++, "using");
                    judge(z++, " ");
                    judge(z++, "System;\n");
                }
                fi = false;
                if (c == '\\')
                {
                    word = word + c;
                    ff = true;
                }
                else if (ff)
                {
                    word = word + c;
                    ff = false;
                }
                else if (c == '\"' && !fstr)
                {
                    fstr = true;
                    word = "\"";
                }
                else if (fstr)
                {
                    word = word + c;
                    if (c == '\"')
                    {
                        judge(z++, word);
                        word = "";
                        fstr = false;
                    }
                }
                else if (fs == true && c == '*')
                {
                    word = word + c;
                    fse = true;
                }
                else if (fse == true && c == '/')
                {
                    word = word + c;
                    String[] line = word.Split('\n');
                    foreach (String ss in line)
                    {
                        tokens[z++] = new AnnotationToken(ss);
                    }
                    word = "";
                    fs = false;
                    fse = false;
                    con = false;
                }
                else if (fs == true)
                {
                    fse = false;
                    word = word + c;
                }
                else if (con == true && !f && c == '*')
                {
                    word = "/*";
                    fs = true;
                }
                else if (con == true && c == '/')
                {
                    word = "//";
                    f = true;
                }
                else if (f == true && (c != '\n' && c != '\r'))
                {
                    word = word + c;
                }
                else if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) word = word + c;
                else if (c == '/')
                {
                    con = true;
                }
                else
                {
                    if(!fc && word.Equals("public"))
                    {
                        fc = true;
                        judge(z++, "namespace");
                        judge(z++, " ");
                        judge(z++, namespaces + "{\n");
                        ft = true;
                        cnt++;
                    }
                    if (c == '}') cnt--;
                    if (ft == true && (!word.Equals("") || c == '}' || c == '@'))
                    {
                        for (int i = 0; i < cnt; i++)
                        {
                            judge(z++, "\t");
                        }
                        ft = false;
                    }
                    if (!word.Equals("")) judge(z++, word);
                    if (c == '{') cnt++;
                    if (c != '\t' && !(ft && c == ' ')) judge(z++, c.ToString());
                    if (c == '\n') ft = true;
                    word = "";
                    con = f = false;
                }
            }
            judge(z++, "}\n");
            for (; z < 5000; z++)
            {
                tokens[z] = new WhitespaceToken("");
            }
        }

        protected void judge(int num, String word)              //c#各字段判别
        {
            if (word.Equals("using") || word.Equals("class") || word.Equals("static") || word.Equals("namespace") ||
                word.Equals("private") || word.Equals("protected") || word.Equals("public") || word.Equals("return") || word.Equals("new") ||
                word.Equals("true") || word.Equals("false") || word.Equals("throws") || word.Equals("throw") || word.Equals("this") ||
                word.Equals("abstract") || word.Equals("break") || word.Equals("catch") || word.Equals("const") || word.Equals("default") || 
                word.Equals("continue") || word.Equals("default") || word.Equals("do") || word.Equals("else") || word.Equals("if") ||
                word.Equals(":") || word.Equals("seal") || word.Equals("finally") || word.Equals("for") || word.Equals("goto") ||
                word.Equals("implements") || word.Equals("instanceof") || word.Equals("interface") || word.Equals("native") ||
                word.Equals("super") || word.Equals("switch") || word.Equals("synchronized") || word.Equals("transient") || word.Equals("case") || 
                word.Equals("while") || word.Equals("try") || word.Equals("volatile") || word.Equals("Main") || word.Equals("base")
                )
                tokens[num] = new KeywordToken(word);
            else if (word.Equals(" ") || word.Equals("\n") || word.Equals("\t"))
                tokens[num] = new WhitespaceToken(word);
            else if (word.Equals(";") || word.Equals("{") || word.Equals("}") || word.Equals("(") || word.Equals(")"))
                tokens[num] = new PunctuatorToken(word);
            else if (word.Equals("+") || word.Equals("-") || word.Equals("*") || word.Equals("/") || word.Equals("."))
                tokens[num] = new OperatorToken(word);
            else if (word.Equals("String") || word.Equals("int") || word.Equals("long") || word.Equals("short") || word.Equals("char") ||
                    word.Equals("Map") || word.Equals("HashMap") || word.Equals("bool") || word.Equals("File") || word.Equals("null") ||
                    word.Equals("void") || word.Equals("double") || word.Equals("float") || word.Equals("byte") || word.Equals("Console") ||
                    word.Equals("is"))
                tokens[num] = new STLToken(word);
            else if (word.Equals("\r"))
                tokens[num] = new IdentifierToken("");
            else if (word.Length >= 2 && (word.Substring(0, 2).Equals("//")))
            {
                tokens[num] = new AnnotationToken(word);
            }
            else if (word.Length >= 1 && word.Substring(0, 1).Equals("\""))
            {
                tokens[num] = new StringLiteralToken(word);
            }
            else
                tokens[num] = new IdentifierToken(word);
        }

        private IVisitableToken[] tokens = new IVisitableToken[5000];

        public String namespaces = "MyNameSpace";
    }
}
