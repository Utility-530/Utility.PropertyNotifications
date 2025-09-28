using Simple.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Utility.Commands;
using Utility.Networks.WPF.Server.Services;
using Utility.PropertyNotifications;

namespace Utility.Networks.WPF.Server.ViewModels
{

    public class MainWindowViewModel : NotifyPropertyClass
    {
        //public string ExternalAddress
        //{
        //    get => Model.Instance.String<ExternalAddressChange>();
        //    set => Model.Instance.AddChange(new ExternalAddressChange(value));
        //}
        public Networks.Server Server
        {
            get => (Networks.Server)Model.Instance.Object<ServerChange>();
            set => Model.Instance.AddChange(new ServerChange(value));
        }

        public string Port
        {
            get => Model.Instance.String<PortChange>();
            set => Model.Instance.AddChange(new PortChange(value));
        }

        public string Status
        {
            get => Model.Instance.String<StatusChange>();
            set => Model.Instance.AddChange(new StatusChange(value));
        }

        public ObservableCollection<object> Outputs { get; } = new();

        public ICommand RunCommand { get; set; }
        public ICommand StopCommand { get; set; }


        public MainWindowViewModel()
        {
            RunCommand = new Command(() => Model.Instance.AddChange(new RunChange()), ()=> !(Server?.IsRunning?? false));
            StopCommand = new Command(() => Model.Instance.AddChange(new StopChange()), ()=>(Server?.IsRunning ?? false));

            Model.Instance.Changes.CollectionChanged += (s, e) =>
            {
                foreach (var item in e.NewItems)
                {
                    switch (item)
                    {

                        case PortChange:
                            RaisePropertyChanged(nameof(Port));
                            break;
                        case StatusChange:
                            RaisePropertyChanged(nameof(Status));
                            break;
                        case ServerChange:
                            RaisePropertyChanged(nameof(Server));
                            break;
                    }
                }
            };
        }


    }
}
