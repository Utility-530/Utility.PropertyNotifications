using ReactiveUI;
using System;
using Utility.Common.Models;
using Utility.Reactive;
using Utility.Interfaces.NonGeneric;

namespace Utility.ViewModels.Filters
{
    public class RandomFilter : Filter, IPredicate
    {
        private readonly Random random = new();

        public RandomFilter() : base("Random")
        {
            Value = new ReactiveProperty<int>(random.Next(0, 2));
        }
        public override ReactiveProperty<int> Value { get; }

        public override bool Invoke(object _)
        {
            var value = random.Next(0, 2);
            var truth = Value.Value > 0;
            Value.Value = value;
            return truth;
        }
    }
}
