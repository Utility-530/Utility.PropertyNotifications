using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Helpers;

namespace Utility.WPF.Templates
{
    public partial class Templates : ResourceDictionary
    {
        public Templates()
        {
            //InitializeComponent();
        }
    }


    public class CustomDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? DefaultDataTemplate { get; set; }
        public DataTemplate? BooleanDataTemplate { get; set; }
        public DataTemplate? StringDataTemplate { get; set; }
        public DataTemplate? EnumDataTemplate { get; set; }
        public DataTemplate? NumberDataTemplate { get; set; }
        public DataTemplate? IconDataTemplate { get; set; }
        public DataTemplate? ColorTemplate { get; set; }
        public DataTemplate? IConvertibleTemplate { get; set; }
        public DataTemplate? DictionaryDataTemplate { get; set; }
        public DataTemplate? EnumerableDataTemplate { get; set; }
        public DataTemplate? NullDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var template = Select_Template(item, container);
            return template;
        }
        public ResourceDictionary Templates
        {
            get
            {
                var resourceDictionary = new ResourceDictionary
                {
                    Source = new Uri("/Utility.WPF.Templates;component/Templates.xaml", UriKind.RelativeOrAbsolute)
                };
                return resourceDictionary;
            }
        }

        public DataTemplate Select_Template(object item, DependencyObject container)
        {
            if (item == null)
                return NullDataTemplate ??= NullTemplate();

            var type = item.GetType();

            if (new DataTemplateKey(type) is var key &&
                (container as FrameworkElement)?.TryFindResource(key) is DataTemplate dataTemplate)
                return dataTemplate;

            var interfaces = type.GetInterfaces();

            if (interfaces.Contains(typeof(IConvertible)))
                return IConvertibleTemplate ??= Templates["IConvertable"] as DataTemplate ?? throw new Exception("Missing DataTemplate for IConvertible");
            else if (interfaces.Any(a => a.Name == "IDictionary`2") || interfaces.Contains(typeof(IDictionary)))
                return DictionaryDataTemplate ??= Templates["Dictionary"] as DataTemplate ?? throw new Exception("Missing DataTemplate for Dictionary");
            else if (interfaces.Contains(typeof(IEnumerable)))
                return EnumerableDataTemplate ??= Templates["Enumerable"] as DataTemplate ?? throw new Exception("Missing DataTemplate for Enumerable");
            if (type == typeof(Color))
                return ColorTemplate ??= Templates["Color"] as DataTemplate ?? throw new Exception("Missing DataTemplate for Color");
            //if (type == typeof(Utility.WPF.Abstract.Icon))
            //    return IconDataTemplate;
            if (type == typeof(bool))
                return BooleanDataTemplate ??= Templates["Boolean"] as DataTemplate ?? throw new Exception("Missing DataTemplate for Boolean");
            if (typeof(Enum).IsAssignableFrom(type))
                return EnumDataTemplate ??= Templates["Enum"] as DataTemplate ?? throw new Exception("Missing DataTemplate for Enum");
            if (type == typeof(string))
                return StringDataTemplate ??= Templates["String"] as DataTemplate ?? throw new Exception("Missing DataTemplate for String");
            if (type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(decimal))
                return NumberDataTemplate ??= Templates["Number"] as DataTemplate ?? throw new Exception("Missing DataTemplate for Number");

            return DefaultDataTemplate ??= TemplateGenerator.CreateDataTemplate
                (() => new TextBlock
                {
                    Text = $"{item.GetType().Name} Missing DataTemplate",
                    Margin = new Thickness(1),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Opacity = 0.5
                });
        }

        protected static DataTemplate NullTemplate() =>
           TemplateGenerator.CreateDataTemplate(() => new TextBlock
           {
               FontSize = 14,
               HorizontalAlignment = HorizontalAlignment.Stretch,
               VerticalAlignment = VerticalAlignment.Stretch,
               Text = $"item is null"
           });

        public static CustomDataTemplateSelector Instance => new();
    }
}