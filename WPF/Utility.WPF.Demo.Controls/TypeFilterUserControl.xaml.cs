using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.WPF.Controls;

namespace Utility.WPF.Demo.Controls
{
    /// <summary>
    /// Interaction logic for TypeFilterUserControl.xaml
    /// </summary>
    public partial class TypeFilterUserControl : UserControl
    {
        private CollectionView view => (CollectionView)CollectionViewSource.GetDefaultView(listBox.ItemsSource);

        public TypeFilterUserControl()
        {
            InitializeComponent();
        }

        private void TypeControl_Changed_2(object sender, TypeControl.ChangedEventArgs e)
        {
            view.Filter = item =>
            {
                if (e.Value == null)
                {
                    return true;
                }
                PropertyInfo info = item.GetType().GetProperty(e.Property);
                if (info == null)
                    return false;

                return info.GetValue(item, null).ToString().ToLower().Contains(e.Value.ToLower());
            };
        }
    }
}