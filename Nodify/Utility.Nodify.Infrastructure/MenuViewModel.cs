using System;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using System.Windows;
using Utility.Nodify.Base;

namespace Utility.Nodify.Core
{
    public class MenuViewModel : BaseNodeViewModel, IObservable<(Point, MenuItemViewModel)>
    {
        private RangeObservableCollection<MenuItemViewModel> items = new RangeObservableCollection<MenuItemViewModel>();
        readonly ReplaySubject<(Point, MenuItemViewModel)> replaySubject = new();

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
            replaySubject.OnNext((Location, obj));
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

        public IDisposable Subscribe(IObserver<(Point, MenuItemViewModel)> observer)
        {
            return replaySubject.Subscribe(observer);
        }
    }
}
