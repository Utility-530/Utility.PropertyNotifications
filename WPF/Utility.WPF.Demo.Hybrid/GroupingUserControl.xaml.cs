using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;
using Utility.Enums;
using Utility.EventArguments;
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

    public class GroupingViewModel : ReactiveObject
    {
        private static string InitialPropertyName = nameof(Stock.Sector);
        private ClassProperty selected;
        private ReactiveCommand<CollectionItemEventArgs, ClassProperty?> command;

        public GroupingViewModel()
        {
            var aa = new sim(a => a.Key);
            CollectionViewModel = new(StockObservableFactory.GenerateChangeSet(), aa, InitialPropertyName);
            selected = CollectionViewModel.Properties.First();

            this.WhenAnyValue(a => a.Selected)
                .Select(a => (ClassProperty?)a)
                .Subscribe(CollectionViewModel);

            command = ReactiveCommand.Create<CollectionItemEventArgs, ClassProperty?>((a) =>
            {
                if (a is { EventType: EventType.Enable })
                    return Selected;
                if (a is { EventType: EventType.Disable })
                    return null;
                else
                    throw new System.Exception("dfs33 33");
            });

            command.Subscribe(CollectionViewModel);
        }

        public CollectionGroupViewModel<Stock, string> CollectionViewModel { get; }

        public ICommand Command => command;

        public ClassProperty Selected { get => selected; set => this.RaiseAndSetIfChanged(ref selected, value); }
    }
}