using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Baml2006;

namespace Utility.WPF.Helper
{
    public static class ResourceDictionaryExtensions
    {
        public static IEnumerable<(DictionaryEntry entry, ResourceDictionary resourceDictionary)>
            SelectResourceDictionaries(this Assembly assembly, Predicate<DictionaryEntry>? predicate = null, bool ignoreXamlReaderExceptions = false)
        {
            // Only interested in main resource file
            return GetResourceNames().SelectMany(GetDictionaries);

            IEnumerable<(DictionaryEntry, ResourceDictionary)> GetDictionaries(string resourceName)
            {
                Stream? resourceStream = assembly.GetManifestResourceStream(resourceName);
                if (resourceStream == null)
                    throw new Exception("dsf33211..33");
                using (ResourceReader reader = new ResourceReader(resourceStream))
                {
                    var entries = GetDictionaryEntries(reader);
                    foreach (DictionaryEntry entry in entries)
                    {
                        if (predicate?.Invoke(entry) == false)
                            continue;

                        ResourceDictionary dictionary;
                        var readStream = entry.Value as Stream;
                        Baml2006Reader bamlReader = new Baml2006Reader(readStream);
                        ResourceDictionary? loadedObject = null;
                        try
                        {
                            loadedObject = System.Windows.Markup.XamlReader.Load(bamlReader) as ResourceDictionary;
                        }
                        catch (Exception ex)
                        {
                            if (ignoreXamlReaderExceptions == false)
                                throw;
                        }

                        if (loadedObject != null)
                        {
                            dictionary = loadedObject;
                        }
                        else
                        {
                            continue;
                        }
                        yield return (entry, dictionary);
                    }
                }
            }

            DictionaryEntry[] GetDictionaryEntries(ResourceReader reader)
            {
                var entries = reader.OfType<DictionaryEntry>()
                   // only interested in baml(xaml) files not images or similar
                   .Where(entry => entry.Key.ToString()?.EndsWith("baml") == true &&
                                   entry.Key.ToString()?.ToLowerInvariant().Contains("generic") != true)
                   .ToArray();
                return entries;
            }

            IEnumerable<string> GetResourceNames()
            {
                IEnumerable<string> allNames = assembly.GetManifestResourceNames();
                string[] resourceNames = assembly.GetManifestResourceNames().Where(a => a.EndsWith("g.resources")).ToArray();
                foreach (string resourceName in resourceNames)
                {
                    ManifestResourceInfo? info = assembly.GetManifestResourceInfo(resourceName);
                    if (info?.ResourceLocation != ResourceLocation.ContainedInAnotherAssembly)
                    {
                        yield return resourceName;
                    }
                }
            }
        }

        public static ResourceDictionary? FirstMatch(this IEnumerable<ResourceDictionary> dictionaries, Uri source)
        {
            // Use forach over linq!
            foreach (var dictionary in dictionaries)
            {
                if (dictionary.FindDictionary(source) is { } ss)
                    return ss;
            }
            return null;
        }

        public static void ReplaceDictionary(this ResourceDictionary resourceDictionary, Uri source, ResourceDictionary destination)
        {
            resourceDictionary.BeginInit();

            resourceDictionary.MergedDictionaries.Add(destination);

            ResourceDictionary? oldResourceDictionary = resourceDictionary.MergedDictionaries
                .FirstOrDefault(x => x.Source == source);
            if (oldResourceDictionary != null)
            {
                resourceDictionary.MergedDictionaries.Remove(oldResourceDictionary);
            }

            resourceDictionary.EndInit();
        }

        public static void ReplaceDictionary(this ResourceDictionary resourceDictionary, Uri source, Uri destination)
        {
            resourceDictionary.BeginInit();

            if (!resourceDictionary.MergedDictionaries.Any(x => x.Source == destination))
            {
                resourceDictionary.MergedDictionaries.Add(
                    new ResourceDictionary()
                    {
                        Source = destination
                    });
            }

            ResourceDictionary? oldResourceDictionary = resourceDictionary.MergedDictionaries
                .FirstOrDefault(x => x.Source == source);
            if (oldResourceDictionary != null)
            {
                resourceDictionary.MergedDictionaries.Remove(oldResourceDictionary);
            }

            resourceDictionary.EndInit();
        }

        /// <summary>
        /// Find the resource dictionary by recursively looking in the merged dictionaries
        /// Throw an exceptionReturn if the dictionary could not be found
        /// </summary>
        public static ResourceDictionary? FindDictionary(this ResourceDictionary resourceDictionary, Uri source)
        {
            // If this is the resource return it
            if (resourceDictionary.Source != null && resourceDictionary.Source == source)
            {
                return resourceDictionary;
            }

            // Search the merged-resource dictionaries
            var foundDictionary = resourceDictionary.MergedDictionaries
                .Select(mergedResource => mergedResource.FindDictionary(source))
                .FirstOrDefault();

            return foundDictionary;
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="resourceDictionary">
        /// The resource dictionary.
        public static bool ContainsDictionary(this ResourceDictionary resourceDictionary, ResourceDictionary resource)
        {
            if (resource.Source == null)
            {
                return false;
            }

            var foundDictionary = resourceDictionary.FindDictionary(resource.Source);
            return foundDictionary != null;
        }

        /// <summary>
        /// Determines if the specified resource dictionary (source) exists anywhere in the
        /// resource dictionary recursively.
        /// </summary>
        public static bool ContainsDictionary(this ResourceDictionary resourceDictionary, Uri source)
        {
            if (string.IsNullOrEmpty(source.LocalPath))
            {
                return false;
            }

            var foundDictionary = resourceDictionary.FindDictionary(source);
            return foundDictionary != null;
        }

        /// <summary>
        /// The find resource.
        /// </summary>
        public static object? FindResource(this ResourceDictionary resourceDictionary, object resourceKey)
        {
            // Try and find the resource in the root dictionary first
            var value = resourceDictionary[resourceKey];
            if (value != null)
            {
                return value;
            }

            // Then try the merged dictionaries
            var foundResource = resourceDictionary.MergedDictionaries
                                    .Select(mergedDictionary => mergedDictionary.FindResource(resourceKey))
                                    .FirstOrDefault(resource => resource != null);
            return foundResource;
        }

        #region Private Methods

        //private static Uri GetSource(ResourceDictionary resourceDictionary)
        //{
        //    SharedResourceDictionary sharedResourceDictionary = resourceDictionary as SharedResourceDictionary;
        //    if (sharedResourceDictionary != null)
        //    {
        //        return sharedResourceDictionary.Source;
        //    }

        //    return resourceDictionary.Source.ToString();
        //}

        #endregion Private Methods
    }
}