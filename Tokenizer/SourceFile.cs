using System;

namespace Tokenizer
{
	class SourceFile : IVisitable
	{
        public void Accept(ITokenVisitor visitor)
        {
            foreach(IVisitableToken token in tokens)
            {
                token.Accept(visitor);
            }
        }

        public void insert(String str)          //读入java数据
        { 
            int z = 0, cnt = 0;
            bool f = false, con = false, fs = false, fse = false, fstr = false, ff = false, ft = false;
            String word = "";
            foreach (char c in str)
            {
                if (c == '\\'){
                    word = word + c;
                    ff = true;
                }
                else if (ff)
                {
                    word = word + c;
                    ff = false;
                }
                else if(c == '\"' && !fstr)
                {
                    fstr = true;
                    word = "\"";
                }
                else if (fstr)
                {
                    word = word + c;
                    if(c == '\"')
                    {
                        judge(z++, word);
                        word = "";
                        fstr = false;
                    }
                }
                else if(fs == true && c == '*')
                {
                    word = word + c;
                    fse = true;
                }
                else if (fse == true && c == '/')
                {
                    word = word + c;
                    String[] line = word.Split('\n');
                    foreach(String ss in line)
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
                    if (c == '}') cnt--;
                    if (ft == true && (!word.Equals("") || c == '}' || c == '@'))
                    {
                        for(int i = 0; i < cnt; i++)
                        {
                            judge(z++, "\t");
                        }
                        ft = false;
                    }
                    if (!word.Equals("")) judge(z++, word);
                    if (c == '{') cnt++;
                    if (c != '\t' && !(ft && c == ' ')) judge(z++, c.ToString());
                    if(c == '\n') ft = true;
                    word = "";
                    con = f = false;
                }
            }

            for(;z < 5000; z++)
            {
                tokens[z] = new WhitespaceToken("");
            }
        }

        protected void judge(int num, String word)              //java各字段判别
        {
            if (word.Equals("import") || word.Equals("class") || word.Equals("static") || word.Equals("package") ||
                word.Equals("private") || word.Equals("protected") || word.Equals("public") || word.Equals("return") || word.Equals("new") ||
                word.Equals("true") || word.Equals("false") || word.Equals("throws") || word.Equals("throw") || word.Equals("this") ||
                word.Equals("abstract") || word.Equals("break") || word.Equals("catch") || word.Equals("const") || word.Equals("default") || 
                word.Equals("continue") || word.Equals("default") || word.Equals("do") || word.Equals("else") || word.Equals("if") ||
                word.Equals("extends") || word.Equals("final") || word.Equals("finally") || word.Equals("for") || word.Equals("goto") ||
                word.Equals("implements") || word.Equals("instanceof") || word.Equals("interface") || word.Equals("native") ||
                word.Equals("super") || word.Equals("switch") || word.Equals("synchronized") || word.Equals("transient") || word.Equals("case") || 
                word.Equals("while") || word.Equals("try") || word.Equals("volatile") || word.Equals("main") || word.Equals("super"))
                tokens[num] = new KeywordToken(word);
            else if (word.Equals(" ") || word.Equals("\n") || word.Equals("\t"))
                tokens[num] = new WhitespaceToken(word);
            else if (word.Equals(";") || word.Equals("{") || word.Equals("}") || word.Equals("(") || word.Equals(")"))
                tokens[num] = new PunctuatorToken(word);
            else if (word.Equals("+") || word.Equals("-") || word.Equals("*") || word.Equals("/") || word.Equals("."))
                tokens[num] = new OperatorToken(word);
            else if (word.Equals("String") || word.Equals("int") || word.Equals("long") || word.Equals("short") || word.Equals("char") ||
                    word.Equals("Map") || word.Equals("HashMap") || word.Equals("boolean") || word.Equals("File") || word.Equals("null") ||
                    word.Equals("void") || word.Equals("double") || word.Equals("float") || word.Equals("byte"))
                tokens[num] = new STLToken(word);
            else if (word.Equals("\r"))
                tokens[num] = new IdentifierToken("");
            else if (word.Length >= 2 && (word.Substring(0, 2).Equals("//")))
            {
                tokens[num] = new AnnotationToken(word);
            }
            else if(word.Length >= 1 && word.Substring(0, 1).Equals("\""))
            {
                tokens[num] = new StringLiteralToken(word);
            }
            else
                tokens[num] = new IdentifierToken(word);
        }

        private IVisitableToken[] tokens = new IVisitableToken[5000];
	}

	class IdentifierToken : DefaultTokenImpl, IVisitableToken
	{
		public IdentifierToken(string name)
			: base(name)
		{
		}

		void IVisitable.Accept(ITokenVisitor visitor)
		{
			visitor.VisitIdentifier(this.ToString());
		}
	}

	class KeywordToken : DefaultTokenImpl, IVisitableToken
	{
		public KeywordToken(string name)
			: base(name)
		{
		}

		void IVisitable.Accept(ITokenVisitor visitor)
		{
			visitor.VisitKeyword(this.ToString());
		}
	}

	class WhitespaceToken : DefaultTokenImpl, IVisitableToken
	{
		public WhitespaceToken(string name)
			: base(name)
		{
		}

		void IVisitable.Accept(ITokenVisitor visitor)
		{
			visitor.VisitWhitespace(this.ToString());
		}
	}

	class PunctuatorToken : DefaultTokenImpl, IVisitableToken
	{
		public PunctuatorToken(string name)
			: base(name)
		{
		}

		void IVisitable.Accept(ITokenVisitor visitor)
		{
			visitor.VisitPunctuator(this.ToString());
		}
	}

	class OperatorToken : DefaultTokenImpl, IVisitableToken
	{
		public OperatorToken(string name)
			: base(name)
		{
		}

		void IVisitable.Accept(ITokenVisitor visitor)
		{
			visitor.VisitOperator(this.ToString());
		}
	}

	class StringLiteralToken : DefaultTokenImpl, IVisitableToken
	{
		public StringLiteralToken(string name)
			: base(name)
		{
		}

		void IVisitable.Accept(ITokenVisitor visitor)
		{
			visitor.VisitStringLiteral(this.ToString());
		}
	}

    class STLToken : DefaultTokenImpl, IVisitableToken
    {
        public STLToken(string name)
            : base(name)
        {
        }

        void IVisitable.Accept(ITokenVisitor visitor)
        {
            visitor.VisitSTL(this.ToString());
        }
    }

    class AnnotationToken : DefaultTokenImpl, IVisitableToken
    {
        public AnnotationToken(string name)
            : base(name)
        {
        }

        void IVisitable.Accept(ITokenVisitor visitor)
        {
            visitor.VisitAnnotation(this.ToString());
        }
    }
}
