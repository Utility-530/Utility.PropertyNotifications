using Utility.Attributes;
using Utility.Enums;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;

namespace Utility.Entities
{
    public class Asset : Entity, IId<Guid>, IClone, IFactory
    {
        public Asset() { }

        [Factory]
        public Asset(object obj)
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string OrderId { get; set; }

        [Column(width: 200)]
        public string Description { get; set; }

        public AssetType AssetType { get; set; }

        [Column(width: 200)]
        public string Name { get; set; }

        public FinancialStorageType StorageType { get; set; }
        public string Storage { get; set; }

        [DataType(Enums.DataType.Money)]
        public decimal Value { get; set; }

        public int Count { get; set; }
        public double Quantity { get; set; }
        public QuantityType QuantityType { get; set; }
        public string QuantityUnit { get; set; }

        [DataType(Enums.DataType.Money)]
        public decimal? Limit { get; set; }

        [DataType(Enums.DataType.Percentage)]
        public decimal? InitialFee { get; set; }

        // purity/condition rating/etc. as a percentage
        [DataType(Enums.DataType.Percentage)]
        public double? Quality { get; set; }

        public string? Buyer { get; set; }
        public string Seller { get; set; }

        // purchase price or initial cost
        public decimal Cost { get; set; }

        // sale value
        public decimal? Gain { get; set; }

        [DataType(Enums.DataType.Percentage)]
        public decimal Interest { get; set; }

        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        [Column(width: 300)]
        public string? Notes { get; set; }

        public object Clone()
        {
            return new Asset
            {
                Id = Guid.NewGuid(),
                OrderId = OrderId,
                Description = Description,
                AssetType = AssetType,
                Name = Name,
                StorageType = StorageType,
                Storage = Storage,
                Value = Value,
                Count = Count,
                Quantity = Quantity,
                QuantityType = QuantityType,
                QuantityUnit = QuantityUnit,
                Limit = Limit,
                InitialFee = InitialFee,
                Quality = Quality,
                Buyer = Buyer,
                Seller = Seller,
                Cost = Cost,
                Gain = Gain,
                Interest = Interest,
                Start = Start,
                End = End,
                Notes = Notes,
            };
        }

        public object Create(object config)
        {
            return new Asset(config);
        }
    }


}