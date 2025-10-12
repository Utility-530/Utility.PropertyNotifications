using Utility.Attributes;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Demo.Lists.Factories;
using Utility.PropertyNotifications;

namespace Utility.Nodes.Demo.Lists.Models
{
    [Model("a762c86f-2e02-4b41-afa9-028a9c4fff1b", nameof(NodeMethodFactory.BuildCreditCardRoot), 6)]
    public class CreditCardModel : NotifyPropertyClass, IId<Guid>, IClone, IFactory, IAmount
    {
        public CreditCardModel() { }

        [FactoryAttribute]
        public CreditCardModel(object obj)
        {

            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        [Attributes.Column(width: 200)]
        public string Name { get; set; }

        [DataTypeAttribute(Enums.DataType.Money)]
        public decimal Amount { get; set; }

        [DataTypeAttribute(Enums.DataType.Percentage)]
        public decimal CurrentInterest { get; set; }

        [DataTypeAttribute(Enums.DataType.Percentage)]
        public decimal FutureInterest { get; set; }



        public DateTime? FutureInterestChange { get; set; }

        [DataTypeAttribute(Enums.DataType.Money)]
        public decimal CreditLimit { get; set; }

        [DataTypeAttribute(Enums.DataType.PIN)]
        public string Pin { get; set; }

        [DataTypeAttribute(Enums.DataType.Percentage)]
        public decimal InitialFee { get; set; }

        [System.ComponentModel.ReadOnly(true)]
        public DateTime LastEdit { get; set; }

        public object Clone()
        {
            var clone = AnyClone.CloneExtensions.Clone(this);
            clone.Id = Guid.NewGuid();
            return clone;
        }

        public object Create(object config)
        {
            return new CreditCardModel(null);
        }
    }
}