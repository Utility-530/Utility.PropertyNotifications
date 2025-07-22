using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reactive.Subjects;
using Utility.Nodify.Base;

namespace Utility.Nodify.ViewModels
{
    public class MenuViewModel : BaseNodeViewModel, IObservable<(PointF, MenuItemViewModel)>
    {
        private RangeObservableCollection<MenuItemViewModel> items = new RangeObservableCollection<MenuItemViewModel>();
        readonly ReplaySubject<(PointF, MenuItemViewModel)> replaySubject = new();

        public MenuViewModel()
        {
            Items
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
                items = value;
            }
        }

        public void OpenAt(PointF targetLocation)
        {
            Close();
            Location = targetLocation;
            IsVisible = true;
        }

        public void Close()
        {
            IsVisible = false;
        }

        public IDisposable Subscribe(IObserver<(PointF, MenuItemViewModel)> observer)
        {
            return replaySubject.Subscribe(observer);
        }
    }
}
