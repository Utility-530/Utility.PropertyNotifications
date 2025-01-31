using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Nodes.Filters;
using Utility.Trees.Abstractions;
using Utility.ViewModels;

namespace Utility.Trees.Demo.Filters.Infrastructure
{
    internal class MainViewModel : NotifyPropertyChangedBase
    {
        private IReadOnlyTree[] filters;
        Lazy<INodeSource> source = new(() => Locator.Current.GetService<INodeSource>());

        public IReadOnlyTree[] Filters
        {
            get
            {
                if (filters == null)
                    source.Value.Single(nameof(Factory.BuildFiltersRoot))
                        .Subscribe(a => 
                        { 
                            filters = [a]; 
                            RaisePropertyChanged(nameof(Filters)); 
                        });
                return filters;
            }
        }
    }
}
