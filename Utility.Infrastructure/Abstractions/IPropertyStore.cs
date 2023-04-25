using Utility.Interfaces.NonGeneric;
using Utility.Observables;

namespace Utility.Infrastructure.Abstractions
{
    //public record KeyValue(IKey Key, object Value);

    //public interface IKey : IEquatable<IKey>
    //{
    //}

    //public interface IObserver : IKey
    //{
    //    public void OnNext(IPropertyChange propertyResult);
    //}

    //public interface IPropertyChange
    //{
    //    //public IEnumerable<IPropertyResult> Results { get; set; }
    //    public IKey Key { get; }

    //    public object Value { get; }
    //}

    //public interface IPropertyResult
    //{
    //    public Guid Guid { get; }
    //}

    //public interface IError : IPropertyResult
    //{
    //    public string Description { get; }
    //}

    //public interface IValidation
    //{
    //    public bool IsValid { get; }

    //    public string Description { get; }
    //}


    public interface IPropertyStore : IObserver, IObservable
    {
        //public void GetValue(IEquatable key);

        //public void SetValue(IEquatable key, object value);

       // public Task<Guid> Get*/GuidByParent(IEquatable key);

        //IDisposable Subscribe(IObserver observer);

        // string Validate(string memberName);
    }
}