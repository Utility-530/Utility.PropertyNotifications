using Utility.Attributes;
using Utility.Enums;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Demo.Lists.Factories;
using Utility.PropertyNotifications;

namespace Utility.Nodes.Demo.Lists.Models
{


    [Model("2cdd17e0-51b6-4fde-944a-c14382f95597", nameof(NodeMethodFactory.BuildDatabaseChangeRoot), 5)]
    public class DatabaseChangeModel : NotifyPropertyClass, IId<Guid>, IClone, Interfaces.NonGeneric.IFactory
    {
        public DatabaseChangeModel() { }

        [FactoryAttribute]
        public DatabaseChangeModel(object obj)
        {

            Id = Guid.NewGuid();
            Currency = Currency.GBP;
        }

        public Guid Id { get; set; }

        public string String { get; set; }


        public Currency Currency { get; set; }

        public object Clone()
        {
            var clone = AnyClone.CloneExtensions.Clone(this);
            clone.Id = Guid.NewGuid();
            return clone;
        }

        public object Create(object config)
        {
            return new DatabaseChangeModel(null);
        }
    }
}
