using System.Windows;
using Utility.WPF.Attached;
using Utility.Enums;
using Utility.WPF.Controls.Base;
using Evan.Wpf;
using System.Windows.Data;
using Utility.WPF.Controls.Objects.Infrastructure;

namespace Utility.WPF.Controls.Objects
{

    public class ObjectItemsControl : LayOutItemsControl
    {
        public static readonly DependencyProperty ObjectProperty = DependencyHelper.Register<object>(new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ValueProperty = DependencyHelper.Register();

        static ObjectItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ObjectItemsControl), new FrameworkPropertyMetadata(typeof(ObjectItemsControl)));


        }

        public ObjectItemsControl()
        {
            SetValue(ItemsControlEx.ArrangementProperty, Arrangement.Wrapped);
            DataContext = new ObjectItemsControlViewModel();
            SetBinding(ObjectProperty, new Binding(nameof(ObjectItemsControlViewModel.Object)) { Mode = BindingMode.OneWayToSource });
            SetBinding(ValueProperty, new Binding(nameof(ObjectItemsControlViewModel.Value)) { Mode = BindingMode.OneWay });
            SetBinding(ItemsSourceProperty, new Binding(nameof(ObjectItemsControlViewModel.Items)) { Mode = BindingMode.OneWay });
        }



        public object Object
        {
            get => GetValue(ObjectProperty);
            set => SetValue(ObjectProperty, value);
        }

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}