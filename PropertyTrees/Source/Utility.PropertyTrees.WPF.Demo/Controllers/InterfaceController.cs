using Utility;
using Utility.Infrastructure;

internal sealed class InterfaceController : BaseObject
{
    private readonly Dictionary<Type, Type> dictionary = new() {  /*{typeof(IViewModel), typeof(ViewModel)} */ };

    public override Key Key => new(Guids.Interface, nameof(InterfaceController), typeof(InterfaceController));

    public override object Model => dictionary;

    public IObservable<TypeResponse> OnNext(TypeRequest value)
    {
        return Return(new TypeResponse(dictionary[value.Type]));
    }
}
