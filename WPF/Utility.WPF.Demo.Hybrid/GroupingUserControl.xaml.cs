using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.Commands;
using Utility.Enums;
using Utility.EventArguments;
using Utility.Nodes;
using Utility.PropertyNotifications;
using Utility.ViewModels;
using Utility.ViewModels.Customs.Infrastructure;
using Utility.WPF.Demo.Data.Factory;
using Utility.WPF.Demo.Data.Model;
using sim = Utility.Services.Deprecated.FilterDictionaryService<Utility.WPF.Demo.Data.Model.Stock>;

namespace Utility.WPF.Demo.Hybrid
{
    /// <summary>
    /// Interaction logic for GroupingUserControl.xaml
    /// </summary>
    public partial class GroupingUserControl : UserControl
    {
        public GroupingUserControl()
        {
            InitializeComponent();
        }
    }

    public class GroupingViewModel : NotifyPropertyClass
    {
        private static string InitialPropertyName = nameof(Stock.Sector);
        private ClassProperty selected;
        private ICommand command;

        public GroupingViewModel()
        {
            var aa = new sim(a => a.Key);
            CollectionViewModel = new(StockObservableFactory.GenerateChangeSet(), aa, InitialPropertyName);
            selected = CollectionViewModel.Properties.First();

            this.WithChangesTo(a => a.Selected)
                .Select(a => (ClassProperty?)a)
                .Subscribe(CollectionViewModel);

            command = new Command<CollectionItemEventArgs>((a) =>
            {
                if (a is { EventType: EventType.Enable })
                    CollectionViewModel.OnNext(Selected);
                if (a is { EventType: EventType.Disable })
                    CollectionViewModel.OnNext(null);
                else
                    throw new System.Exception("dfs33 33");
            });


        }

        public CollectionGroupViewModel<Stock, string> CollectionViewModel { get; }

        public ICommand Command => command;

        public ClassProperty Selected { get => selected; set => this.RaisePropertyChanged(ref selected, value); }
    }
}