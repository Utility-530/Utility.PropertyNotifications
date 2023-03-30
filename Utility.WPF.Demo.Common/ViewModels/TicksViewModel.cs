using Endless;
using Microsoft.Xaml.Behaviors.Core;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Utility.Common;
using Utility.Helpers.Ex;
using Utility.WPF.Demo.Common.Meta;

namespace Utility.WPF.Demo.Common.ViewModels
{
    public class TicksViewModel
    {
        private ICommand changeCommand;

        public string Header { get; } = "Ticks_ViewModel";

        public ObservableCollection<TickViewModel> Collection { get; } = new ObservableCollection<TickViewModel>(Resolver.Instance.Resolve<Factory>().Create<TickViewModel>(3).ToObservableCollection());

        public System.Collections.IEnumerator NewItem { get => 0.Repeat().Select(a =>Resolver.Instance.Resolve<Factory>().Create<TickViewModel>()).GetEnumerator(); }

        public ICommand ChangeCommand => changeCommand ??= new ActionCommand(Change);

        private void Change()
        {
        }
    }
}