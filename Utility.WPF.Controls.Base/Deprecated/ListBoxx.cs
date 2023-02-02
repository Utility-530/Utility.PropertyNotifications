using System;
using System.Reactive.Linq;
using System.Windows;
//using System.Windows.Controls;
//using UtilityWpf.Mixins;
//using Utility.WPF.Reactive;

namespace Utility.WPF.Base
{
    //public abstract class ListBoxx : ListBox, IPropertyListener, IControlListener
    //{
    //    private readonly NameTypeDictionary<SingleReplaySubject<object>> dict;

    //    IObservable<FrameworkElement> IControlListener.lazy { get; set; }
    //    Type IDependencyObjectListener.Type { get; } = typeof(Controlx);
    //    NameTypeDictionary<SingleReplaySubject<object>> IPropertyListener.dict => dict;

    //    public ListBoxx()
    //    {
    //        dict = new(this);

    //        this.LoadedChanges()
    //            .Take(1)
    //            .Subscribe(a =>
    //            {
    //                (this as IPropertyListener).Init();
    //            });
    //    }

    //    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    //    {
    //        (this as IPropertyListener).OnPropertyChanged(e);
    //        base.OnPropertyChanged(e);
    //    }
    //}
}