using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Utility.Conversions.Json.Newtonsoft;

public enum StringTypeEnumConverterBehavior
{
    AlwaysUseTypeNameInValue,                // writer always writes "TypeName.Value"
    UseTypeNameInValueForStrictEnumsOnly,    // writer writes type name only when declared type is Enum (non-generic Enum marker)
    NeverUseTypeNameInValue                  // never include type name on write
}

public sealed class StringTypeEnumConverter : JsonConverter
{
    private readonly StringEnumConverter _enumConverter = new StringEnumConverter();

    private readonly StringTypeEnumConverterBehavior _behavior;
    private readonly bool _inferTypeFromValue;
    private Service _service;

    /// <summary>
    /// Default: will search loaded assemblies. Behavior defaults to AlwaysUseTypeNameInValue and inference enabled.
    /// </summary>
    public StringTypeEnumConverter(
        IEnumerable<Assembly>? assemblies = null,
        StringTypeEnumConverterBehavior behavior = StringTypeEnumConverterBehavior.AlwaysUseTypeNameInValue,
        IEnumerable<Type>? knownEnumTypes = null,
        bool inferTypeFromValue = true)
    {
        _service = new(assemblies, knownEnumTypes);

        _behavior = behavior;
        _inferTypeFromValue = inferTypeFromValue;

    }

    public override bool CanConvert(Type objectType)
    {
        var t = objectType.GetNonNullable();
        return t.IsEnum || t == typeof(Enum); // allow when declared as Enum (loose)
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var targetType = objectType.GetNonNullable();

        if (reader.TokenType == JsonToken.String)
        {
            var s = (string)reader.Value!;

            // If string has form "TypeName.Value" and inference is allowed, try to find the type
            if (_inferTypeFromValue && s.TrySplitTypeValueString(out var typeName, out var literal))
            {
                Type foundType = null;
                if(objectType!=null)
                {
                    foundType = objectType;
                    _service.TypeCache.TryAdd(typeName, foundType);
                }
                // find type
                else if (_service.TypeCache.TryGetValue(typeName, out var _foundType) == false)
                {
                    foundType = _service.FindTypeByName(typeName);
                    if (foundType != null)
                        _service.TypeCache.TryAdd(typeName, foundType);
                }

                if (foundType != null)
                {
                    if (!foundType.IsEnum)
                        throw new JsonSerializationException($"Type '{typeName}' is not an enum.");

                    // Use Enum.Parse (case-sensitive by default like Enum.IsDefined expectation)
                    try
                    {
                        return Enum.Parse(foundType, literal, ignoreCase: false);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new JsonSerializationException($"Value '{literal}' is not a member of enum '{foundType.FullName}'.", ex);
                    }
                }

                // If we couldn't find the type but the declared target is an enum, try parse using declared target
                if (targetType.IsEnum)
                {
                    targetType.EnsureEnumValueExists(literal);
                    return Enum.Parse(targetType, literal, ignoreCase: false);
                }

                // No type found and target isn't an enum: fall through to error below
                throw new JsonSerializationException($"Could not resolve enum type '{typeName}'.");
            }

            // If no type prefix, or inference disabled, parse using the declared target type
            if (targetType.IsEnum)
            {
                targetType.EnsureEnumValueExists(s);
                return Enum.Parse(targetType, s, ignoreCase: false);
            }

            // If declared type is Enum (loose), we could attempt to infer by searching assemblies for an enum with that literal
            if (targetType == typeof(Enum) && _inferTypeFromValue)
            {
                // try to find a matching enum type containing the literal
                var found = _service.FindEnumTypeContainingLiteral(s);
                if (found != null)
                    return Enum.Parse(found, s, ignoreCase: false);
            }

            throw new JsonSerializationException($"Cannot convert string '{s}' to target type {objectType}.");
        }

        // Delegate to StringEnumConverter for other token types (numbers)
        return _enumConverter.ReadJson(reader, objectType, existingValue, serializer);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        Type runtimeType = value.GetType();
        var underlyingType = runtimeType.GetNonNullable();

        // let StringEnumConverter handle standard enum formatting, but we may need to prefix with type name
        if (underlyingType.IsEnum)
        {
            // Based on behavior, determine whether to emit TypeName.Value
            var includeTypeName = _behavior switch
            {
                StringTypeEnumConverterBehavior.AlwaysUseTypeNameInValue => true,
                StringTypeEnumConverterBehavior.UseTypeNameInValueForStrictEnumsOnly => writer.IsDeclaredAsLooseEnum(serializer),
                StringTypeEnumConverterBehavior.NeverUseTypeNameInValue => false,
                _ => false
            };

            if (includeTypeName)
            {
                var literal = Enum.GetName(underlyingType, value) ?? value.ToString();
                var typeName = _service.GetTypeNameFor(underlyingType);
                writer.WriteValue($"{typeName}.{literal}");
                return;
            }
        }

        // Fall back to default string enum converter
        _enumConverter.WriteJson(writer, value, serializer);
    }



}

public static class EnumerableExtensions
{
    /// <summary>
    /// Creates a HashSet<T> from an IEnumerable<T> using the default equality comparer.
    /// </summary>
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new HashSet<T>(source);
    }

    /// <summary>
    /// Creates a HashSet<T> from an IEnumerable<T> using the specified equality comparer.
    /// </summary>
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new HashSet<T>(source, comparer);
    }
}

public class Service
{
    private readonly IEnumerable<Assembly> _assemblies;
    private readonly ConcurrentDictionary<Type, string> _typeNameCache = new();
    private readonly HashSet<string> _knownTypeNames;

    public Service(IEnumerable<Assembly>? assemblies = null, IEnumerable<Type>? knownEnumTypes = null)
    {
        _assemblies = (assemblies ?? AppDomain.CurrentDomain.GetAssemblies()).Where(a => !a.IsDynamic);
        _knownTypeNames = knownEnumTypes?.Select(a => a.Name).ToHashSet() ?? new HashSet<string>();

        if (knownEnumTypes != null)
        {
            foreach (var t in knownEnumTypes)
                TypeCache.TryAdd(t.Name, t);
        }
    }

    public ConcurrentDictionary<string, Type> TypeCache { get; } = new();

    public Type? FindTypeByName(string typeName)
    {
        if (_knownTypeNames.Contains(typeName) && TypeCache.TryGetValue(typeName, out var t))
            return t;

        // Try Type.GetType first (handles assembly qualified names)
        var maybe = Type.GetType(typeName, throwOnError: false, ignoreCase: false);
        if (maybe != null)
            return maybe;

        // Search provided assemblies for matching simple name
        foreach (var asm in _assemblies)
        {
            try
            {
                var match = asm.GetTypes().FirstOrDefault(x => x.Name == typeName);
                if (match != null)
                    return match;
            }
            catch (ReflectionTypeLoadException)
            {
                // ignore assemblies that can't be fully inspected
            }
        }

        return null;
    }


    public Type? FindEnumTypeContainingLiteral(string literal)
    {
        foreach (var asm in _assemblies)
        {
            try
            {
                foreach (var t in asm.GetTypes().Where(x => x.IsEnum))
                {
                    if (Enum.IsDefined(t, literal))
                        return t;
                }
            }
            catch (ReflectionTypeLoadException)
            {
                // skip assemblies that fail to load some types
            }
        }
        return null;
    }
    public string GetTypeNameFor(Type t)
    {
        if (_typeNameCache.TryGetValue(t, out var name))
            return name;
        name = t.Name;
        _typeNameCache.TryAdd(t, name);
        return name;
    }
}

public static class Extensions
{

    public static Type GetNonNullable(this Type t) =>
        (Nullable.GetUnderlyingType(t) ?? t);

    public static bool TrySplitTypeValueString(this string s, out string typeName, out string literal)
    {
        typeName = string.Empty;
        literal = string.Empty;
        if (string.IsNullOrWhiteSpace(s))
            return false;

        var idx = s.IndexOf('.');
        if (idx <= 0 || idx == s.Length - 1)
            return false;

        typeName = s.Substring(0, idx);
        literal = s.Substring(idx + 1);
        return true;
    }



    public static void EnsureEnumValueExists(this Type enumType, string literal)
    {
        if (!Enum.IsDefined(enumType, literal))
            throw new JsonSerializationException($"Value '{literal}' is not part of enum '{enumType.FullName}'.");
    }


    /// <summary>
    /// Helper to reason about writer path and serializer if caller configured 'Write when declared is Enum' behavior.
    /// We attempt to detect whether the write is for a 'declared-as-Enum' property by walking serializer's internal stack via reflection.
    /// If we cannot determine, we conservatively return false.
    /// </summary>
    public static bool IsDeclaredAsLooseEnum(this JsonWriter writer, JsonSerializer serializer)
    {
        try
        {
            // Use same approach you had previously but with safe null checks and minimal reflection
            var getInternal = serializer.GetType().GetMethod("GetInternalSerializer", BindingFlags.Instance | BindingFlags.NonPublic);
            if (getInternal == null) return false;
            var internalSerializer = getInternal.Invoke(serializer, null);
            if (internalSerializer == null) return false;

            var stackField = internalSerializer.GetType().GetField("_serializeStack", BindingFlags.Instance | BindingFlags.NonPublic);
            if (stackField == null) return false;

            var list = stackField.GetValue(internalSerializer) as System.Collections.IList;
            if (list == null || list.Count == 0) return false;

            var current = list[list.Count - 1]; // top of stack
            // Walk writer.Path to find the property info (best-effort)
            var pathParts = writer.Path?.Split('.') ?? Array.Empty<string>();
            var curType = current.GetType();
            foreach (var part in pathParts)
            {
                var prop = curType.GetProperty(part, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop == null) break;
                curType = prop.PropertyType;
            }

            var declType = Nullable.GetUnderlyingType(curType) ?? curType;
            return declType == typeof(Enum); // someone declared it as System.Enum (rare)
        }
        catch
        {
            return false;
        }
    }
}