using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reactive.Subjects;
using Utility.Nodify.Base;
using Utility.Nodify.Base.Abstractions;
using Utility.Nodify.Models;

namespace Utility.Nodify.ViewModels
{
    public class MenuViewModel : NodeViewModel, IMenuViewModel
    {
        private RangeObservableCollection<IMenuItemViewModel> items = new RangeObservableCollection<IMenuItemViewModel>();
        readonly ReplaySubject<(PointF, IMenuItemViewModel)> replaySubject = new();

        public MenuViewModel()
        {
            items
                .WhenAdded(a =>
            {
                a.Selected += selected;
            });
        }

        private void selected(IMenuItemViewModel obj)
        {
            replaySubject.OnNext((Location, obj));
            Close();
        }

        public IList<IMenuItemViewModel> Items
        {
            get
            {
                return items;
            }
            set
            {
                foreach (var item in value)
                    item.Selected += selected;

                items.AddRange(value);
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

        public IDisposable Subscribe(IObserver<(PointF, IMenuItemViewModel)> observer)
        {
            return replaySubject.Subscribe(observer);
        }
    }
}
