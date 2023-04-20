using CoffeeFlow.ViewModel;
using System.Windows;
using System.Windows.Threading;

namespace CoffeeFlow.WPF.Infrastructure
{
    public class Globals
    {
        private Globals()
        {

        }

        public Window MainWindow { get; set; }

        public Dispatcher Dispatcher { get; set; }

        public MainViewModel Main { get; set; }

        public NetworkViewModel Network { get; set; }

        public static Globals Instance { get; } = new();

    }
}