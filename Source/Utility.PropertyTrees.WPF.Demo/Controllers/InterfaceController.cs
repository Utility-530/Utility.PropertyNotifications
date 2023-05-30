using Utility.Infrastructure;
using Utility.Models;
using Utility.PropertyTrees.Abstractions;
using Utility.PropertyTrees.Infrastructure;


internal sealed class InterfaceController : BaseObject
{
    private readonly Dictionary<Type, Type> dictionary = new() { { typeof(IViewModel), typeof(ViewModel) } };

    public override Key Key => new(Guids.Interface, nameof(InterfaceController), typeof(InterfaceController));

    public override object Model => dictionary;

    public IObservable<Response> OnNext(TypeRequest value)
    {
        return Return(new TypeResponse(dictionary[value.Type]));
    }
}
