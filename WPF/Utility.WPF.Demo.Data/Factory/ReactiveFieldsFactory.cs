using System;
using System.Collections.Generic;
using AutoBogus;
using Utility.WPF.Demo.Common.ViewModels;

namespace Utility.WPF.Demo.Data.Factory
{
    public class ReactiveFieldsFactory
    {
        private Lazy<Bogus.Faker<ReactiveFields>> fieldsFaker = new(() =>
          {
              return new AutoFaker<ReactiveFields>()
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

        public IEnumerator<ReactiveFields> Build()
        {
            return fieldsFaker.Value.GenerateForever().GetEnumerator();
        }

        //static FieldsFactory()
        //{
        //    // Configure globally
        //    AutoFaker.Configure(builder =>
        //    {
        //        builder
        //          .WithLocale("en")             // Configures the locale to use
        //          .WithRepeatCount(10)        // Configures the number of items in a collection
        //          /*        .WithDataTableRowCount() */ // Configures the number of data table rows to generate
        //          .WithRecursiveDepth(1);     // Configures how deep nested types should recurse
        //          //.WithTreeDepth()      // Configures the tree depth of an object graph
        //          //.WithBinder()             // Configures the binder to use
        //          //.WithFakerHub()           // Configures a Bogus.Faker instance to be used - instead of a default instance
        //          //.WithSkip()               // Configures members to be skipped for a type
        //          //.WithOverride();          // Configures the generator overrides to use - can be called multiple times
        //    });
        //}
    }
}