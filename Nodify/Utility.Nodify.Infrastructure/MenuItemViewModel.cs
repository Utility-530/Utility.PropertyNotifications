using System;
using System.Windows.Input;
using Utility.Commands;

namespace Utility.Nodify.ViewModels
{


    public class MenuItemViewModel
    {
        public event Action<MenuItemViewModel> Selected;

        public MenuItemViewModel()
        {
            Command = new Command(() =>
            {
                Selected?.Invoke(this);
            });
        }

        public Guid Guid { get; set; }

        public string? Content { get; set; }

        public ICommand Command { get; set; }

    }
}
