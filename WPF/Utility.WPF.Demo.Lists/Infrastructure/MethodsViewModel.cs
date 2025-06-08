using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Utility.WPF.Demo.Common.ViewModels;
using Utility.Helpers.Reflection;
using _Model = Utility.WPF.Demo.Buttons.Infrastructure.Model;

namespace Utility.WPF.Demo.Lists.Infrastructure
{
    public class MethodsViewModel
    {
        public MethodsViewModel()
        {
            var instance = new _Model();
            Data = new ObservableCollection<ButtonViewModel>(
            
                TypeHelper.GetInstanceMethods(instance.GetType())
                .Select(m => (m.GetDescription(), new Func<object?>(() => m.Invoke(instance, Array.Empty<object>()))))
                .Select(a => new ButtonViewModel(a.Item1, ReactiveCommand.Create(() => { _ = a.Item2(); }))));
        }
        public ObservableCollection<ButtonViewModel> Data { get; }
    }
}