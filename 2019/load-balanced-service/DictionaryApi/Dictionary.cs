using System;
using System.IO;

namespace DictionaryApi
{
    public class Dictionary
    {
        public class SearchStatistics
        {
            public SearchStatistics()
            {
                RecordsRead = 0;
            }

            public long RecordsRead { get; private set; }

            public void LogRecordRead()
            {
                RecordsRead++;
            }
        }

        public static string Search(string filePath, string word, out SearchStatistics stat)
        {
            stat = new SearchStatistics();

            using (var tokenizer = new Tokenizer(filePath))
            {
                while (true)
                {
                    Tokenizer.Token key = tokenizer.GetNextToken();

                    stat.LogRecordRead();

                    Tokenizer.Token delimiter = tokenizer.GetNextToken();

                    if (delimiter.Type != Tokenizer.TokenType.Delimiter)
                        throw new Exception("Syntax error: required delimiter");

                    Tokenizer.Token value = tokenizer.GetNextToken();

                    if (key.String.Equals(word, StringComparison.CurrentCultureIgnoreCase))
                        return value.String;

                    Tokenizer.Token nextToken = tokenizer.GetNextToken();

                    if (nextToken.Type == Tokenizer.TokenType.Eof)
                        break;

                    if (nextToken.Type != Tokenizer.TokenType.Comma)
                        throw new Exception("Syntax error: comma required");
                }
            }

            return null;
        }

        private static string ReadFile(string path)
        {
            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}