using Newtonsoft.Json;
using Splat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Interfaces.Exs;
using Utility.Models.Trees;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Repos;
using Utility.Trees.Extensions;
using Utility.WPF.Factorys;
using Utility.WPF.Templates;
using Utility.Helpers;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Utility.Nodes.Demo.Queries
{

    internal class CustomDataTemplateSelector : DataTemplateSelector
    {
        private Lazy<ILiteRepository> repo = new(() => Locator.Current.GetService<ILiteRepository>());

        Dictionary<object, DataTemplate> dataTemplates = [];

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var contentPresenter = new ContentPresenter() { Content = item, ContentTemplate = base.SelectTemplate(item, container) };
            List<FilterEntity> filters = [];


            SelectorController.Instance.WithChangesTo(a => a.Value)
                .Subscribe(a =>
            {
                Locator.Current.GetService<IMainViewModel>().Save();

                filters.Clear();

                _ = repo.Value
                    .FindBy(nameof(FilterEntity.GroupKey), "CustomDataTemplate")
                    .ToObservable()
                    .SelectMany()
                    .Cast<FilterEntity>()
                    .Subscribe(a =>
                    {
                        filters.Add(a);
                    }, () =>
                    {
                        foreach (var filter in filters)
                        {
                            if (filter.Body != null)
                            {
                                var node = JsonConvert.DeserializeObject<Node>(filter.Body);
                                node.SelfAndDescendants().ForEach(a =>
                                {
                                    if (a.Data is ISetNode setNode)
                                        setNode.SetNode((INode)a);
                                });


                                if (node.Data is AndOrModel { } andOrModel)
                                {
                                    if (andOrModel.Get(item))
                                    {
                                        var dataTemplate = Application.Current.TryFindResource(filter.Key) as DataTemplate;
                                        dataTemplates[item] = dataTemplate;
                                        contentPresenter.ContentTemplate = dataTemplate;
                                    }
                                    else
                                    {
                                        contentPresenter.ContentTemplate = base.SelectTemplate(item, container);
                                    }
                                }
                            }
                        }
                    });
            });

            var template = TemplateGenerator.CreateHierarcialDataTemplate(() =>
            {
                return contentPresenter;
            }, "Cars");

            return template;
        }

        public static CustomDataTemplateSelector Instance { get; } = new();
    }

    public class DataTemplateValueConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            return CustomDataTemplateSelector.Instance.SelectTemplate(value[1], null);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static DataTemplateValueConverter Instance { get; } = new();

    }

    public class StyleValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return CustomStyleSelector.Instance.SelectStyle(value, null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static StyleValueConverter Instance { get; } = new();

    }


    public class SelectorController : NotifyPropertyClass
    {


        public int Value
        {
            get => value; set
            {

                this.value = value;
                RaisePropertyChanged();
            }
        }



        public static SelectorController Instance = new SelectorController();

        private int value;
    }
}
