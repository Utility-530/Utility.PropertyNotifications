using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Utility.Common;
using Utility.Helpers.Ex;
using Utility.ViewModels.Base;
using Utility.WPF.Demo.Common.Meta;

namespace Utility.WPF.Demo.Common.ViewModels
{
    public class ButtonViewModel : BaseViewModel
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
            Data = (Resolver.Instance.Resolve<Factory>() ?? throw new Exception("df___fsd")).Create<ButtonViewModel>(3).ToObservableCollection();
        }

        public ObservableCollection<ButtonViewModel> Data { get; }
    }
}