using System.ComponentModel;
using Utility.Helpers;

namespace Utility.PropertyTrees
{
    public class ReferenceProperty : PropertyBase
    {
        public ReferenceProperty(Guid guid) : base(guid)
        {
        }

        public override string Name => Descriptor.Name;
        public string DisplayName => Descriptor.DisplayName;
        public override bool IsReadOnly => Descriptor.IsReadOnly;
        public bool IsFlagsEnum => PropertyType.IsFlagsEnum();

        public virtual string? Category => string.IsNullOrWhiteSpace(Descriptor.Category) ||
                Descriptor.Category.EqualsIgnoreCase(CategoryAttribute.Default.Category)
                    ? null
                    : Descriptor.Category;

        public virtual TypeConverter Converter => Descriptor.Converter;

        public virtual PropertyDescriptor Descriptor { get; set; }

        public override Type PropertyType => Descriptor.PropertyType;

        public override object Content => Name;

        protected override async Task<bool> RefreshAsync()
        {
            if ((PropertyType.IsValueType || PropertyType == typeof(string)) != true)
                return await base.RefreshAsync();

            return await Task.FromResult(true);
        }

        public override object? Value
        {
            get
            {
                var property = Data;
                return property;
            }
            set => throw new Exception("aa 4 43321``");
        }

        public override string ToString()
        {
            return Name;
        }



    }
}