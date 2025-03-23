using Jonnidip;
using Newtonsoft.Json;
using Splat;
using System.Windows.Input;
using Utility.Commands;
using Utility.Helpers;
using Utility.Interfaces.Exs;
using Utility.Nodes;
using Utility.Nodes.Filters;
using Utility.Trees.Abstractions;
using Utility.ViewModels;

namespace Utility.Trees.Demo.Filters.Infrastructure
{
    internal class MainViewModel : NotifyPropertyChangedBase
    {
        private Node[] filters;
        Lazy<INodeSource> source = new(() => Locator.Current.GetService<INodeSource>());
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Converters = 
                        [
                            new StringTypeEnumConverter(),
                            new NodeConverter()
                        ]
        };

        public MainViewModel()
        {
            SaveCommand = new Command(() =>
            {

            });
        }

        public ICommand SaveCommand { get; }
        public ICommand FinishEdit { get; }

        public IReadOnlyTree[] Nodes
        {
            get
            {
                if (filters == null)
                    source.Value.Single(nameof(Factory.BuildCollectionRoot))
                        .Subscribe(a =>
                        {
                            filters = [(Node)a];
                            RaisePropertyChanged(nameof(Nodes));
                        });
                return filters;
            }
        }
    }
}
