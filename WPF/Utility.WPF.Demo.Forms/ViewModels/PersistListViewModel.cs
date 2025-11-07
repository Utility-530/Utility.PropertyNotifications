using ReactiveUI;
using Utility.Interfaces.NonGeneric.Data;

namespace Utility.WPF.Demo.Forms.ViewModels
{
    public class PersistListViewModel : ReactiveObject
    {
        //private readonly ReactiveFieldsFactory factory = new();
        //private IEnumerator<Item> build;
        //private readonly CollectionService service = new();

        //private IRepository repository;

        public PersistListViewModel(IRepository repository)
        {
            //this.WhenAnyValue(a => a.DatabaseService)
            //    .WhereNotNull()
            //    .Subscribe(a => { service.OnNext(new(a)); });

            //service.OnNext(new(repository));

            //ChangeCommand = ReactiveCommand.Create<CollectionEventArgs, Unit>((obj) =>
            //{
            //    switch (obj)
            //    {
            //        case { EventType: EventType.Add }:
            //            if (NewItem.MoveNext())
            //                service.Items.Add(NewItem.Current);
            //            break;

            //        case { EventType: EventType.Remove, Item: { } item }:
            //            service.Items.Remove(item);
            //            break;

            //        case { EventType: EventType.Remove }:
            //            service.Items.RemoveAt(service.Items.Count - 1);
            //            break;

            //        case MovementEventArgs eventArgs:
            //            foreach (var item in eventArgs.Changes)
            //            {
            //                //Data.Move(item.OldIndex, item.Index);
            //            }
            //            break;

            //        default:
            //            break;
            //    }
            //    return Unit.Default;
            //});
            //this.repository = repository;
        }

        //public IValueConverter Converter { get; } = new ValueConverter();

        ////public MintPlayer.ObservableCollection.ObservableCollection<Item> Data { get; } = new MintPlayer.ObservableCollection.ObservableCollection<Item>();

        //public IEnumerable Data => service.Items;

        //public IEnumerator NewItem
        //{
        //    get
        //    {
        //        var build = 0.Repeat().Select(a => new Item()).GetEnumerator();
        //        return build;
        //    }
        //}

        //public ReactiveCommand<CollectionEventArgs, Unit> ChangeCommand { get; }
        //public ReactiveCommand<bool, Unit> ChangeRepositoryCommand { get; }
        //public ReactiveCommand<object, Unit> CollectionChangedCommand { get; }

        //public IRepository DatabaseService { get => repository; private set => this.RaiseAndSetIfChanged(ref repository, value); }
    }
}