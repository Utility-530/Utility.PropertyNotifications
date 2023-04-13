using System;
using System.Linq;

namespace Utility.WPF.Demo.Data.Model
{
    public class Text
    {
        public string Value { get; } = StringFaker.Value.Generate(1).Single();

        private static readonly Lazy<Bogus.Faker<string>> StringFaker = new(() =>
                  {
                      return new Bogus.Faker<string>().CustomInstantiator((fake) => fake.Lorem.Sentence(10));
                  });

        public override string ToString()
        {
            return Value;
        }
    }
}