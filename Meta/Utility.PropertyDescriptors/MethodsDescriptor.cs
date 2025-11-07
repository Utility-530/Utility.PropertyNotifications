namespace Utility.PropertyDescriptors;

internal class MethodsDescriptor(Descriptor Descriptor, object Instance) : ReferenceDescriptor(Descriptor, Instance), IMethodsDescriptor
{
    public static readonly string? _Name = "Methods";

    public override string? Name => _Name;

    public override IEnumerable Items()
    {
        var children = MethodExplorer.MethodInfos(Descriptor.PropertyType).ToArray();
        return children.Select(methodInfo => DescriptorFactory.CreateMethodItem(Instance, methodInfo, Type));
    }
}

public static class MethodExplorer
{
    public static IEnumerable<MethodInfo> MethodInfos(Type type)
    {
        return Types(type)
        .SelectMany(t => t
        .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .Where(m => !m.IsSpecialName)
        .Where(m => true)
        .Cast<MethodInfo>()
        .OrderBy(d => d.Name));
    }

    private static IEnumerable<Type> Types(Type type)
    {
        if (type != typeof(object))
        {
            yield return type;
            if (type.BaseType is Type baseType)
                foreach (var t in Types(baseType))
                    yield return t;
        }
        else
            yield break;
    }
}