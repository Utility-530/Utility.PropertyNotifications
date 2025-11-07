using System;
using System.Collections.Generic;
using AutoBogus;
using Utility.WPF.Demo.Common.ViewModels;

namespace Utility.WPF.Demo.Data.Factory
{
    public class FieldsFactory
    {
        private Lazy<Bogus.Faker<Fields>> fieldsFaker = new(() =>
          {
              return new AutoFaker<Fields>()
                  .Configure(builder => builder
                        .WithLocale("en")             // Configures the locale to use
                        /*      .WithRepeatCount(10)  */      // Configures the number of items in a collection
                        .WithRecursiveDepth(1))

                  .RuleFor(fake => fake.Name, fake => fake.Name.FirstName())
                  .RuleFor(fake => fake.Surname, fake => fake.Name.LastName())
                  .RuleFor(fake => fake.Age, fake => fake.Random.Int(0, 100))
                  .RuleFor(fake => fake.PhoneNumber, fake => fake.Phone.PhoneNumber());
              //.FinishWith((a,b)=> b.Id=a.Random.Guid());
          });

        public IEnumerator<Fields> Build()
        {
            return fieldsFaker.Value.GenerateForever().GetEnumerator();
        }

        public IEnumerable<Fields> BuildCollection()
        {
            return fieldsFaker.Value.GenerateLazy(5);
        }
    }
}