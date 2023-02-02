using System.Collections.Generic;
using Utility.Helpers;

namespace UtilityWpf.Demo.Common.Meta
{
    internal class KeyStore
    {
        public static HashSet<string> Keys { get; } = new();

        public string CreateNewKey(string word = "")
        {
            while (string.IsNullOrEmpty(word) || Keys.Add(word) == false)
                word = RandomHelper.NextWord(Statics.Random, 5);
            return word;
        }
    }
}