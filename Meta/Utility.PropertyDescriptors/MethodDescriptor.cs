
using Splat;

namespace Utility.Descriptors;

internal record MethodDescriptor : MemberDescriptor, IMethodDescriptor, IChildren
{
    Dictionary<int, object?> dictionary = new();

    private Lazy<Command> command;
    private readonly MethodInfo methodInfo;
    private readonly object instance;

    internal MethodDescriptor(MethodInfo methodInfo, object instance) : base((System.Type)null)
    {
        command = new Lazy<Command>(() => new Command(() =>
        {
            methodInfo.Invoke(instance, dictionary.OrderBy(a => a.Key).Select(a => a.Value).ToArray());
        }));
        this.methodInfo = methodInfo;
        this.instance = instance;
    }

    public object? this[int key]
    {
        get { return dictionary[key]; }
        set { dictionary[key] = value; }
    }

    public override string? Name => methodInfo.Name;

    public override Type ParentType => methodInfo.DeclaringType;

    public override object Get()
    {
        throw new NotImplementedException();
    }
    public override void Set(object value)
    {
        throw new NotImplementedException();
    }


    public void Invoke()
    {
        methodInfo.Invoke(instance, dictionary.OrderBy(a => a.Key).Select(a => a.Value).ToArray());
    }

    public override void Initialise(object? item = null)
    {
        throw new NotImplementedException();
    }

    public override void Finalise(object? item = null)
    {
        throw new NotImplementedException();
    }

    public ICommand Command => command.Value;

    public override bool IsReadOnly => true;

    public IObservable<object> Children
    {
        get
        {
            return Observable.Create<Change<IDescriptor>>(async observer =>
            {
                var descriptors = methodInfo
                .GetParameters()
                .Select(a => new ParameterDescriptor(a, dictionary)).ToArray();


                foreach (var paramDescriptor in descriptors)
                {
                    var guid = await Locator.Current.GetService<ITreeRepository>().Find(this.Guid, paramDescriptor.Name, paramDescriptor.Type);
                    paramDescriptor.Guid = guid;
                    dictionary[paramDescriptor.ParameterInfo.Position] = GetValue(paramDescriptor.ParameterInfo);
                    observer.OnNext(new Change<IDescriptor>(paramDescriptor, Changes.Type.Add));
                }
                return Disposable.Empty;
            });

            static object? GetValue(ParameterInfo a)
            {
                return a.HasDefaultValue ? a.DefaultValue : AlternateValue(a);

                static object? AlternateValue(ParameterInfo a)
                {
                    if (a.ParameterType.IsValueType || a.ParameterType.GetConstructor(System.Type.EmptyTypes) != null)
                        return Activator.CreateInstance(a.ParameterType);
                    return null;
                }
            }
        }
    }
}