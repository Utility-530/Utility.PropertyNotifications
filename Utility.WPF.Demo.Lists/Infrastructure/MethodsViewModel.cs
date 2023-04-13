using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using Utility.Helpers.Ex;
using Utility.WPF.Demo.Buttons.Infrastructure;
using Utility.WPF.Demo.Common.ViewModels;

namespace Utility.WPF.Demo.Lists.Infrastructure
{
    public class MethodsViewModel
    {
        public MethodsViewModel()
        {
            Data = new ObservableCollection<ButtonViewModel>(
                Utility.Helpers.ReflectionHelper.GetMethods(new Model())
                .Select(a => new ButtonViewModel(a.Item1, ReactiveCommand.Create(() => { _ = a.Item2(); }))));
        }

        public ObservableCollection<ButtonViewModel> Data { get; }
    }
}