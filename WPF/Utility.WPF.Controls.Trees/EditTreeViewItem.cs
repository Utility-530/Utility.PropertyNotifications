using Fasterflect;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Utility.Commands;
using Utility.WPF.Helpers;

namespace Utility.WPF.Controls.Trees
{
    public partial class CustomTreeViewItem
    {
        public static readonly DependencyProperty CheckedCommandProperty =
            DependencyProperty.Register("CheckedCommand", typeof(ICommand), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty SwapCommandProperty =
            DependencyProperty.Register("SwapCommand", typeof(ICommand), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty UnCheckedCommandProperty =
            DependencyProperty.Register("UnCheckedCommand", typeof(ICommand), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty CheckedPropertyNameProperty =
            DependencyProperty.Register("CheckedPropertyName", typeof(string), typeof(CustomTreeViewItem), new PropertyMetadata("IsChecked", isCheckedChanged));
        public static readonly DependencyProperty FinishEditCommandProperty =
            DependencyProperty.Register("FinishEditCommand", typeof(ICommand), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty NewObjectConverterProperty = DependencyProperty.Register("NewObjectConverter", typeof(IValueConverter), typeof(CustomTreeViewItem), new PropertyMetadata(new NewObjectConverter()));

        public static readonly DependencyProperty MaxHeightOfEditBoxProperty = DependencyProperty.Register("MaxHeightOfEditBox", typeof(double), typeof(CustomTreeViewItem), new PropertyMetadata(300.0));


        private static void isCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomTreeView sel && e.NewValue is string str)
            {
            }
        }

        private void initialiseCommands()
        {
            SwapCommand = new Command<int[]>(
                (indexes) =>
                {
                    if (this.ItemsSource is IList collection)
                    {
                        int fromS = indexes[0];
                        int to = indexes[1];
                        var elementSource = collection[to];
                        var dragged = collection[fromS];
                        collection[to] = dragged;
                        collection[fromS] = elementSource;

                    }
                });

            RemoveCommand = new Command<object>((a) =>
            {

                int i = 0;
                if (this.ItemsSource is IList collection)
                    foreach (var item in collection)
                    {
                        if (item == (a))
                        {
                            collection.RemoveAt(i);
                            return;
                        }
                        i++;
                    }
            });

            CheckedCommand = new Command<object>((a) =>
            {

                if (string.IsNullOrEmpty(CheckedPropertyName) == false)
                    a.TrySetPropertyValue(CheckedPropertyName, true);

            });

            UnCheckedCommand = new Command<object>((a) =>
            {
                if (string.IsNullOrEmpty(CheckedPropertyName) == false)
                    a.TrySetPropertyValue(CheckedPropertyName, false);
            });
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is ListBoxItem lbi)
            {
                lbi.Loaded += Lbi_Loaded;
            }
            void Lbi_Loaded(object sender, RoutedEventArgs e)
            {
                BindingOperations.SetBinding(element.FindChild<CheckBox>(), CheckBox.IsCheckedProperty, new Binding { Source = item, Path = new PropertyPath(CheckedPropertyName) });

            }

            base.PrepareContainerForItemOverride(element, item);
        }


        #region properties

        public double MaxHeightOfEditBox
        {
            get { return (double)GetValue(MaxHeightOfEditBoxProperty); }
            set { SetValue(MaxHeightOfEditBoxProperty, value); }
        }

        public IValueConverter NewObjectConverter
        {
            get { return (IValueConverter)GetValue(NewObjectConverterProperty); }
            set { SetValue(NewObjectConverterProperty, value); }
        }

        public ICommand CheckedCommand
        {
            get { return (ICommand)GetValue(CheckedCommandProperty); }
            set { SetValue(CheckedCommandProperty, value); }
        }

        public ICommand UnCheckedCommand
        {
            get { return (ICommand)GetValue(UnCheckedCommandProperty); }
            set { SetValue(UnCheckedCommandProperty, value); }
        }


        public ICommand SwapCommand
        {
            get { return (ICommand)GetValue(SwapCommandProperty); }
            set { SetValue(SwapCommandProperty, value); }
        }

        public string CheckedPropertyName
        {
            get { return (string)GetValue(CheckedPropertyNameProperty); }
            set { SetValue(CheckedPropertyNameProperty, value); }
        }

        public ICommand FinishEditCommand
        {
            get { return (ICommand)GetValue(FinishEditCommandProperty); }
            set { SetValue(FinishEditCommandProperty, value); }
        }

        #endregion properties

    }
}
