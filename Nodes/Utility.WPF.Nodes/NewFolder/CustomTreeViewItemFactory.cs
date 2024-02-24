using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Views.Trees;
using Jellyfish;

namespace VisualJsonEditor.Test.Infrastructure
{
    public class CustomTreeViewItemFactory : ITreeViewItemFactory, IObservable<ViewModel>
    {
        private ReplaySubject<ViewModel> _replaySubject = new();

        public TreeViewItem Make()
        {
            var item = new CustomTreeViewItem();
            item.IsExpanded = true;
            item.BorderThickness = new Thickness(2);
            item.Cast<ViewModel>().Subscribe(_replaySubject);
            return item;
        }

        public IDisposable Subscribe(IObserver<ViewModel> observer)
        {
            return _replaySubject.Subscribe(observer);
        }

        public static CustomTreeViewItemFactory Instance { get; } = new();
    }

}
