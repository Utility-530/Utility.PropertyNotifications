using Utility.Attributes;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;

namespace Utility.Entities
{

    public class Loan : Entity, IId<Guid>, IClone, IFactory, IGetAmount
    {
        public Loan() { }

        [Factory]
        public Loan(object obj)
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        [Column(width: 200)]
        public string Name { get; set; }

        [DataType(Enums.DataType.Money)]
        public decimal Amount { get; set; }

        [DataType(Enums.DataType.Percentage)]
        public decimal Interest { get; set; }

        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        [DataType(Enums.DataType.Money)]
        public decimal Limit { get; set; }

        [DataType(Enums.DataType.Money)]
        public decimal InitialFee { get; set; }

        [DataType(Enums.DataType.Money)]
        public decimal MonthlyFee { get; set; }

        public object Clone()
        {
            return new Loan
            {
                Id = Guid.NewGuid(),
                Name = Name,
                Amount = Amount,
                Interest = Interest,
                Start = Start,
                End = End,
                Limit = Limit,
                InitialFee = InitialFee,
            };
            
        }

        public object Create(object config)
        {
            return new Loan(null);
        }
    }
}