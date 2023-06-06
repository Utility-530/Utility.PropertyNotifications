using DryIoc;
using Utility.PropertyTrees.WPF.Demo.Views;

namespace Utility.PropertyTrees.WPF.Demo
{
    public partial class MainView : UserControl
    {
        public MainView(IContainer container)
        {
            InitializeComponent();
        }
    }
}