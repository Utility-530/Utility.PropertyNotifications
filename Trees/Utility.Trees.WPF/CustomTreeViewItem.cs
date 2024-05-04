using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace Utility.Trees.WPF
{
    public class CustomTreeViewItem : TreeViewItem, IObservable<object>
    {
        ReplaySubject<object> _playSubject = new(1);
        private Button? plus;
        private Button? minus;
        public static readonly DependencyProperty MousePositionProperty = DependencyProperty.Register("MousePosition", typeof(Point), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty AddCommandProperty = DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(CustomTreeViewItem), new PropertyMetadata(AddChanged));

        private static void AddChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomTreeViewItem { plus: { } plus } item && e.NewValue is ICommand command)
                plus.Command = command;
        }

        public static readonly DependencyProperty RemoveCommandProperty = DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(CustomTreeViewItem), new PropertyMetadata(RemoveChanged));

        private static void RemoveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomTreeViewItem { minus: { } minus } item && e.NewValue is ICommand command)
                minus.Command = command;
        }

        // Register a custom routed event using the Bubble routing strategy.
        public static readonly RoutedEvent ConditionalClickEvent = EventManager.RegisterRoutedEvent(
            name: "ConditionalClick",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(CustomTreeViewItem));

        // Provide CLR accessors for assigning an event handler.
        public event RoutedEventHandler ConditionalClick
        {
            add { AddHandler(ConditionalClickEvent, value); }
            remove { RemoveHandler(ConditionalClickEvent, value); }
        }



        public CustomTreeViewItem()
        {
            //var textBlock = new ToolTip { Name = "FDgdfg" };
            //var binding = new Binding { RelativeSource = new RelativeSource { Mode = RelativeSourceMode.Self }, Path = new PropertyPath(nameof(System.Windows.Controls.ToolTip.PlacementTarget) + "." + nameof(Header) + "." + nameof(ViewModel.Guid)), Mode = BindingMode.OneWay };
            //textBlock.SetBinding(ContentControl.ContentProperty, binding);
            //ToolTipService.SetInitialShowDelay(textBlock, 100);
            //ToolTipService.SetShowDuration(textBlock, 10000);
            //ToolTip = textBlock;


            var contextMenu = new ContextMenu { };
            var show = new MenuItem { Header = "show" };
            show.Click += Show_Click;
            contextMenu.Items.Add(show);
            //var contextBinding = new Binding { RelativeSource = new RelativeSource { Mode = RelativeSourceMode.Self }, Path = new PropertyPath(nameof(System.Windows.Controls.ToolTip.PlacementTarget) + "." + nameof(Header) + "." + nameof(ViewModel.Guid)), Mode = BindingMode.OneWay };
            //ToolTipService.SetInitialShowDelay(textBlock, 100);
            //ToolTipService.SetShowDuration(textBlock, 10000);
            ContextMenu = contextMenu;
            Loaded += CustomTreeViewItem_Loaded;

        }

        private void CustomTreeViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ConditionalClickEvent));
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            _playSubject.OnNext(null);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var headerBorder = GetTemplateChild("Bd") as Border;
            plus = GetTemplateChild("PlusButton") as Button;
            minus = GetTemplateChild("MinusButton") as Button;
            if (plus != null)
                plus.Command = AddCommand;
            if (minus != null)
                minus.Command = RemoveCommand;
            //SetBinding(Grid.RowProperty, new Binding { Source = Header, Path = new PropertyPath(nameof(ViewModel.Row)) });
            //SetBinding(Grid.ColumnProperty, new Binding { Source = Header, Path = new PropertyPath(nameof(ViewModel.Column)) });
            ////this.SetBinding(TreeMapPanel.AreaProperty, new Binding { Source = Header, Path = new PropertyPath(nameof(ViewModel.Area)) });
            ////this.SetBinding(TreeStackPanel.AreaProperty, new Binding { Source = Header, Path = new PropertyPath(nameof(ViewModel.Area)) });
            ////this.SetBinding(AutoGrid.RowHeightOverrideProperty, new Binding { Source = Header, Path = new PropertyPath(nameof(ViewModel.Height)) });
            ////this.SetBinding(AutoGrid.ColumnWidthOverrideProperty, new Binding { Source = Header, Path = new PropertyPath(nameof(ViewModel.Width)) });
            //SetBinding(IsSelectedProperty, new Binding { Source = Header, Path = new PropertyPath(nameof(ViewModel.IsSelected)) });
            //headerBorder.SetBinding(IsEnabledProperty, new Binding { Source = Header, Path = new PropertyPath(nameof(ViewModel.IsEnabled)) });
            //headerBorder.SetBinding(VisibilityProperty, new Binding { Source = Header, Path = new PropertyPath(nameof(ViewModel.Visibility)) });
            //this.SetBinding(TreeViewItem.IsExpandedProperty, new Binding { Source = Header, Path = new PropertyPath(nameof(ViewModel.IsExpanded)), Mode = BindingMode.TwoWay });

            //var x = headerBorder.FindVisualChildren<ContentPresenter>().Single();
            //adornerController = new ButtonAdornerController(x);
            //adornerController.Subscribe(_playSubject);
        }



        public Point MousePosition
        {
            get { return (Point)GetValue(MousePositionProperty); }
            set { SetValue(MousePositionProperty, value); }
        }

        public ICommand AddCommand
        {
            get { return (ICommand)GetValue(AddCommandProperty); }
            set { SetValue(AddCommandProperty, value); }
        }
        public ICommand RemoveCommand
        {
            get { return (ICommand)GetValue(RemoveCommandProperty); }
            set { SetValue(RemoveCommandProperty, value); }
        }



        public IDisposable Subscribe(IObserver<object> observer)
        {

            return _playSubject
                .Select(a =>
                {
                    return Header;
                })
                .Subscribe(observer);
        }


    }
}
