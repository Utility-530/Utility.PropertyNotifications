using System;
using System.Windows.Markup;

namespace Utility.WPF.Templates
{
    public class NullableExtension : TypeExtension
    {
        public NullableExtension()
        {
        }

        public NullableExtension(string type)
            : base(type)
        {
        }

        public NullableExtension(Type type)
            : base(type)
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Type basis = (Type)base.ProvideValue(serviceProvider);
            return typeof(Nullable<>).MakeGenericType(basis);
        }
    }
}