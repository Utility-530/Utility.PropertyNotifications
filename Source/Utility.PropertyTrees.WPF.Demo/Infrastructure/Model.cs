using System.Collections.ObjectModel;
using Utility.Models;
using Utility.PropertyTrees.Demo.Model;

namespace Utility.PropertyTrees.WPF.Demo
{
    public class RootModel : INotifyPropertyChanged
    {
        private string jSON;

        public GameModel GameModel { get; set; }

        public Server Server { get; set; }

        public Events Events { get; set; }

        public string JSON
        {
            get => jSON; set
            {
                jSON = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JSON)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class Commands : ReadOnlyCollection<Icon>
    {
        public Commands() : base(new[] { new Icon { Label = "Refresh" } })
        {
        }
    }

    public class Events : ObservableCollection<Event>
    {
    }

    public class Server
    {
        public string IP { get; set; }
        public int Port { get; set; }
    }

    public class Icon
    {
        public string Label { get; set; }
    }
}
