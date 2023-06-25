using System.Collections.ObjectModel;
using Utility.Models;
using Utility.PropertyTrees.Abstractions;
using Utility.PropertyTrees.Demo.Model;

namespace Utility.PropertyTrees.WPF.Demo
{
    public class HUD_Simulator 
    {

        public GameModel GameModel { get; set; } = new();

        public ServerConnection ServerConnection { get; set; } = new();

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

    public class ServerConnection : INotifyPropertyChanged
    {
        private bool isConnected;
        private ServerRequest serverRequest;

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

        public ServerRequest ServerRequest
        {
            get => serverRequest; 
            private set
            {
                serverRequest = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ServerRequest)));

            }
        }

        public Events Events { get; set; }

        public void Connect()
        {
            ServerRequest = new ServerRequest(IP, Port);
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


    public class RootViewModelsProperty : RootProperty
    {
        static readonly Guid guid = Guid.Parse("aabe5f0b-6024-4913-8017-74475096fc52");

        public RootViewModelsProperty() : base(guid)
        {
            var data = new ViewModels();
            Data = data;
        }
    }

    public class ViewModels
    {
        public string Name { get; set; }
        public ViewModelsCollection Collection { get; set; } = new ();
    }

    public class ViewModelsCollection: ObservableCollection<ViewModel>
    {
        public void MoveUp(ViewModel ViewModel)
        {
            var oldIndex = this.IndexOf(ViewModel);
            Move(oldIndex, oldIndex-1);
        }

        public void MoveDown(ViewModel ViewModel)
        {
            var oldIndex = this.IndexOf(ViewModel);
            Move(oldIndex, oldIndex + 1);
        }
    }

    public class RootMethodsProperty : RootProperty
    {
        static readonly Guid guid = Guid.Parse("ffbe5f0b-6024-4913-8017-74475096fc52");

        public RootMethodsProperty() : base(guid)
        {
            var data = new Wrapper();
            Data = data;
        }
    }

    public class Wrapper
    {
        public Methods Methods { get; set; }
    }

    public class Methods
    {
        public void Foo()
        {
        }    

        public void Bar()
        {
        }
    }
}