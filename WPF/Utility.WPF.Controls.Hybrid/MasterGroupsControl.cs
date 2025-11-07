using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using ReactiveUI;
using Utility.WPF.Attached;
using Utility.WPF.Controls.Dragablz;
using Utility.WPF.Controls.Master;

namespace Utility.WPF.Controls.Hybrid
{
    public class MasterGroupsControl : MasterBindableControl
    {
        public static readonly DependencyProperty IsReadOnlyPathProperty = DependencyProperty.Register("IsReadOnlyPath", typeof(string), typeof(MasterGroupsControl), new PropertyMetadata(null));

        //static MasterGroupsControl()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterGroupsControl), new FrameworkPropertyMetadata(typeof(MasterGroupsControl)));
        //}

        public MasterGroupsControl()
        {
            ButtonTypes = ButtonType.Add | ButtonType.Remove;
            RemoveOrder = RemoveOrder.Selected;
        }

        public string IsReadOnlyPath
        {
            get => (string)GetValue(IsReadOnlyPathProperty);
            set => SetValue(IsReadOnlyPathProperty, value);
        }

        public override void OnApplyTemplate()
        {
            Content = new GroupsControl
            {
                DisplayMemberPath = DisplayMemberPath,
                ItemsSource = ItemsSource,
                IsReadOnlyPath = IsReadOnlyPath
            };

            this.WhenAnyValue(a => a.ItemsSource)
                .Skip(1)
                .CombineLatest(this.WhenAnyValue(a => a.DisplayMemberPath), this.WhenAnyValue(a => a.IsReadOnlyPath))
                     .Subscribe(a =>
                     {
                         var (itemsSource, memberPath, readOnlyPath) = a;

                         Dispatcher.InvokeAsync(() =>
                         {
                             if (Content is GroupsControl groupsControl)
                             {
                                 groupsControl.ItemsSource = itemsSource;
                                 groupsControl.DisplayMemberPath = memberPath;
                                 groupsControl.IsReadOnlyPath = readOnlyPath;
                             }
                             else
                             {
                                 throw new ApplicationException("Expected Content to be MasterGroupsControl");
                             }

                             DoubleAnimation heightAnimation = new()
                             {
                                 From = 0,
                                 To = this?.ActualHeight ?? 0,
                                 Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500))
                             };
                             BeginAnimation(HeightProperty, heightAnimation);
                             Visibility = Visibility.Visible;
                         }, System.Windows.Threading.DispatcherPriority.Background);
                     });

            base.OnApplyTemplate();
        }

        protected override void ExecuteAdd()
        {
            ExecuteAddRemove(false);
            base.ExecuteAdd();
        }

        protected override void ExecuteRemove()
        {
            ExecuteAddRemove(true);
            base.ExecuteRemove();
        }

        private void ExecuteAddRemove(bool isAdd)
        {
            if (SelectedItem is UIElement elem)
            {
                elem.SetValue(Ex.IsReadOnlyProperty, isAdd);
            }
            else if (SelectedItem is { })
            {
                var container = (Content as ItemsControl)?.ItemContainerGenerator.ContainerFromItem(SelectedItem);
                container?.SetValue(Ex.IsReadOnlyProperty, isAdd);
            }
        }
    }
}