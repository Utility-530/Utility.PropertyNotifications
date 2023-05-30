using System.Windows.Input;
using Utility.Models;
using Utility.PropertyTrees.Demo.Model;

namespace Utility.PropertyTrees.WPF.Demo
{
    public class ModelViewModel : BaseViewModel
    {
        private bool isConnected;

        public Model Model { get; set; }
        public Server Server { get; set; }

        public ICommand SendLeaderboard { get; set; }
        public ICommand SendPrizewheel { get; set; }
        public ICommand SendScreensaver { get; set; }
        public ICommand Connect { get; set; }
        public bool IsConnected { get => isConnected; set => Set(ref isConnected, value); }

        public void Refresh()
        {
            this.OnPropertyChanged(nameof(Connect));
        }
    }
}