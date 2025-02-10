using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Tonic.UI
{
    /// <summary>
    /// Bind enum values using the EnumConverter
    /// </summary>
    public class EnumBinding : MarkupExtension
    {
        private readonly Binding binding;

        public EnumBinding(string Path) : this(new Binding(Path))
        {
        }
        public EnumBinding(Binding binding)
        {
            binding.Converter = new EnumConverter();
            this.binding = binding;
        }

        public EnumBinding() : this("") { }


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!(serviceProvider is IProvideValueTarget pvt))
            {
                return null;
            }
            if (!(pvt.TargetObject is FrameworkElement frameworkElement))
            {
                return this;
            }

            if (binding.Path == null)
            {
                return DependencyProperty.UnsetValue;
            }
            return binding.ProvideValue(serviceProvider);
        }
    }
}