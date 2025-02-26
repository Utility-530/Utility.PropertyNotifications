using Bogus.Bson;
using Jonnidip;
using Newtonsoft.Json;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UnitsNet;
using Utility.Commands;
using Utility.Conversions.Json.Newtonsoft;
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
            Converters = [
                        new StringTypeEnumConverter(),
                        new NodeConverter(),

                    ]
        };

        public MainViewModel()
        {
            var dir = System.IO.Directory.CreateDirectory("../../../Data");
            var file = DirectoryHelper.And(dir, "data.json");

            SaveCommand = new Command(() =>
            {
                var json = JsonConvert.SerializeObject(filters[0], Formatting.Indented, settings);
                file.OverWrite(json);
            });

            if (file.Exists)
            {
                var json = file.Read();
                filters = [JsonConvert.DeserializeObject<Node>(json, settings)];
            }

        }

        public ICommand SaveCommand { get; }

        public IReadOnlyTree[] Filters
        {
            get
            {
                if (filters == null)
                    source.Value.Single(nameof(Factory.BuildAndOrRoot))
                        .Subscribe(a =>
                        {
                            source.Value.Add(a);
                            filters = [(Node)a];
                            RaisePropertyChanged(nameof(Filters));
                        });
                return filters;
            }
        }
    }
}
