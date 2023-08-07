using System;
using System.Reflection;
using System.Windows;

namespace Utility.WPF
{
    public static class ResourceHelper
    {
        public static T FindResource<T>(string directory, string key)
        {
            var resourceDictionary = new System.Windows.ResourceDictionary();
            resourceDictionary.Source = new Uri(directory, UriKind.RelativeOrAbsolute);
            var path = resourceDictionary[key];
            return (T)path;
        }

        public static T FindRelativeResource<T>(string relativedirectory, string key)
        {
            var ass = Assembly.GetCallingAssembly().GetName();
            var resourceDictionary = new System.Windows.ResourceDictionary();
            resourceDictionary.Source = new Uri($"/{ass};component/{relativedirectory}", UriKind.RelativeOrAbsolute);
            var path = resourceDictionary[key];
            return (T)path;
        }

        // Summary:
        //     GetResource
        //
        // Parameters:
        //   _:
        //
        //   key:
        //     Resource Key
        //
        // Type parameters:
        //   Target:
        public static TTarget FindResource<TTarget>(this DependencyObject _, string key)
        {
            if (!string.IsNullOrEmpty(key) && Application.Current != null)
            {
                object obj = Application.Current?.TryFindResource(key);
                if (obj is TTarget)
                {
                    return (TTarget)obj;
                }

                return default(TTarget);
            }

            return default(TTarget);
        }
    }
}