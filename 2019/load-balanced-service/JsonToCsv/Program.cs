using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonToCsv
{
    public class Data
    {
        public Trace[] data;
    }

    public class Trace
    {
        public Span[] spans;
    }

    public class Span
    {
        public string operationName;
        public string startTime;
        public string duration;

        public Tag[] tags;
    }

    public class Tag
    {
        public string key;
        public string value;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Data data = JsonConvert.DeserializeObject<Data>(File.ReadAllText(@"E:\10\response.json"));

            using (StreamWriter file = new StreamWriter(@"E:\10\request.csv"))
            {
                string line = "operationName,startTime,duration,word,recordsRead,machineName";
                file.WriteLine(line);

                foreach (Trace trace in data.data)
                {
                    Span span = trace.spans[0];

                    line = $"{span.operationName},{span.startTime},{span.duration},{GetKey(span.tags, "word")},{GetKey(span.tags, "recordsRead")},{GetKey(span.tags, "machine")}";

                    file.WriteLine(line);
                }
            }
        }

        static string GetKey(IEnumerable<Tag> tags, string key)
        {
            return tags.Single(t => t.key == key).value;
        }
    }
}
