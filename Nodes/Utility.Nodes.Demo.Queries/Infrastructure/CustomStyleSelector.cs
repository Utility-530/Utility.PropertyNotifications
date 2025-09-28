using Newtonsoft.Json;
using Splat;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.Interfaces.Exs;
using Utility.Models.Trees;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Repos;
using Utility.Trees.Extensions;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Interfaces.NonGeneric.Data;
using Utility.Extensions;

namespace Utility.Nodes.Demo.Queries
{
    public class CustomStyleSelector : StyleSelector
    {
        private Lazy<ILiteRepository> repo = new(() => Locator.Current.GetService<ILiteRepository>());

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var baseStyle = base.SelectStyle(item, container);
            if (baseStyle == null)
            {
                baseStyle = Application.Current.Resources["CustomTreeViewItem"] as Style;
                //baseStyle = Application.Current.Resources["CustomTreeViewItem"] as Style;
            }


            if (item is IKey key)
            {
                (container as Control).SetResourceReference(Control.TemplateProperty, key.Key());
                Application.Current.Resources[key.Key()] = baseStyle.Setters.OfType<Setter>().Single(a => a.Property == Control.TemplateProperty).Value;

            }
            else if (item is IId id)
            {
                (container as Control).SetResourceReference(Control.TemplateProperty, id.Id);
                Application.Current.Resources[id.Id] = baseStyle.Setters.OfType<Setter>().Single(a => a.Property == Control.TemplateProperty).Value;

                List<FilterEntity> filters = [];

                SelectorController.Instance.WithChangesTo(a => a.Value)
                    .Subscribe(a =>
                    {
                        Locator.Current.GetService<IMainViewModel>().Save();

                        filters.Clear();
                        _ = repo.Value
                            .FindBy(nameof(FilterEntity.GroupKey), "CustomControlTemplate")
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
                                        var node = JsonConvert.DeserializeObject<NodeViewModel>(filter.Body);

                                        if (node is AndOrModel { } andOrModel)
                                        {
                                            if (andOrModel.Evaluate(item))
                                            {
                                                var template = Application.Current.TryFindResource(filter.Key) as ControlTemplate;
                                                Application.Current.Resources[id.Id] = template;
                                            }
                                            else
                                            {

                                            }
                                        }
                                    }
                                }
                            });
                    });
            }
         

            return baseStyle;
        }

        public static CustomStyleSelector Instance { get; } = new();

    }
}
