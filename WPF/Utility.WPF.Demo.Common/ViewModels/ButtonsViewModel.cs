using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Utility.Helpers.Ex;
using Utility.PropertyNotifications;
using Utility.ServiceLocation;
using Utility.WPF.Demo.Common.Meta;
using Utility.Helpers.Generic;

namespace Utility.WPF.Demo.Common.ViewModels
{
    public class ButtonViewModel : NotifyPropertyClass
    {
        public ButtonViewModel(string header, ICommand command)
        {
            Header = header;
            Command = command;
        }

#pragma warning disable CS8618

        public ButtonViewModel()
#pragma warning restore CS8618
        {
        }

        public ICommand Command { get; set; }

        public bool IsRefreshable { get; init; }

        public string Header { get; init; }
    }

    public class ButtonsViewModel
    {
        public ButtonsViewModel()
        {
            Data = (Globals.Resolver.Resolve<Factory>() ?? throw new Exception("df___fsd")).Create<ButtonViewModel>(3).ToObservableCollection();
        }

        public ObservableCollection<ButtonViewModel> Data { get; }
    }
}