using Jonnidip;
using Newtonsoft.Json;
using System.Windows.Input;
using Utility.Commands;
using Utility.Nodes;
using Utility.Nodes.Filters;
using Utility.Nodes.WPF;
using Utility.Trees.Abstractions;

namespace Utility.Trees.Demo.Filters.Infrastructure
{
    internal class MainViewModel : ViewModel
    {
        private Node[] filters;
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Converters = 
                        [
                            new StringTypeEnumConverter(),
                            new Utility.Nodes.NodeConverter()
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
                    Subscribe(nameof(NodeMethodFactory.BuildCollectionRoot),
                        a =>
                        {
                            filters = [(Node)a];
                        });
                return filters;
            }
        }
    }
}
