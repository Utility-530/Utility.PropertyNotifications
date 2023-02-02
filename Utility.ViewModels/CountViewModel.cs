//using DynamicData;
//using ReactiveUI;
//using System;
//using System.Reactive.Linq;
//using Utility.Common.Models;
//using Utility.Service;

//namespace Utility.ViewModels;

//public class CountViewModel : ViewModel, IObserver<IChangeSet>
//{
//    public CountViewModel():base("Count")
//    {
//    }

//    public override ReactiveProperty<int> Model => new ReactiveProperty<int>();

//    public void OnNext(IChangeSet value)
//    {
//        Model.Value = Model.Value + value.Adds - value.Removes;
//    }
//}

////public class FilteredCountViewModel<T> : ReactiveObject
////{
////    private readonly ObservableAsPropertyHelper<int> count;

////    public FilteredCountViewModel(IObservable<IChangeSet<T>> changeSet)
////    {
////        count = changeSet
////                    .Scan(0, (a, b) => a + b.Adds - b.Removes)
////                    .ToProperty(this, a => a.Count);
////    }

////    public int Count => count.Value;
////}