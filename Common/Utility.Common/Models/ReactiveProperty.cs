namespace Utility.Common.Models
{
#nullable enable

    //public class ReactiveProperty : ReactiveProperty<string>
    //{
    //}

    //public class ReactiveProperty<T> : Property<T>, IObservable<T>
    //{
    //    private readonly IEqualityComparer<T>? equalityComparer;
    //    private readonly ReplaySubject<T> subject = new ReplaySubject<T>(1);
    //    private T value = default!;

    //    public ReactiveProperty()
    //    {
    //    }

    //    public ReactiveProperty(T value, IEqualityComparer<T>? equalityComparer = default) : this(equalityComparer)
    //    {
    //        Value = value;
    //    }

    //    public ReactiveProperty(IEqualityComparer<T>? equalityComparer = default) => this.equalityComparer = equalityComparer;

    //    public override T Value
    //    {
    //        get => value;
    //        set
    //        {
    //            if (equalityComparer?.Equals(value, this.value) is true || value?.Equals(this.value) is true) return;
    //            this.value = value;
    //            RaisePropertyChanged();
    //            subject.OnNext(value);
    //        }
    //    }

    //    public IDisposable Subscribe(IObserver<T> observer)
    //    {
    //        return subject.Subscribe(observer);
    //    }
    //}

    //public class Property<T> : Property, INotifyPropertyChanged
    //{
    //    private readonly IEqualityComparer<T>? equalityComparer;
    //    private readonly ReplaySubject<T> subject = new ReplaySubject<T>(1);
    //    private T value = default!;

    //    public event PropertyChangedEventHandler? PropertyChanged;

    //    public Property()
    //    {
    //    }

    //    public Property(T value)
    //    {
    //        Value = value;
    //    }

    //    public virtual T Value
    //    {
    //        get => value;
    //        set => throw new Exception("Unable to set readonly property");
    //    }

    //    public void RaisePropertyChanged([CallerMemberName] string? callerMemberName = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
    //    }

    //    public IDisposable Subscribe(IObserver<T> observer)
    //    {
    //        return subject.Subscribe(observer);
    //    }
    //}

    //public class Property
    //{
    //}

    //public class StringProperty : Property
    //{
    //    public StringProperty(string value) => Value = value;
    //    public string Value { get; set; }

    //    public static implicit operator string(StringProperty stringProperty)
    //    {
    //        return stringProperty.Value;
    //    }

    //    public static implicit operator StringProperty(string stringValue)
    //    {
    //        return new StringProperty(stringValue);
    //    }
    //}
}