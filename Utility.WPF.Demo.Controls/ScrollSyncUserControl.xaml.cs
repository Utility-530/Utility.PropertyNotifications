using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls
{
    /// <summary>
    /// Interaction logic for TableUserControl.xaml
    /// </summary>
    public partial class ScrollSyncUserControl : UserControl
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable), typeof(ScrollSyncUserControl));

        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public ScrollSyncUserControl()
        {
            InitializeComponent();
            usercontrol.DataContext = this;
        }
    }
}