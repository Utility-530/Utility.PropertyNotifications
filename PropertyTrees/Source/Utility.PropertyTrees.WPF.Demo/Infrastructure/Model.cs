using System.Collections.ObjectModel;
using System.Reactive;
using Utility.Models;
using Utility.PropertyTrees.Demo.Model;
using VM = Utility.PropertyTrees.Services.ViewModel;
namespace Utility.PropertyTrees.WPF.Demo
{

    public class RootModel : BaseViewModel
    {
        DateTime lastRefresh;

        public void Refresh()
        {
            LastRefresh = DateTime.Now;
        }

        public DateTime LastRefresh
        {
            get => lastRefresh; 
            set
            {           
                Set(ref lastRefresh, value);
            }
        }

        public HUD_Simulator HUD_Simulator { get; set; } = new();

        public ViewModels ViewModels { get; set; } = new();
    }




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

    public class ViewModels : BaseViewModel
    {
        //private VM @default = new();
        private ObservableCollection<VM> collection = new();
        private Key key;
        private string name;
        private Guid guid;
        private string type;

        public void AddByName()
        {
            Collection.Add(new VM() { Name = Name });
        }

        public void AddByKey()
        {
            Collection.Add(new VM() { ParentGuid = Guid });
        }

        public void AddByType()
        {
            Collection.Add(new VM() { Type = System.Type.GetType(Type) });
        }

        public void Update()
        {
            Key = new Key(Guid, Name, System.Type.GetType(Type));
        }

        public ObservableCollection<VM> Collection { get => collection; set => collection = value; }

        public string Name
        {
            get => name; set
            {
               Set(ref name, value);
            }
        }

        public Guid Guid
        {
            get => guid; set
            {
                Set(ref guid, value);
            }
        }

        public string Type
        {
            get => type; set
            {
                Set(ref type, value);
            }
        }

        public Key Key
        {
            get => key;
            private set
            {
                this.Set(ref key, value);
            }
        }

        //public VM Default
        //{
        //    get => @default; set
        //    {
        //        @default = value;
        //    }
        //}
    }


    public class ViewModelsCollection : ObservableCollection<VM>
    {
        public void MoveUp(VM ViewModel)
        {
            var oldIndex = this.IndexOf(ViewModel);
            Move(oldIndex, oldIndex - 1);
        }

        public void MoveDown(VM ViewModel)
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