using Splat;
using Utility.PropertyNotifications;
using Utility.ServiceLocation;

namespace Utility.Simulation
{
    public class MasterPlayViewModel
    {
        public int GridRow => 1;
        public IEnumerable<NotifyPropertyClass> Collection => [
            Globals.Resolver.Resolve<PlayBackViewModel>(), 
            Globals.Resolver.Resolve<HistoryViewModel>(),
            Globals.Resolver.Resolve<ExceptionsViewModel>(),
        ];
    }
}
