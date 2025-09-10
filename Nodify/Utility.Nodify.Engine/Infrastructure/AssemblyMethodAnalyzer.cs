using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class AssemblyMethodAnalyzer
{
    /// <summary>
    /// Enumerates all public static methods in an assembly, filters by parameter types, and groups by class name
    /// </summary>
    /// <param name="assembly">The assembly to analyze</param>
    /// <param name="parameterTypeFilter">Optional filter for parameter types. If null, no filtering is applied.</param>
    /// <returns>Dictionary where key is class name and value is list of matching methods</returns>
    public static Dictionary<string, List<MethodInfo>> GetPublicStaticMethodsGroupedByClass(this
        Assembly assembly,
        Func<Type[], bool> parameterTypeFilter = null)
    {
        return assembly
            .GetTypes()
            .Where(type => type.IsClass && type.IsPublic)
            .SelectMany(type => type
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(method => parameterTypeFilter == null ||
                               parameterTypeFilter(method.GetParameters().Select(p => p.ParameterType).ToArray()))
                .Select(method => new { Type = type, Method = method }))
            .GroupBy(x => x.Type.Name)
            .ToDictionary(
                group => group.Key,
                group => group.Select(x => x.Method).ToList()
            );
    }

    /// <summary>
    /// Alternative method with more detailed filtering options
    /// </summary>
    /// <param name="assembly">The assembly to analyze</param>
    /// <param name="exactParameterTypes">Exact parameter types to match (in order)</param>
    /// <param name="containsParameterType">Parameter type that must be present (anywhere in parameters)</param>
    /// <param name="parameterCount">Specific number of parameters to match</param>
    /// <returns>Dictionary where key is full class name and value is list of matching methods</returns>
    public static Dictionary<string, List<MethodInfo>> GetFilteredPublicStaticMethods(this
        Assembly assembly,
        Type[] exactParameterTypes = null,
        Type containsParameterType = null,
        int? parameterCount = null)
    {
        return assembly
            .GetTypes()
            .Where(type => type.IsClass && type.IsPublic)
            .SelectMany(type => type
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(method => MatchesFilter(method, exactParameterTypes, containsParameterType, parameterCount))
                .Select(method => new { Type = type, Method = method }))
            .GroupBy(x => x.Type.FullName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(x => x.Method).ToList()
            );
    }

    private static bool MatchesFilter(MethodInfo method, Type[] exactParameterTypes, Type containsParameterType, int? parameterCount)
    {
        var parameters = method.GetParameters();
        var paramTypes = parameters.Select(p => p.ParameterType).ToArray();

        // Check parameter count if specified
        if (parameterCount.HasValue && parameters.Length != parameterCount.Value)
            return false;

        // Check exact parameter types if specified
        if (exactParameterTypes != null)
        {
            if (paramTypes.Length != exactParameterTypes.Length)
                return false;

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (!paramTypes[i].Equals(exactParameterTypes[i]))
                    return false;
            }
        }

        // Check if contains specific parameter type if specified
        if (containsParameterType != null && !paramTypes.Contains(containsParameterType))
            return false;

        return true;
    }

    /// <summary>
    /// Get detailed method information with parameter details
    /// </summary>
    public static Dictionary<string, List<MethodDetails>> GetDetailedMethodInfo(Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(type => type.IsClass && type.IsPublic)
            .SelectMany(type => type
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Select(method => new MethodDetails
                {
                    ClassName = type.Name,
                    FullClassName = type.FullName,
                    MethodName = method.Name,
                    ReturnType = method.ReturnType,
                    Parameters = method.GetParameters()
                        .Select(p => new ParameterDetails
                        {
                            Name = p.Name,
                            Type = p.ParameterType,
                            IsOptional = p.IsOptional,
                            DefaultValue = p.IsOptional ? p.DefaultValue : null
                        }).ToList(),
                    Method = method
                }))
            .GroupBy(x => x.ClassName)
            .ToDictionary(
                group => group.Key,
                group => group.ToList()
            );
    }
}

public class MethodDetails
{
    public string ClassName { get; set; }
    public string FullClassName { get; set; }
    public string MethodName { get; set; }
    public Type ReturnType { get; set; }
    public List<ParameterDetails> Parameters { get; set; }
    public MethodInfo Method { get; set; }

    public override string ToString()
    {
        var paramStr = string.Join(", ", Parameters.Select(p => $"{p.Type.Name} {p.Name}"));
        return $"{ReturnType.Name} {MethodName}({paramStr})";
    }
}

public class ParameterDetails
{
    public string Name { get; set; }
    public Type Type { get; set; }
    public bool IsOptional { get; set; }
    public object DefaultValue { get; set; }
}

// Example usage class
public static class ExampleUsage
{
    public static void DemonstrateUsage()
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Example 1: Get all public static methods with string parameters
        var methodsWithStringParams = AssemblyMethodAnalyzer.GetPublicStaticMethodsGroupedByClass(
            assembly,
            paramTypes => paramTypes.Any(t => t == typeof(string))
        );

        // Example 2: Get methods with exactly 2 parameters
        var methodsWith2Params = AssemblyMethodAnalyzer.GetFilteredPublicStaticMethods(
            assembly,
            parameterCount: 2
        );

        // Example 3: Get methods with exact parameter signature (string, int)
        var stringIntMethods = AssemblyMethodAnalyzer.GetFilteredPublicStaticMethods(
            assembly,
            exactParameterTypes: new[] { typeof(string), typeof(int) }
        );

        // Example 4: Get detailed information about all methods
        var detailedInfo = AssemblyMethodAnalyzer.GetDetailedMethodInfo(assembly);

        // Print results
        foreach (var classGroup in methodsWithStringParams)
        {
            Console.WriteLine($"Class: {classGroup.Key}");
            foreach (var method in classGroup.Value)
            {
                var paramStr = string.Join(", ",
                    method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                Console.WriteLine($"  {method.ReturnType.Name} {method.Name}({paramStr})");
            }
        }
    }
}