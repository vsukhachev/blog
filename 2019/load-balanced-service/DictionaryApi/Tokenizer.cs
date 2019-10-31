using System;
using System.Text;

namespace DictionaryApi
{
    public class Tokenizer: IDisposable
    {
        private readonly Input input;

        public Tokenizer(string str)
        {
            this.input = new Input(str);
        }

        public Token GetNextToken()
        {
            if(input.IsEof())
                return new Token(TokenType.Eof);

            //for (char c = str[pos]; pos < str.Length; c = str[pos++])
            while (!input.IsEof())
            {
                char c = input.Read();

                if (char.IsWhiteSpace(c))
                    continue;

                if (c == '{' || c == '}')
                    continue;

                if (c == ',')
                {
                    return Token.Comma;
                }

                if (c == ':')
                {
                    return Token.Delimiter;
                }

                if (c == '\"')
                {
                    StringBuilder s = new StringBuilder();

                    while (!input.IsEof())
                    {
                        char c1 = input.Read();

                        if (c1 == '\\')
                        {
                            char? c2 = input.Peek();
                            if (c2 == '\"')
                            {
                                s.Append(input.Read());
                                continue;
                            }
                        }

                        if (c1 == '\"')
                        {
                            return new Token(TokenType.String, s.ToString());
                        }

                        s.Append(c1);
                    }
                }
            }

            return Token.Eof;
        }

        public enum TokenType
        {
            Eof,
            Delimiter,
            String,
            Comma,
        }

        public class Token
        {
            public static Token Eof = new Token(TokenType.Eof);
            public static Token Delimiter = new Token(TokenType.Delimiter);
            public static Token Comma = new Token(TokenType.Comma);

            public Token(TokenType type)
            {
                Type = type;
            }

            public Token(TokenType type, string s)
            {
                Type = type;
                String = s;
            }

            public TokenType Type { get; private set; }
            public string String { get; private set; }

            public override string ToString()
            {
                if (Type == TokenType.String)
                    return this.String;

                return Type.ToString();
            }
        }

        public void Dispose()
        {
            input?.Dispose();
        }
    }
}
