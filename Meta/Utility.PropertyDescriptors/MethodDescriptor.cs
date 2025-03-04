using Utility.Meta;

namespace Utility.PropertyDescriptors;

internal record MethodDescriptor : MemberDescriptor, IMethodDescriptor
{
    Dictionary<int, object?> dictionary = new();

    private Lazy<Command> command;
    private readonly MethodInfo methodInfo;
    private readonly object instance;

    internal MethodDescriptor(MethodInfo methodInfo, object instance) : base(new RootDescriptor(null, methodInfo.DeclaringType, methodInfo.Name))
    {
        this.methodInfo = methodInfo;
        this.instance = instance;
    }

    public object? this[int key]
    {
        get { return dictionary[key]; }
        set { dictionary[key] = value; }
    }

    public void Invoke()
    {
        methodInfo.Invoke(instance, dictionary.OrderBy(a => a.Key).Select(a => a.Value).ToArray());
    }

    public override bool IsReadOnly => true;

    public override IEnumerable<object> Children
    {
        get
        {

            var descriptors = methodInfo
            .GetParameters()
            .Select(a => new ParameterDescriptor(a, dictionary)).ToArray();

            foreach (var paramDescriptor in descriptors)
            {
                dictionary[paramDescriptor.ParameterInfo.Position] = GetValue(paramDescriptor.ParameterInfo);
                yield return paramDescriptor;
            }


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