using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Utility.WPF.Factorys;
using System.Windows.Shapes;
using System.Windows.Media;
using Utility.Descriptors.Repositorys;
using Utility.Keys;
using Newtonsoft.Json;
using Utility.WPF.Controls.Trees;
using System.Reflection;
using Utility.WPF.ResourceDictionarys;
using NetFabric.Hyperlinq;
using System.Linq;

namespace Utility.Trees.Demo.MVVM
{
    public class View
    {
        public class ItemsPanelConverter : System.Windows.Data.IValueConverter
        {
            public static ItemsPanelConverter Instance { get; } = new();

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is Tree { Key: string str } && (GuidKey)str is { Value: { } guid } && guid != default)
                {
                    var _guid = TreeRepository.Instance.Find(guid, nameof(ViewModel.ItemsPanelTemplateKey));
                    if (_guid.Wait(500))
                    {
                        var itemsPanelKey = TreeRepository.Instance.Get(_guid.Result);
                        if (itemsPanelKey != null)
                            if (App.Current?.TryFindResource(itemsPanelKey) is ItemsPanelTemplate template)
                                return template;
                    }
                    else
                    {
                        throw new TimeoutException(" dssdfds");
                    }
                }
                return ItemsPanelFactory.Template(default, default, Orientation.Vertical, Enums.Arrangement.Stacked);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public class DataTemplateSelector : System.Windows.Controls.DataTemplateSelector
        {
            public static DataTemplateSelector Instance { get; } = new();


            private DataTemplateSelector()
            {
                Task.Run(() => TreeRepository.Instance.Initialise());

            }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                //if (item?.GetType() is Type type && new DataTemplateKey(type) is var key &&  App.Current?.TryFindResource(key) is DataTemplate dataTemplate)
                //    return dataTemplate;        
                TreeRepository.Instance.Initialise().Wait(500);
                if (item is Tree { Key: string str } && (GuidKey)str is { Value: { } guid } && guid != default)
                {
                    var _guid = TreeRepository.Instance.Find(guid, nameof(ViewModel.DataTemplateKey));
                    if(_guid.Wait(500)==false)
                        throw new TimeoutException(" d222 ssdfds");

                    var dataTemplateKey = TreeRepository.Instance.Get(_guid.Result);

                    if (dataTemplateKey != null)
                    {
                        var x = JsonConvert.DeserializeObject<AssemblyTreeKey>(dataTemplateKey.Value.Value.ToString());

                        var assembly = Assembly.Load(x.Assembly);
                        var res = assembly.SelectResourceDictionaries().Where(a=>a.Name == x.ResourceDictionary).Single();
         
                        if (res.ResourceDictionary.FindResource(x.Element) is DataTemplate template)
                            return template;
                    }
                }
                return TemplateGenerator.CreateDataTemplate(() => new Ellipse { Fill = Brushes.Red, Height = 20, Width = 20 });
            }

        }
        public class StyleSelector : System.Windows.Controls.StyleSelector
        {
            public override Style SelectStyle(object item, DependencyObject container)
            {
                TreeRepository.Instance.Initialise().Wait(500);

                if (item is Tree { Key: string str } && (GuidKey)str is { Value: { } guid } && guid != default)
                {
                    var _guid = TreeRepository.Instance.Find(guid, nameof(ViewModel.StyleKey)).GetAwaiter().GetResult();
                    var styleKey = TreeRepository.Instance.Get(_guid);
                    if (styleKey != null)
                        if (App.Current?.TryFindResource(styleKey) is Style style)
                            return style;
                }
                return base.SelectStyle(item, container);
            }

            //public ResourceDictionary NewTemplates => new()
            //{
            //    Source = new Uri($"/{typeof(CustomStyleSelector).Assembly.GetName().Name};component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            //};

            public static StyleSelector Instance { get; } = new();
        }
    }
}
