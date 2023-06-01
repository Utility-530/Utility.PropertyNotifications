using System.ComponentModel;
using System.Windows.Input;
using Utility.Commands;
using Utility.Helpers;

namespace Utility.PropertyTrees
{
    public class ReferenceProperty : PropertyBase
    {
        public ReferenceProperty(Guid guid) : base(guid)
        {
            AddCommand = new ObservableCommand(async o =>
            {
                o.OnNext(false);

                if (Data is IList collection)
                {
                    var type = Data.GetType().GetGenericArguments().SingleOrDefault();
                    var instance = Activator.CreateInstance(type);
                    collection.Add(instance);
                    //await RefreshAsync();
                }

                o.OnNext(true);
            });
        }

        public ICommand AddCommand { get; }

        public override string Name => Descriptor?.Name ?? "Descriptor not set";
        public string DisplayName => Descriptor.DisplayName;
        public override bool IsReadOnly => Descriptor.IsReadOnly;
        public virtual string? Category => string.IsNullOrWhiteSpace(Descriptor.Category) ||
                Descriptor.Category.EqualsIgnoreCase(CategoryAttribute.Default.Category)
                    ? null
                    : Descriptor.Category;

        public virtual TypeConverter Converter => Descriptor.Converter;

        public override Type PropertyType => Descriptor.PropertyType;

        public override object Content => Name;

        public override object? Value
        {
            get
            {
                var property = Data;
                return property;
            }
            set => throw new Exception("aa 4 43321``");
        }

        public override bool HasChildren => PropertyType != typeof(string);

        public override string ToString()
        {
            return Name;
        }



    }
}