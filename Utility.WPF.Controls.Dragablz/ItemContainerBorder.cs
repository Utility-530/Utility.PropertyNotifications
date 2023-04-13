using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Dragablz
{
    public class ItemContainerBorder : ContentControl
    {
        public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.Register("IsDragging", typeof(bool), typeof(ItemContainerBorder), new PropertyMetadata(false));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(ItemContainerBorder), new PropertyMetadata(false));
        public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.Register("IsSelectable", typeof(bool), typeof(ItemContainerBorder), new PropertyMetadata(false));
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(ItemContainerBorder), new PropertyMetadata(new CornerRadius(12)));

        static ItemContainerBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemContainerBorder), new FrameworkPropertyMetadata(typeof(ItemContainerBorder)));
        }

        #region properties

        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set { SetValue(IsDraggingProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public bool IsSelectable
        {
            get { return (bool)GetValue(IsSelectableProperty); }
            set { SetValue(IsSelectableProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        #endregion properties
    }
}