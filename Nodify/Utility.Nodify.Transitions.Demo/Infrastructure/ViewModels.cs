using System.Windows.Input;
using Utility.Commands;
using Utility.Models.Diagrams;
using Utility.Simulation;
using Utility.Helpers;
using Splat;
using Utility.ServiceLocation;
using Utility.Interfaces.Exs;
using Utility.Nodify.Models;

namespace Utility.Nodify.Transitions.Demo.Infrastructure
{
    public class CommandsViewModel
    {
        public int GridRow => 0;
        public ICommand AddCommand { get; } = new Command(() => 
        Enumerable
        .Repeat<PlaybackAction>(new(null, null, null) { Name = "a" }, 5)
        .ForEach(a => Locator.Current.GetService<IPlaybackEngine>()
        .OnNext(a)));
    }



    public class MainViewModel
    {
        object[] collection = [Globals.Resolver.Resolve<DiagramViewModel>(), Globals.Resolver.Resolve<MasterPlayViewModel>()];

        public object[] Collection => collection;
    }
}
