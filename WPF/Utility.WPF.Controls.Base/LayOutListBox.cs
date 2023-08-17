using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Abstract;
using Utility.WPF.Helpers;
using static Utility.WPF.Helpers.LayOutHelper;
using Orientation = System.Windows.Controls.Orientation;

namespace Utility.WPF.Controls.Base
{
    public class LayOutListBox : ListBox, IOrientation //, IWrapping
    {
        public static readonly DependencyProperty OrientationProperty = LayOutItemsControl.OrientationProperty.AddOwner(typeof(LayOutListBox), new PropertyMetadata(LayOutItemsControl.OrientationChanged));
        public static readonly DependencyProperty ArrangementProperty = LayOutItemsControl.ArrangementProperty.AddOwner(typeof(LayOutListBox), new PropertyMetadata(LayOutItemsControl.ArrangementChanged));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public Arrangement Arrangement
        {
            get { return (Arrangement)GetValue(ArrangementProperty); }
            set { SetValue(ArrangementProperty, value); }
        }
    }

    public class LayOutItemsControl : ItemsControl, IOrientation
    {
        public static readonly DependencyProperty OrientationProperty =
            WrapPanel.OrientationProperty.AddOwner(typeof(LayOutItemsControl), new PropertyMetadata(OrientationChanged));

        public static readonly DependencyProperty ArrangementProperty =
            DependencyProperty.Register("Arrangement", typeof(Arrangement), typeof(LayOutItemsControl), new PropertyMetadata(ArrangementChanged));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public Arrangement Arrangement
        {
            get { return (Arrangement)GetValue(ArrangementProperty); }
            set { SetValue(ArrangementProperty, value); }
        }

        public static void OrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl itemsControl)
                if (e.NewValue is Orientation orientation)
                {
                    var arrangement = (Arrangement)d.GetValue(ArrangementProperty);
                    LayOutHelper.Changed(itemsControl, orientation, arrangement);
                }
        }

        public static void ArrangementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl itemsControl)
                if (e.NewValue is Arrangement arrangement)
                {
                    var orientation = (Orientation)d.GetValue(WrapPanel.OrientationProperty);
                    LayOutHelper.Changed(itemsControl, orientation, arrangement);
                }
        }
    }
}