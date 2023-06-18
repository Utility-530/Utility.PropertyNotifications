using ReactiveUI;
using System.Linq;
using System.Reactive.Disposables;

namespace Utility.Tasks.View
{
    /// <summary>
    /// Interaction logic for ProgressStateSummaryView.xaml
    /// </summary>
    public partial class ProgressStateSummaryView 
    {
        public ProgressStateSummaryView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                StateTextBlock.Text = ViewModel.State.ToString();
                DateTextBlock.Text = ViewModel.Date.ToString();
            });
        }
    }
}
