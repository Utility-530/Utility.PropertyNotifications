using DryIoc;
using Utility.PropertyTrees.Services;
using static Utility.PropertyTrees.Events;
using Utility.Observables.Generic;

namespace Utility.PropertyTrees.WPF.Demo.Views
{
    /// <summary>
    /// Interaction logic for ViewEditorView.xaml
    /// </summary>
    public partial class ViewEditorView : UserControl
    {
        public ViewEditorView()
        {
            InitializeComponent();

            var store  = container.Resolve<ViewModelStore>();
           
            store.OnNext(new GetViewModelRequest(new Key(Guid.NewGuid(), null, null)))
                .Subscribe(a =>
                {
                    MyDataGrid.ItemsSource = a.Value as IEnumerable;
                });
        }
    }
}
