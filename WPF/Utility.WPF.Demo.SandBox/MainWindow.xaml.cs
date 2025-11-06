using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.Commands;
using Utility.WPF.Controls.Objects;
using System.Windows.Data;
using Utility.WPF.Demo.Data.Model;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Utility.WPF.Controls.Base;
using Microsoft.Xaml.Behaviors;

namespace Utility.WPF.Demo.SandBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Character newObject = new Character() { Color = Colors.Aqua };

        public MainWindow()
        {
            InitializeComponent();
            FinishEdit = new Command<object>(o =>
            {
                if (o is EditRoutedEventArgs args)
                {
                    Items.Add(args.Edit as Character);
                }
            });

            Items = new (Utility.WPF.Demo.Data.Resources.Instance["Characters"] as Character[]);

            MainGrid.DataContext = this;
        }

        public ICommand FinishEdit { get; }

        public ObservableCollection<Character> Items { get; }
        public Character Edit
        {
            get => newObject; set
            {
                newObject = value;
            }
        }
    }


    public class BreadCrumbs : CustomItemsControl
    {

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(BreadCrumbs), new PropertyMetadata());
        public static readonly DependencyProperty ContainerWidthProperty =
            DependencyProperty.Register("ContainerWidth", typeof(double), typeof(BreadCrumbs), new PropertyMetadata(80.0));

        static BreadCrumbs()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadCrumbs), new FrameworkPropertyMetadata(typeof(BreadCrumbs)));
            HorizontalAlignmentProperty.OverrideMetadata(typeof(BreadCrumbs), new FrameworkPropertyMetadata(HorizontalAlignment.Right));
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }



        public double ContainerWidth
        {
            get { return (double)GetValue(ContainerWidthProperty); }
            set { SetValue(ContainerWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContainerWidth.  This enables animation, styling, binding, etc...



    }

    public class LastItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is ItemsControl itemscontrol && (values[1] is FrameworkElement fe))
            {
                int count = itemscontrol.Items.Count;

                if (values != null && values.Length >= 3 && count > 0)
                {
                    var lastItem = itemscontrol.Items[count - 1];
                    if (Equals(values[2], false) && Equals(lastItem, fe.DataContext) == false)
                    {
                        return true;
                    }
                    return false;
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is double x && values[1] is double y)
            {
                return x * y;
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public class ScrollToBottomBehavior : Behavior<System.Windows.Controls.ScrollViewer>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            // Add event handler to scroll to the bottom when content changes
            AssociatedObject.LayoutUpdated += ScrollToBottom;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            // Remove event handler
            AssociatedObject.LayoutUpdated -= ScrollToBottom;
        }

        private void ScrollToBottom(object sender, EventArgs e)
        {
            AssociatedObject.ScrollToBottom();
        }
    }
}