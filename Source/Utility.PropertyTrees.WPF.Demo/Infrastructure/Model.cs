using System.Collections.ObjectModel;
using Utility.Models;
using Utility.PropertyTrees.Abstractions;
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

    public class Server : INotifyPropertyChanged
    {
        private bool isConnected;

        public string IP { get; set; }
        public int Port { get; set; }

        public bool IsConnected
        {
            get => isConnected; set
            {
                isConnected = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsConnected)));

            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

    }

    public class AppItems : List<AppItem>
    {
    }

    public class AppItem
    {
    }
    public class BooleanAppItem : AppItem
    {
        public bool Value { get; set; }
    }

    public class Icon
    {
        public string Label { get; set; }
    }


    public class RootModel2Property : RootProperty
    {
        static readonly Guid guid = Guid.Parse("aabe5f0b-6024-4913-8017-74475096fc52");

        public RootModel2Property() : base(guid)
        {
            var data = new ViewModels();
            Data = data;
        }
    }

    public class ViewModels
    {
        public ObservableCollection<ViewModel> Collection { get; set; } = new ObservableCollection<ViewModel>();
    }
}