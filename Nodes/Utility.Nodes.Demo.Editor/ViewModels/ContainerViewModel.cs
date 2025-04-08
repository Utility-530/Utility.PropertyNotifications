using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.PropertyNotifications;

namespace Utility.Nodes.Demo.Editor
{
    public class ContainerViewModel : NotifyPropertyClass
    {
        public MasterViewModel Master => Locator.Current.GetService<MasterViewModel>();
        public SlaveViewModel Slave => Locator.Current.GetService<SlaveViewModel>();
    }
}
