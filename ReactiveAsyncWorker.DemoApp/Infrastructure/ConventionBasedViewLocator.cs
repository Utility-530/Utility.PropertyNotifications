#nullable enable

using ReactiveUI;
using System;
using System.Linq;
using System.Reflection;
using Splat;
using MoreLinq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Endless.Functional;

namespace ReactiveAsyncWorker.DemoApp.Infrastructure
{
    public static class ConventionBasedViewLocatorHelper
    {
        public static ConventionBasedViewLocator ToConventionBased(this IViewLocator viewLocator, Type defaultViewType, Type[] viewTypes)
        {
            return new ConventionBasedViewLocator(viewLocator, viewTypes, defaultViewType);
        }
    }

    public sealed partial class ConventionBasedViewLocator : IViewLocator
    {
        private static readonly Type IViewForType = typeof(IViewFor);
        private readonly IViewLocator deferTo;
        private readonly Type? defaultViewType;
        private readonly Lazy<TypeItem[]> viewTypes;
        private readonly Lazy<Regex> regex = new Lazy<Regex>(() => new Regex(@"([^\`]*)(\`\d)?"));

        public ConventionBasedViewLocator(IViewLocator deferTo, Type[]? viewTypes = null, Type? defaultViewType = null)
        {
            if (defaultViewType != null && typeof(IViewFor<object>).IsAssignableFrom(defaultViewType) == false)
                throw new Exception($"{nameof(defaultViewType)} is not null, yet {nameof(IViewFor<object>)} is not assignable from it.\n" +
                    $"Check that the {nameof(defaultViewType)} parameter is a {nameof(ReactiveUserControl<object>)} or something similar");
            this.viewTypes = new Lazy<TypeItem[]>(() => GetTypes(viewTypes).ToArray());
            this.deferTo = deferTo;
            this.defaultViewType = defaultViewType;

            static IEnumerable<TypeItem> GetTypes(Type[]? types)
            {
                return (types?.SelectMany(a => a.Assembly.GetTypes()) ??
                            Assembly.GetExecutingAssembly().GetTypes().SelectMany(a => a.Assembly.GetTypes()))
                            .Concat(typeof(ConventionBasedViewLocator).Assembly.GetTypes())
                            .DistinctBy(a => a.AssemblyQualifiedName)
                            .Where(a => a.IsAssignableTo(IViewForType))
                            .Select(a => new TypeItem(a));
            }
        }


        /// <summary>
        /// Resolves views by first using the default Splat view-locator to resolve
        /// then looking for a type with a matching name among the types it aware of with
        /// the convention being
        /// <Name>ViewModel -> <Name>View
        /// and if <see cref="contract"/> is not null
        /// <Name>ViewModel -> <Name><see cref="contract"/>View
        /// then using a defaultViewType if one has been provided.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="contract"></param>
        /// <returns></returns>
        public IViewFor? ResolveView<T>(T viewModel, string? contract = null)
        {
            IViewFor? viewFor = null;

            if (deferTo.ResolveView(viewModel, contract) is { } deferViewFor)
                viewFor = deferViewFor;
            else if (GetViewType(viewModel?.GetType(), contract).ToArray() is { } tempViewTypes
                 && tempViewTypes.FirstOrDefault() is { } tempViewType
                 && Activator.CreateInstance(tempViewType) is IViewFor tempViewFor)
                viewFor = tempViewFor;
            else if (defaultViewType != null &&
                Activator.CreateInstance(defaultViewType) is IViewFor defaultViewFor)
                viewFor = defaultViewFor;

            var viewType = viewFor?.GetType().Name;
            var viewModelType = viewModel?.GetType().Name;

            if (viewType == null)
            {
                var views = viewTypes.Value.Where(a => a.Name.Contains("View", StringComparison.OrdinalIgnoreCase)).ToArray();
                //if(viewType == typeof(System).Name)
                //{
                //    this.Log().Write("Happens when thing are not loaded", LogLevel.Error);
                //}
                //var viewType =  typeof(IViewFor).IsAssignableFrom(viewType);
            }

            this.Log().Write($"{viewType ?? "No"} view returned for view-model {viewModelType}." + 
                (defaultViewType == null ? $"Try setting the {nameof(defaultViewType)} in the constructor of {nameof(ConventionBasedViewLocator)}." : string.Empty),
                typeof(ConventionBasedViewLocator), LogLevel.Info);

            return viewFor;

            IEnumerable<Type?> GetViewType(Type viewModelType, string? viewModelContract = null)
            {
                TypeItem? viewType;

                // To determine whether view and viewmodel match
                // use the key from the attribute if it exists or ...
                if (viewModelType.GetAttributeSafe<ViewModelAttribute>() is { } vma && vma.success)
                {
                    viewType = viewTypes.Value.SingleOrDefault(a => a.IsViewType && a.ViewAttributeKey == vma.attribute!.Key);
                }
                // use the first part of the name
                else
                {
                    // Remove generic related characters
                    var match = regex.Value.Match(viewModelType.FullName ?? throw new Exception("FullName doesn't exist!!!"));

                    var regexName = match.Groups[1].Value;

                    var viewTypeName = regexName.ReplaceLast("ViewModel", viewModelContract + "View").Split('.').Last();

                    var viewMatchingTypes = viewTypes.Value.Where(a => a.Name == viewTypeName).ToArray();

                    if (viewMatchingTypes.Length > 1)
                    {

                    }

                    viewType = viewMatchingTypes.SingleOrDefault();

                    if (viewType == null)
                    {
                        viewTypeName = (regexName + "View").Split('.').Last(); ;
                        viewType = viewTypes.Value.SingleOrDefault(a => a.Name == viewTypeName);
                    }

                    if ((viewType?.Equals(viewModelType) ?? true) && viewModelType?.BaseType != typeof(object))
                    {
                        foreach (var xx in GetViewType(viewModelType?.BaseType))
                            yield return xx;
                    }
                }

                if (viewType != null)
                    yield return viewType.Type;
            }
        }


        public class TypeItem : IEquatable<TypeItem?>, IEquatable<Type>
        {
            private readonly Lazy<string?> viewAttributeKey;
            private readonly Lazy<string?> viewModelAttributeKey;
            private readonly Lazy<bool> bootStrapperIgnore;
            private readonly Lazy<IViewFor?> view;

            public TypeItem(Type type)
            {
                Key = type.AssemblyQualifiedName;
                Name = type.Name;
                Type = type;
                viewAttributeKey = Helper.Create(() => type.GetAttributeSafe<ViewAttribute>().Pipe(a => a.success ? a.attribute!.Key : null));
                viewModelAttributeKey = Helper.Create(() => type.GetAttributeSafe<ViewModelAttribute>().Pipe(a => a.success ? a.attribute!.Key : null));
                bootStrapperIgnore = Helper.Create(() => type.GetAttributeSafe<BootStrapperIgnore>().Pipe(a => a.success));
                view = Helper.Create(() => Activator.CreateInstance(type) as IViewFor);
            }

            public string? Key { get; }

            public string Name { get; }

            public Type Type { get; }

            public bool IsViewType => viewAttributeKey.Value != null;

            public string? ViewAttributeKey => viewAttributeKey.Value;

            public bool IsViewModelType => viewModelAttributeKey.Value != null;

            public string? ViewModelAttributeKey => viewModelAttributeKey.Value;

            public bool BootStrapperIgnore => bootStrapperIgnore.Value;

            public IViewFor? View { get; }

            public override bool Equals(object? obj)
            {
                return Equals(obj as TypeItem);
            }

            public bool Equals(TypeItem? other)
            {
                return other != null &&
                       EqualityComparer<Lazy<IViewFor>>.Default.Equals(view, other.view);
            }

            public bool Equals(Type? other)
            {
                return other != null && Equals(new TypeItem(other));
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(view);
            }

            public static bool operator ==(TypeItem? left, TypeItem? right)
            {
                return EqualityComparer<TypeItem>.Default.Equals(left, right);
            }

            public static bool operator !=(TypeItem? left, TypeItem? right)
            {
                return !(left == right);
            }
        }

        public class BootStrapperIgnore : Attribute
        {

        }

        /// <summary>
        /// Used to link to viewmodels via <see cref="ViewModelAttribute"/>
        /// </summary>
        public class ViewAttribute : Attribute
        {
            public ViewAttribute(string key)
            {
                Key = key;
            }

            public string Key { get; }
        }

        /// <summary>
        /// Used to link to views via <see cref="ViewAttribute"/>
        /// </summary>
        public class ViewModelAttribute : Attribute
        {
            public ViewModelAttribute(string key)
            {
                Key = key;
            }

            public string Key { get; }
        }
    }


    public static class Helper
    {
        public static Lazy<T> Create<T>(Func<T> func)
        {
            return new Lazy<T>(func);
        }

        public static Lazy<T> ToLazy<T>(this Func<T> func)
        {
            return new Lazy<T>(func);
        }

        public static (bool success, T? attribute) GetAttributeSafe<T>(this MemberInfo value) where T : Attribute
        {
            T[] attributes = (T[])value.GetCustomAttributes(typeof(T), false);

            return attributes?.Length > 0 ? (true, attributes[0]) : (false, default)!;
        }
        public static string ReplaceLast(this string text, string search, string replace)
        {
            return ReplaceAtPosition(text, search, replace, text.LastIndexOf(search, StringComparison.Ordinal));
        }
        public static string ReplaceAtPosition(this string text, string search, string replace, int index) =>

      index >= 0 && search == text.Substring(index, search.Length) ?
              text.Substring(0, index) + replace + text.Substring(index + search.Length)
              : text;
        /// <summary>
        /// Used to link to views via <see cref="ViewAttribute"/>
        /// </summary>
        public class ViewModelAttribute : Attribute
        {
            public ViewModelAttribute(string key)
            {
                Key = key;
            }

            public string Key { get; }
        }
    }


}

