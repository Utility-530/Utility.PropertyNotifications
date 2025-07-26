using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Utility.ViewModels;
using Utility.WPF.Controls.Meta;
using Utility.WPF.Helpers;

namespace Utility.WPF.Demo.Meta
{
    /// <summary>
    /// Interaction logic for UserGrid.xaml
    /// </summary>
    public partial class UserGrid : UserControl
    {
        public UserGrid()
        {
            InitializeComponent();
            (this.Content as UniformGrid).ChildrenOfType<TypesComboBox>()
                .Single().SelectionChanged += UserGrid_SelectionChanged;
        }

        private void UserGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show(e.AddedItems.Cast<object>().SingleOrDefault()?.ToString());
        }
    }

    public class DemoType
    {
        public string Property { get; set; }
    }

    public class ViewModelItemTemplateSelector:DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ViewModel viewModel)
            {
                var x = new DataTemplateKey(typeof(ViewModel));
                var xx = container.FindTemplate(x);
                return xx;
            }
            var bas = base.SelectTemplate(item, container);
            return bas;
        }
    }
}
