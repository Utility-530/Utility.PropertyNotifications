using System;
using DynamicData;
using Utility.Models;

namespace Utility.ViewModels;

public class CountViewModel : ViewModel, IObserver<IChangeSet>
{
    public CountViewModel() : base("Count")
    {
    }

    public override Property<int> Model => new Property<int>(0);

    public void OnNext(IChangeSet value)
    {
        Model.SetValue(Model.GetValue() + value.Adds - value.Removes);
    }
}

//public class FilteredCountViewModel<T> : ReactiveObject
//{
//    private readonly ObservableAsPropertyHelper<int> count;

//    public FilteredCountViewModel(IObservable<IChangeSet<T>> changeSet)
//    {
//        count = changeSet
//                    .Scan(0, (a, b) => a + b.Adds - b.Removes)
//                    .ToProperty(this, a => a.Count);
//    }

//    public int Count => count.Value;
//}