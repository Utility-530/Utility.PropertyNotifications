//using System;
//using System.Collections;
//using System.Drawing;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Markup;
//using Utility.WPF.Adorners;
//using Utility.WPF.Controls.Adorners;
//using Utility.WPF.Helpers;
//using Utility.WPF.Templates;

//namespace Utility.WPF.Controls.Templates
//{

//    public partial class Templates2 : ResourceDictionary
//    {
//        public void Click(object sender, RoutedEventArgs args)
//        {

//        }
//    }

//    public class Custom2DataTemplateSelector : CustomDataTemplateSelector
//    {
//        public Custom2DataTemplateSelector()
//        {
//            //            DefaultDataTemplate ??= TemplateGenerator.CreateDataTemplate
//            //(() => new TextBlock
//            //{
//            //    Text = $"Missing DataTemplate for type, {item.GetType().Name}",
//            //    Margin = new Thickness(1),
//            //    HorizontalAlignment = HorizontalAlignment.Stretch,
//            //    VerticalAlignment = VerticalAlignment.Stretch,
//            //    Opacity = 0.5
//            //});
//        }

//        public override DataTemplate SelectTemplate(object item, DependencyObject container)
//        {
//            var template = Select_Template(item, container);

//            //if (template.LoadContent() is FrameworkElement element)
//            //{
//            //    element.ApplyTemplate();
//            //    SettingsAdorner.AddTo(element);
//            //}
//            return template;
//        }

//        public ResourceDictionary Templates
//        {
//            get
//            {
//                return new ResourceDictionary
//                {
//                    Source = new Uri("/Utility.WPF.Controls.Templates;component/Templates.xaml", UriKind.RelativeOrAbsolute)
//                };
//            }
//        }

//        public new DataTemplate Select_Template(object item, DependencyObject container)
//        {
//            Templates.BeginInit();

//            if (item == null)
//                return NullDataTemplate ??= NullTemplate();

//            var type = item.GetType();
//            while (type != typeof(object) && new DataTemplateKey(type) is var key)
//            {
//                if ((container as FrameworkElement)?.TryFindResource(key) is not DataTemplate dataTemplate)
//                    type = type?.BaseType;
//                else
//                    return dataTemplate;
//            }

//            var interfaces = type.GetInterfaces();

//            if (interfaces.Contains(typeof(IConvertible)))
//                return IConvertibleTemplate ??= base.Templates["IConvertable"] as DataTemplate ?? throw new Exception("Missing DataTemplate for IConvertible");
//            else if (interfaces.Any(a => a.Name == "IDictionary`2") || interfaces.Contains(typeof(IDictionary)))
//                return DictionaryDataTemplate ??= base.Templates["Dictionary"] as DataTemplate ?? throw new Exception("Missing DataTemplate for Dictionary");
//            else if (interfaces.Contains(typeof(IEnumerable)))
//                return EnumerableDataTemplate ??= base.Templates["Enumerable"] as DataTemplate ?? throw new Exception("Missing DataTemplate for Enumerable");
//            if (type == typeof(Color))
//                return ColorTemplate ??= base.Templates["Color"] as DataTemplate ?? throw new Exception("Missing DataTemplate for Color");
//            //if (type == typeof(Utility.WPF.Abstract.Icon))
//            //    return IconDataTemplate;
//            if (type == typeof(bool))
//                return BooleanDataTemplate ??= base.Templates["Boolean"] as DataTemplate ?? throw new Exception("Missing DataTemplate for Boolean");
//            if (typeof(Enum).IsAssignableFrom(type))
//                return EnumDataTemplate ??= base.Templates["Enum"] as DataTemplate ?? throw new Exception("Missing DataTemplate for Enum");
//            if (type == typeof(string))
//                return StringDataTemplate ??= base.Templates["String"] as DataTemplate ?? throw new Exception("Missing DataTemplate for String");
//            if (type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(decimal))
//                return NumberDataTemplate ??= base.Templates["Number"] as DataTemplate ?? throw new Exception("Missing DataTemplate for Number");

//            return DefaultDataTemplate ??= Templates["Default"] as DataTemplate ?? TemplateGenerator.CreateDataTemplate
//                (() =>
//                {
//                    var textBlock = new TextBlock
//                    {
//                        Text = @$"Missing DataTemplate for type,{Environment.NewLine}{item.GetType().Name}",
//                        Margin = new Thickness(1),
//                        HorizontalAlignment = HorizontalAlignment.Stretch,
//                        VerticalAlignment = VerticalAlignment.Stretch,
//                        Opacity = 0.5,
//                        FontSize = 8,
//                        Foreground = System.Windows.Media.Brushes.DarkRed,
//                        Background = System.Windows.Media.Brushes.Blue
//                    };
//                    //textBlock.Loaded += TextBlock_Loaded;

//                    Footer.SetText(textBlock, "Sdgfcsdfd");
//                    //SettingsAdorner.AddTo(textBlock as TextBlock);
//                    return new AdornerDecorator { Child = textBlock };
//                });
//        }

//        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
//        {
//            SettingsAdorner.AddTo(sender as TextBlock);
//        }

//        public static Custom2DataTemplateSelector Instance2 => new();
//    }
//}
