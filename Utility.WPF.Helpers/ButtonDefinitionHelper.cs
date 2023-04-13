namespace Utility.WPF
{
    //class ButtonDefinition
    //{
    //public static class ButtonDefinitionHelper
    //{
    //public static IEnumerable<KeyValuePair<object, Func<object>>> GetCommandOutput(Type type, Type outputType = null, params object[] parameters)
    //{
    //    if (type.IsEnum)
    //    {
    //        return BuildFromEnum(type);
    //    }
    //    else if (type.IsInterface)
    //    {
    //        if (parameters?.All(a => a == null) == null)
    //            return type.LoadInterfaces(parameters);
    //        else
    //            return type.LoadInterfaces();
    //    }
    //    else if (type.Assembly.GetName().Name != "mscorlib")
    //    {
    //        // user-defined
    //        return LoadMethods(type, outputType.Name, parameters);
    //    }
    //    else
    //        throw new ArgumentException();
    //}

    //public static IEnumerable<KeyValuePair<object, Func<T>>> GetCommandOutput<T>(Type type = null, params object[] parameters)
    //{
    //    if (type.IsEnum)
    //    {
    //        return BuildFromEnum<T>();
    //    }
    //    else if (type.Assembly.GetName().Name != "mscorlib")
    //    {
    //        // user-defined
    //        return LoadMethods<T>(type, parameters);
    //    }
    //    else
    //        throw new ArgumentException();
    //}

    //public static IEnumerable<KeyValuePair<object, Func<object>>> BuildFromEnum(Type t)
    //{
    //    return Enum.GetValues(t).Cast<object>().Select(_ => new KeyValuePair<object, Func<object>>(_, () => _));
    //}

    //public static IEnumerable<KeyValuePair<object, Func<T>>> BuildFromEnum<T>()
    //{
    //    return Enum.GetValues(typeof(T)).Cast<T>().Select(e => new KeyValuePair<object, Func<T>>(e, () => e));
    //}

    //public static IEnumerable<KeyValuePair<object, Func<object>>> LoadMethods(this Type t, string TypeName, params object[] parameters)
    //{
    //    return Assembly.GetAssembly(t)
    //          .GetType(t.FullName)
    //            .GetMethods(BindingFlags.Public | BindingFlags.Static)
    //               // filter by return type
    //               .Where(a => a.ReturnType.Name == TypeName)
    //                .Select(_ => new KeyValuePair<object, Func<object>>(_.GetDescription(), () => _.Invoke(null, parameters)));
    //}

    //public static IEnumerable<KeyValuePair<object, Func<object>>> LoadInterfaces(this Type type, params object[] parameters)
    //{
    //    return AppDomain.CurrentDomain.GetAssemblies()
    //        .SelectMany(s => s.GetTypes())
    //        .Where(p => type.IsAssignableFrom(p) && type != p)
    //       .Select(_ => new KeyValuePair<object, Func<object>>(_.FullName, () => Activator.CreateInstance(_, parameters)));
    //}

    //public static IEnumerable<KeyValuePair<object, Func<T>>> LoadMethods<T>(this Type t, params object[] parameters)
    //{
    //    var typename = typeof(T).Name;
    //    return Assembly.GetAssembly(t)
    //          .GetType(t.Name)
    //            .GetMethods(BindingFlags.Public | BindingFlags.Static)
    //               // filter by return type
    //               .Where(a => a.ReturnType.Name == typename)
    //                .Select(_ => new KeyValuePair<object, Func<T>>(_.GetDescription(), () => (T)_.Invoke(null, parameters)));
    //}

    //public static String GetDescription(this MethodInfo methodInfo)
    //{
    //    try
    //    {
    //        object[] attribArray = methodInfo.GetCustomAttributes(false);

    //        if (attribArray.Length > 0)
    //        {
    //            var attrib = attribArray[0] as DescriptionAttribute;

    //            if (attrib != null)
    //                return attrib.Description;
    //        }
    //        return methodInfo.Name;
    //    }
    //    catch (NullReferenceException ex)
    //    {
    //        return "Unknown";
    //    }
    //}

    //public static IEnumerable<KeyValuePair<string, Action>> ToActions<T>(this IEnumerable<KeyValuePair<string, Func<T>>> kvps, Action<T> tr)
    //{
    //    foreach (var m in kvps)
    //    {
    //        yield return new KeyValuePair<string, Action>(
    //              m.Key,
    //            () => tr(m.Value())

    //        );
    //    }
    //}

    //public static Func<T, R> GetInstanceMethod<T, R>(MethodInfo method)
    //{
    //    //ParameterExpression x = Expression.Parameter(typeof(T), "it");
    //    return Expression.Lambda<Func<T, R>>(
    //        Expression.Call(null, method), null).Compile();
    //}
    //}

    //public static class ButtonDefinitionGeneric
    //{
    //    public static IEnumerable<KeyValuePair<string, Func<T>>> GetCommandOutput<T>(Type type, params object[] parameters)
    //    {
    //        if (type.IsEnum)
    //        {
    //            return BuildFromEnum<T>();
    //        }
    //        else if (type.Assembly.GetName().Name != "mscorlib")
    //        {
    //            // user-defined
    //            return LoadMethods<T>(type, parameters);
    //        }
    //        else
    //            throw new ArgumentException();
    //    }

    //    public static IEnumerable<KeyValuePair<string, Func<T>>> BuildFromEnum<T>()
    //    {
    //        return Enum.GetValues(typeof(T)).Cast<T>().Select(_ => new KeyValuePair<string, Func<T>>(_.ToString(), () => _));
    //    }

    //    public static IEnumerable<KeyValuePair<string, Func<T>>> LoadMethods<T>(this Type t, params object[] parameters)
    //    {
    //        return Assembly.GetAssembly(t)
    //              .GetType(t.FullName)
    //                .GetMethods(BindingFlags.Public | BindingFlags.Static)
    //                   // filter by return type
    //                   .Where(a => a.ReturnType.Name == typeof(T).Name)
    //                    .Select(_ => new KeyValuePair<string, Func<T>>(_.GetDescription(), () => (T)_.Invoke(null, parameters)));

    //    }

    //}
}