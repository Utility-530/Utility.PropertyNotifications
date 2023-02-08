using System;
using System.Collections.Generic;
using Utility.Helpers;

namespace Utility.Randoms
{
    public class Next
    {
        public Next(System.Random random)
        {
            Random = random;
        }
        public System.Random Random { get; }

        public double Percent()
        {
            return Random.NextPercent();
        }


        public bool Boolean()
        {
            return Random.NextBoolean();
        }


        public string Word(int length = 4)
        {
            return Random.NextWord(length);
        }

        public int Sign(int factor = 1)
        {
            return Random.NextSign(factor);
        }

        public double Double(double factor = 1d)
        {
            return Random.NextDouble(factor);
        }

        public double Value(double current, double min, double max)
        {
            return Random.NextValue(current, min, max);
        }

        public T Enum<T>(Dictionary<Type, Array>? cache = default) where T : Enum
        {
            return Random.NextEnum<T>(cache);
        }

        public double Levy(double c = 5.5, double mu = 1)
        {
            return Random.NextLevy(c, mu);
        }
        public string Name()
        {
            return Names.Random(Random);
        }

        const string chars = "abcdefghijklmnopqrstuvwxyz";

        public static char AlphabetCharacter(int index)
        {
            return chars[index % 26];
        }
    }
}