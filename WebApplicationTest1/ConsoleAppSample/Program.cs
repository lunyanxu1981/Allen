using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConsoleAppSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var dic = ParseJSONString();
            Console.ReadLine();
        }

        public static Dictionary<string, int> ParseJSONString()
        {
            string json = @"{""key1"":""1"",""key2"":""0""}";

            var values = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);

            return values;
        }
    }
}
