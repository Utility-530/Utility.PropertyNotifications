using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
//using DynamicData;
//using DynamicData.Operators;
//using ReactiveUI;

namespace Utility.WPF.Controls
{
    public class PageNavigatorControl : Control
    {
        private SizeControl SizeControl;
        private NavigatorControl NavigatorControl;

        private ISubject<IEnumerable> ItemsSourceChanges = new Subject<IEnumerable>();
        private ISubject<int> PageSizeChanges = new Subject<int>();
        private ISubject<bool> ControlTemplateChanges = new Subject<bool>();
        //private ISubject<PageRequest> pageRequests = new Subject<PageRequest>();

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(PageNavigatorControl), new PropertyMetadata(null, ItemsSourceChanged));
        public static readonly DependencyProperty FilteredItemsProperty = DependencyProperty.Register("FilteredItems", typeof(IEnumerable), typeof(PageNavigatorControl));
        public static readonly DependencyProperty PageSizeProperty = DependencyProperty.Register("PageSize", typeof(object), typeof(PageNavigatorControl), new PropertyMetadata(null, PageSizeChanged));

        static PageNavigatorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PageNavigatorControl), new FrameworkPropertyMetadata(typeof(PageNavigatorControl)));
        }

        public PageNavigatorControl()
        {
           // var obs2 = new Subject<PageRequest>();

           // var obs = pageRequests.StartWith(new PageRequest(1, 20));

           // var filteredPaginatedVM = new FilteredPaginatedModel<object>(ItemsSourceChanges.Select(a => a.Cast<object>().ToObservable()).Switch().ToObservableChangeSet(),
           //obs,
           //obs.Select(_ => new Func<object, bool>(a => true)));

            //this.Dispatcher.InvokeAsync(() =>
            //FilteredItems = filteredPaginatedVM.Items, System.Windows.Threading.DispatcherPriority.Background);

            //filteredPaginatedVM.WithChangesTo(a => a.PageResponse)
            //    .CombineLatest(ControlTemplateChanges, (a, b) => b ? a : null)
            //    .Where(a => a != null)
            //    .Subscribe(a =>
            //    {
            //        this.Dispatcher.Invoke(() =>
            //            {
            //                SizeControl.TotalSize = a.Page;
            //                SizeControl.Size = a.PageSize;
            //                NavigatorControl.Size = a.Pages;
            //                NavigatorControl.Current = a.TotalSize;
            //            });
            //    });
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public IEnumerable FilteredItems
        {
            get { return (IEnumerable)GetValue(FilteredItemsProperty); }
            set { SetValue(FilteredItemsProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            SizeControl = this.GetTemplateChild("SizeControl") as SizeControl;
            NavigatorControl = this.GetTemplateChild("NavigatorControl") as NavigatorControl;
            ControlTemplateChanges.OnNext((SizeControl != null) && NavigatorControl != null);
            PageSizeChanges.OnNext(SizeControl.Size);
            //NavigatorControl.SelectedIndex += NavigatorControl_SelectedIndex;
            //SizeControl.SelectedSizeChanged += SizeControl_SelectedSizeChanged;
        }

        //private void NavigatorControl_SelectedIndex(object sender, RoutedEventArgs e)
        //{
        //    pageRequests.OnNext(new PageRequest((e as NavigatorControl.SelectedIndexRoutedEventArgs).Index, (int)this.GetValue(PageSizeProperty)));
        //}

        //private void SizeControl_SelectedSizeChanged(object sender, RoutedEventArgs e)
        //{
        //    pageRequests.OnNext(new PageRequest(NavigatorControl.Current, (e as SizeControl.SelectedSizeChangedRoutedEventArgs).Size));
        //}

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PageNavigatorControl).ItemsSourceChanges.OnNext((IEnumerable)e.NewValue);
        }

        private static void PageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PageNavigatorControl).PageSizeChanges.OnNext((int)e.NewValue);
        }

        //public class FilteredPaginatedModel<T> : ReactiveObject
        //{
        //    private IPageResponse pageResponse;
        //    private ReadOnlyObservableCollection<T> pitems;

        //    public FilteredPaginatedModel(IObservable<IChangeSet<T>> obs, IObservable<PageRequest> request, IObservable<Func<T, bool>> filter)
        //    {
        //        obs
        //              .Filter(filter)
        //              .Page(request)
        //                       .Do(_ =>
        //                       {
        //                           PageResponse = ((IPageChangeSet<T>)_).Response;
        //                       })
        //                       .Bind(out pitems)
        //                       .DisposeMany()
        //                       .Subscribe();
        //    }

        //    public ReadOnlyObservableCollection<T> Items => pitems;
        //    public IPageResponse PageResponse { get => pageResponse; set => this.RaiseAndSetIfChanged(ref pageResponse, value); }
        //}
    }
}