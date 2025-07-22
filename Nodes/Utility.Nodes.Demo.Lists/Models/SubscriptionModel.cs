using CsvHelper;
using Utility.Attributes;
using Utility.Enums;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Demo.Lists.Factories;
using Utility.PropertyNotifications;

namespace Utility.Nodes.Demo.Lists.Models
{
    [Model("d17c5de2-7836-4c02-958c-eb1de974f474", nameof(NodeMethodFactory.BuildUserProfileRoot))]
    public class SubscriptionModel : NotifyPropertyClass, IId<Guid>, IClone, Interfaces.NonGeneric.IFactory
    {
        public SubscriptionModel() { }

        [FactoryAttribute]
        public SubscriptionModel(object obj)
        {

            Id = Guid.NewGuid();
            Currency = Currency.GBP;
            Frequency = TimeInterval.Month;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public double Cost { get; set; }

        public Currency Currency { get; set; }

        public TimeInterval Frequency { get; set; }

        public object Clone()
        {
            var clone = AnyClone.CloneExtensions.Clone(this);
            clone.Id = Guid.NewGuid();
            return clone;
        }

        public object Create(object config)
        {
            return new SubscriptionModel(null);
        }
    }
}
