using System.Collections;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.Common.EventArgs;
using Utility.WPF.Demo.Data.Factory;

namespace Utility.WPF.Demo.Master.View
{
    /// <summary>
    /// Interaction logic for ItemsWrapUserControl.xaml
    /// </summary>
    public partial class MasterControlUserControl : UserControl
    {
        public MasterControlUserControl()
        {
            InitializeComponent();
        }
    }

    public class MasterControlViewModel
    {
        private static FieldsFactory Factory { get; } = new();

        public IEnumerable Data { get; } = Factory.BuildCollection().ToArray();

        public ICommand ChangeCommand { get; } = ReactiveUI.ReactiveCommand.Create<CollectionEventArgs>(a =>
         {
         });
    }
}