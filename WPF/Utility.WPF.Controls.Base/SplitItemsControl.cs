using System.Windows;
using System.Windows.Controls;
using Utility.Enums;

namespace Utility.WPF.Controls.Base
{
    public class SplitItemsControl : HeaderedItemsControl
    {
        public static readonly DependencyProperty MovementProperty =
            DependencyProperty.Register("Movement", typeof(XYMovement), typeof(SplitItemsControl), new PropertyMetadata(XYTraversal.LeftToRight));

        static SplitItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitItemsControl), new FrameworkPropertyMetadata(typeof(SplitItemsControl)));
        }

        public SplitItemsControl()
        {
        }

        #region properties

        public XYMovement Movement
        {
            get => (XYMovement)GetValue(MovementProperty);
            set => SetValue(MovementProperty, value);
        }

        #endregion properties
    }
}