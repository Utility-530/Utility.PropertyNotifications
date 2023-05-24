using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Utility.Nodify.Core
{
    public class MenuViewModel : BaseNodeViewModel
    {
        public event Action<Point, MenuItemViewModel> Selected;

        public MenuViewModel()
        {
            (Items = new RangeObservableCollection<MenuItemViewModel>())
                .WhenAdded(a =>
            {
                a.Selected += selected;
            });
        }

        private void selected(MenuItemViewModel obj)
        {
            Selected?.Invoke(Location, obj);
            Close();
        }

        public RangeObservableCollection<MenuItemViewModel> Items { get; }


        public void OpenAt(Point targetLocation)
        {
            Close();
            Location = targetLocation;
            IsVisible = true;
        }

        public void Close()
        {
            IsVisible = false;
        }
    }
}
