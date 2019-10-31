using System;
using System.IO;

namespace DictionaryApi
{
    public class Input: IDisposable
    {
        private readonly FileStream fs;
        private readonly BufferedStream bs;
        private readonly StreamReader sr;

        public Input(string path)
        {
            fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            bs = new BufferedStream(fs);
            sr = new StreamReader(bs);
        }

        public char Read()
        {
            return (char)sr.Read();
        }

        public char? Peek()
        {
            int c = sr.Peek();

            if (c == -1)
                return null;

            return (char) c;
        }

        public bool IsEof()
        {
            return Peek() == null;
        }
        
        public void Dispose()
        {
            fs?.Dispose();
            bs?.Dispose();
            sr?.Dispose();
        }
    }
}