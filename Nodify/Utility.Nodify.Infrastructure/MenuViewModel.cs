using System;
using System.Collections.ObjectModel;
using System.Windows;
using Utility.Nodify.Base;

namespace Utility.Nodify.Core
{
    public class MenuViewModel : BaseNodeViewModel
    {
        private RangeObservableCollection<MenuItemViewModel> items = new RangeObservableCollection<MenuItemViewModel>();

        public event Action<Point, MenuItemViewModel> Selected;

        public MenuViewModel()
        {
            (Items)
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

        public RangeObservableCollection<MenuItemViewModel> Items
        {
            get
            {
                return items;
            }
            set
            {
                foreach (var item in value)
                    item.Selected += selected;
                value
                    .WhenAdded(a =>
                    {
                        a.Selected += selected;
                    });
                this.items = value;
            }
        }

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
