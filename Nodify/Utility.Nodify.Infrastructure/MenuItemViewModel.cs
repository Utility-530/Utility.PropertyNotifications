using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Utility.Commands;
using Utility.Nodify.Base.Abstractions;
using Utility.Nodify.Operations.Infrastructure;
using Utility.PropertyNotifications;

namespace Utility.Nodify.ViewModels
{


    public class MenuItemViewModel : NotifyPropertyClass, IMenuItemViewModel
    {
        private readonly ObservableCollection<MenuItemViewModel> children = new();
        public event Action<IMenuItemViewModel> Selected;

        public MenuItemViewModel()
        {
            Command = new Command(() =>
            {


                if (Content is MenuItem menuItem)
                {
                    if (menuItem.Children is { } childs)
                    {
                        bool flag = false;
                        foreach (var item in childs)
                        {
                            flag = true;
                            var menuItemViewModel = new MenuItemViewModel() { Content = item };
                            menuItemViewModel.Selected += a => Selected?.Invoke(a);
                            children.Add(menuItemViewModel);
                        }
                        if (flag)
                        {
                            IsExpanded = flag;
                            RaisePropertyChanged(nameof(IsExpanded));
                        }
                    }
                }
                if (IsExpanded == false)
                    Selected?.Invoke(this);
            });
        }

        public Guid Guid { get; set; }

        public object? Content { get; set; }

        public ICommand Command { get; set; }

        public bool IsExpanded { get; set; }

        public IEnumerable Children => children;

    }
}
