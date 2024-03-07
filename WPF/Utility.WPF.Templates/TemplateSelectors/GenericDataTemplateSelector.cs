using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Factorys;

namespace Utility.WPF.Templates
{
    public abstract class GenericDataTemplateSelector : DataTemplateSelector
    {

        public virtual ResourceDictionary Templates { get; }

        public DataTemplate NullTemplate { get; set; }
    }


    public abstract class TemplateFactory
    {
        //public static DataTemplate SelectTemplate(Type type)
        //{

        //    var interfaces = type.GetInterfaces();

        //    if (interfaces.Any(a => a.Name == "IDictionary`2") || interfaces.Contains(typeof(IDictionary)))
        //        throw new Exception("Missing DataTemplate for Dictionary");
        //    else if (type == typeof(string))
        //        throw new Exception("Missing DataTemplate for String");
        //    else if (interfaces.Contains(typeof(IEnumerable)))
        //        throw new Exception("Missing DataTemplate for Enumerable");
        //    else if (type == typeof(Color))
        //        throw new Exception("Missing DataTemplate for Color");
        //    else if (type == typeof(Guid))
        //        throw new Exception("Missing DataTemplate for Guid");
        //    else if (type == typeof(Enum))
        //        throw new Exception("Missing DataTemplate for Enum");
        //    //if (type == typeof(Utility.WPF.Abstract.Icon))
        //    //    return IconDataTemplate;
        //    else if (type == typeof(bool))
        //        throw new Exception("Missing DataTemplate for Boolean");
        //    else if (typeof(Enum).IsAssignableFrom(type))
        //        throw new Exception("Missing DataTemplate for Enum");
        //    else if (type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(decimal))
        //        throw new Exception("Missing DataTemplate for Number");
        //    else if (interfaces.Contains(typeof(IConvertible)))
        //        throw new Exception("Missing DataTemplate for IConvertible");
        //    return TemplateGenerator.CreateDataTemplate
        //        (() => new TextBlock
        //        {
        //            Text = $"{type.Name} Missing DataTemplate",
        //            Margin = new Thickness(1),
        //            HorizontalAlignment = HorizontalAlignment.Stretch,
        //            VerticalAlignment = VerticalAlignment.Stretch,
        //            Opacity = 0.5
        //        });
        //}

        public static DataTemplate CreateNullTemplate() =>
           TemplateGenerator.CreateDataTemplate(() => new TextBlock
           {
               FontSize = 14,
               HorizontalAlignment = HorizontalAlignment.Stretch,
               VerticalAlignment = VerticalAlignment.Stretch,
               Text = $"item is null"
           });

    }
}