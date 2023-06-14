//using System;
//using System.Drawing;
//using System.Windows;
//using System.Windows.Controls;
//using Utility.WPF.Helpers;

//namespace Utility.WPF.DataTemplateSelectors
//{
//    public class GeneralDataTemplateSelector : System.Windows.Controls.DataTemplateSelector
//    {
//        public DataTemplate? DefaultDataTemplate { get; set; }
//        public DataTemplate? BooleanDataTemplate { get; set; }
//        public DataTemplate? StringDataTemplate { get; set; }
//        public DataTemplate? EnumDataTemplate { get; set; }
//        public DataTemplate? NumberDataTemplate { get; set; }
//        public DataTemplate? IconDataTemplate { get; set; }
//        public DataTemplate? ColorTemplate { get; set; }

//        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
//        {
//            if (item == null)
//                return DefaultDataTemplate;

//            var type = item.GetType();

//            //DataTemplate myDataTemplate = ((System.Windows.Controls.ContentPresenter)container).ContentTemplate;

//            if (new DataTemplateKey(type) is { } key &&
//                (container as FrameworkElement)?.TryFindResource(key) is DataTemplate dt)
//                return dt;

//            //var interfaces = type.GetInterfaces();
//            //if (interfaces.Contains(typeof(IConvertible)))
//            //    return IConvertibleTemplate;
//            //else if (interfaces.SingleOrDefault(a => a.Name == "IDictionary`2") != null || interfaces.Contains(typeof(IDictionary)))
//            //    return DictionaryDataTemplate;
//            //else if (interfaces.Contains(typeof(IEnumerable)))
//            //    return EnumerableDataTemplate;
//            if (type == typeof(Color))
//                return ColorTemplate;
//            //if (type == typeof(Utility.WPF.Abstract.Icon))
//            //    return IconDataTemplate;
//            if (type == typeof(bool))
//                return BooleanDataTemplate;
//            if (typeof(Enum).IsAssignableFrom(type))
//                return EnumDataTemplate;
//            if (type == typeof(string))
//                return StringDataTemplate;
//            if (type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(decimal))
//                return NumberDataTemplate;
//            return DefaultDataTemplate ??= TemplateGenerator.CreateDataTemplate
//            (
//                () => new TextBlock
//                {
//                    Text = "(x)",
//                    Margin = new Thickness(1),
//                    VerticalAlignment = VerticalAlignment.Center,
//                    Opacity = 0.5
//                });
//        }

//        public static GeneralDataTemplateSelector Instance => new();
//    }
//}