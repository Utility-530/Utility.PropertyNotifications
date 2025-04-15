using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.WPF.Demo.Data
{
    public partial class Resources
    {
        private static Lazy<Resources> _lazyResources => new(() =>
        {
            var resources = new Resources();
            resources.InitializeComponent();
            return resources;
        });

        public Resources()
        {
            //InitializeComponent();
        }

        public static Resources Instance => _lazyResources.Value;

    }
}
