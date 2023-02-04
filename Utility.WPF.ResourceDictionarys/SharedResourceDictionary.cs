using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using Utility.WPF.Helper;

namespace Utility.WPF.ResourceDictionarys
{
    /// <summary>
    /// Loads singleton instance of ResourceDictionary to current scope
    /// </summary>
    /// <remarks>
    /// From Elysium
    /// </remarks>
    public class SharedResourceDictionary : ResourceDictionary
    {
        private static readonly ResourceDictionaryCollection SharedResources = new();
        private Uri? source;

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public new Uri? Source
        {
            get => DesignModeHelper.IsInDesignMode ? base.Source : source;
            set
            {
                if (source != value)
                {
                    source = value;
                    if (value != null)
                    {
                        var rd = SharedResources.Get(value);
                        AddToMergedDictionaries(rd);
                    }
                    base.Source = source;
                }
            }
        }

        protected void AddToMergedDictionaries(ResourceDictionary dictionary)
        {
            if (dictionary.Keys.Count == 0)
                return;
            MergedDictionaries.Add(dictionary);
            if (MergedDictionaries.Distinct(new ResourceDictionaryEqualityComparer()).Count() != MergedDictionaries.Count)
            {
                throw new Exception("£444");
            }
        }

        private class ResourceDictionaryEqualityComparer : IEqualityComparer<ResourceDictionary>
        {
            public bool Equals(ResourceDictionary? x, ResourceDictionary? y)
            {
                var keys = x.Keys.OfType<string>();
                var keys2 = y.Keys.OfType<string>();

                var z1 = x.Keys.OfType<DataTemplateKey>().Select(a => (a.DataType as Type)?.FullName?.ToString());
                var z = y.Keys.OfType<DataTemplateKey>().Select(a => (a.DataType as Type)?.FullName?.ToString());

                var dd = keys.SequenceEqual(keys2);
                var dde = z1.SequenceEqual(z);
                var bb = (keys.Any() && keys2.Any() || z.Any() && z1.Any()) && dd && dde;

                if (bb)
                {
                }
                return bb;
            }

            public int GetHashCode([DisallowNull] ResourceDictionary obj)
            {
                return obj.Keys.Count;
            }
        }

        /// <summary>
        /// The is in application scope.
        /// </summary>
        private bool IsInApplicationScope(ResourceDictionary? resource)
        {
            if (resource == null || resource.Source == null)
            {
                return false;
            }
            // Try and find the resource dictionary in the application scope
            return Application.Current != null && Application.Current.Resources.ContainsDictionary(resource.Source);
        }

        public override string? ToString()
        {
            return source?.ToString() ?? "none" + " " + string.Join(',', Keys);
        }
    }

    internal class ResourceDictionaryCollection : DictionaryCollection<ResourceDictionary>
    {
        public ResourceDictionaryCollection() : base(
           source => new ResourceDictionary { Source = source },
           (source, select) => select.FirstMatch(source))
        {
        }

        /// <summary>
        /// Find the resource dictionary by recursively looking in the merged dictionaries
        /// Return null if the dictionary could not be found
        /// </summary>
        public ResourceDictionary? FindResourceDictionary(Uri source)
        {
            return Select()
               .Select(dictionary => dictionary.FindDictionary(source))
               .FirstOrDefault();
        }

        ///// <summary>
        ///// The find resource.
        ///// </summary>
        //public object FindResource(object resourceKey)
        //{
        //    var foundResource = TryFindResource(resourceKey);
        //    if (foundResource == null)
        //    {
        //        throw new Exception($"Could not find resource with resourceKey {resourceKey} in SharedResourceDictionary");
        //    }
        //    return foundResource;
        //}

        ///// <summary>
        ///// The try find resource.
        ///// </summary>
        //public object? TryFindResource(object resourceKey)
        //{
        //    var foundResource = Select()
        //       .Select(resourceDictionary => resourceDictionary.FindResource(resourceKey))
        //       .FirstOrDefault();
        //    return foundResource;
        //}
    }

    internal class DictionaryCollection<T>
    {
        private readonly Func<Uri, T> createFunc;
        private readonly Func<Uri, IEnumerable<T>, T?> find;
        private readonly Dictionary<Uri, T> SharedResources = new Dictionary<Uri, T>();

        public DictionaryCollection(Func<Uri, T> createFunc, Func<Uri, IEnumerable<T>, T?> find)
        {
            this.createFunc = createFunc;
            this.find = find;
        }

        /// <summary>
        /// Return all resource dictionaries that are in memory.
        /// </summary>
        public IEnumerable<T> Select()
        {
            return SharedResources.Select(kvp => kvp.Value);
        }

        /// <summary>
        /// Get the resource dictionary specified by the source uri.
        /// If the dictionary is not loaded add a weak reference to the list
        /// </summary>
        public T Get(Uri source)
        {
            // Return the resource dictionary if it could be found
            var foundDictionary = find(source, Select());
            if (foundDictionary != null)
            {
                return foundDictionary;
            }

            if (SharedResources.ContainsKey(source) == false)
            {
                var newDictionary = createFunc(source);
                SharedResources.Add(source, newDictionary);
                return newDictionary;
            }
            throw new Exception("sdf33 3233");
        }
    }
}