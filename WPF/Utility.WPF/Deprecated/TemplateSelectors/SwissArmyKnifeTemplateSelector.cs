//using ReactiveUI;
//using Splat;
//using System;
//using System.Collections;
//using System.Linq;
//using System.Windows;
//using System.Windows.Media;

//namespace Utility.WPF.DataTemplateSelectors
//{
//    public class SwissArmyKnifeTemplateSelector : System.Windows.Controls.DataTemplateSelector
//    {
//        public DataTemplate DefaultDataTemplate { get; set; }
//        public DataTemplate EnumerableDataTemplate { get; set; }
//        public DataTemplate ContentPresenterTemplate { get; set; }
//        public DataTemplate DictionaryDataTemplate { get; set; }
//        public DataTemplate IConvertibleTemplate { get; set; }
//        public DataTemplate ViewModelHostViewTemplate { get; set; }
//        public DataTemplate ColorTemplate { get; set; }

//        public override DataTemplate SelectTemplate(object item, DependencyObject container)
//        {
//            if (item == null)
//                return DefaultDataTemplate;

//            var type = item.GetType();

//            //DataTemplate myDataTemplate = ((System.Windows.Controls.ContentPresenter)container).ContentTemplate;

//            var dataTemplateKey = new DataTemplateKey(type);
//            //var x = Application.Current.Resources[dataTemplateKey];
//            object dataTemplate = null;
//            if (dataTemplateKey != null)
//                dataTemplate = (container as FrameworkElement).TryFindResource(dataTemplateKey);

//            if (dataTemplate != null)
//                return ContentPresenterTemplate;

//            var interfaces = type.GetInterfaces();
//            if (interfaces.Contains(typeof(IConvertible)))
//                return IConvertibleTemplate;
//            else if (interfaces.SingleOrDefault(a => a.Name == "IDictionary`2") != null || interfaces.Contains(typeof(IDictionary)))
//                return DictionaryDataTemplate;
//            else if (interfaces.Contains(typeof(IEnumerable)))
//                return EnumerableDataTemplate;
//            else if (type == typeof(Color))
//                return ColorTemplate;
//            else if (TryResolveView(item).success)
//                return ViewModelHostViewTemplate;
//            else
//                return DefaultDataTemplate;

//            static (bool success, object obj) TryResolveView(object item)
//            {
//                try
//                {
//                    return (true, Locator.Current.GetService<IViewLocator>()?.ResolveView(item));
//                }
//                catch
//                {
//                    return (false, null);
//                }
//            }
//        }
//    }
//}