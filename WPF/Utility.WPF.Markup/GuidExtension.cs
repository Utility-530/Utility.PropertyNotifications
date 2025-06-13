using System;
using System.Windows.Markup;

namespace Utility.WPF.Markup
{
    [MarkupExtensionReturnType(typeof(Guid))]
    public class GuidExtension : MarkupExtension
    {
        public string Value { get; set; }

        public GuidExtension() { }

        public GuidExtension(string value)
        {
            Value = value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Guid.TryParse(Value, out Guid guid))
            {
                return guid;
            }

            throw new FormatException($"Invalid GUID format: {Value}");
        }
    }
}