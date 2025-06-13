//using System.Collections;
//using System.Collections.ObjectModel;

//namespace Utility.Models;


//public class Disposer<TObserver, T> : IDisposable where TObserver : IObserver<T>
//{
//    private readonly ICollection<TObserver> observers;
//    private readonly TObserver observer;

//    public Disposer(ICollection<TObserver> observers, TObserver observer)
//    {
//        (this.observers = observers).Add(observer);
//        this.observer = observer;
//    }

//    public IEnumerable Observers => observers;
//    public IObserver<T> Observer => observer;

//    public void Dispose()
//    {
//        observers?.Remove(observer);
//    }

//    public static Disposer<TObserver, T> Empty => new(new Collection<TObserver>(), default);
//}

//public class Disposer : IDisposable
//{
//    private readonly IList observers;
//    private readonly IObserver observer;

//    public Disposer(IList observers, IObserver observer)
//    {
//        (this.observers = observers).Add(observer);
//        this.observer = observer;
//    }

//    public IEnumerable Observers => observers;
//    public IObserver Observer => observer;

//    public void Dispose()
//    {
//        observers?.Remove(observer);
//    }

//    //public static Disposer<TObserver, T> Empty => new(new Collection<TObserver>(), default);
//}

//public interface IObserver
//{
//    void OnNext(object value);
//    void OnError(Exception value);
//    void OnCompleted();
//}

//public interface IObservable
//{

//    IDisposable Subscribe(IObserver value);


//}