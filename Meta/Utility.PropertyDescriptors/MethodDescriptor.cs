
namespace Utility.Descriptors;

public record MethodDescriptor : MemberDescriptor, IMethodDescriptor
{
    Dictionary<int, object?> dictionary = new();

    private Lazy<ObservableCommand> command;
    private readonly MethodInfo methodInfo;
    private readonly object instance;

    public MethodDescriptor(MethodInfo methodInfo, object instance): base((System.Type)null)
    {
        command = new Lazy<ObservableCommand>(() => new ObservableCommand(a =>
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

    public override IObservable<Change<IMemberDescriptor>> GetChildren()
    {
        return Observable.Create<Change<IMemberDescriptor>>(async observer =>
        {
            var descriptors = MethodExplorer.ParameterDescriptors(methodInfo, dictionary);
            foreach (var paramDescriptor in descriptors)
            {
                var guid = await GuidRepository.Instance.Find(this.Guid, paramDescriptor.Name, paramDescriptor.Type);
                paramDescriptor.Guid = guid;
                dictionary[paramDescriptor.ParameterInfo.Position] = GetValue(paramDescriptor.ParameterInfo);
                observer.OnNext(new Change<IMemberDescriptor>(paramDescriptor, Changes.Type.Add));
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

    public override object GetValue()
    {
        throw new NotImplementedException();
    }

    public void Invoke()
    {
        methodInfo.Invoke(instance, dictionary.OrderBy(a => a.Key).Select(a => a.Value).ToArray());
    }

    public ICommand Command => command.Value;

    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }
}

