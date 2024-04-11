using System;
using System.Reflection;
using System.Windows;

namespace Utility.WPF.Helpers
{
    public static class ResourceHelper
    {
        public static T FindResource<T>(string directory, string key)
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri(directory, UriKind.RelativeOrAbsolute);
            var path = resourceDictionary[key];
            return (T)path;
        }

        public static T FindRelativeResource<T>(string relativedirectory, string key)
        {
            var ass = Assembly.GetCallingAssembly().GetName();
            var resourceDictionary = new ResourceDictionary();
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
        public static TTarget? FindResource<TTarget>(string key)
        {
            if (!string.IsNullOrEmpty(key) && Application.Current != null)
            {
                object obj = Application.Current.TryFindResource(key);
                if (obj is TTarget)
                {
                    return (TTarget)obj;
                }

                return default;
            }

            return default;
        }

        public static TTarget? FindResource<TTarget>(this DependencyObject dependencyObject, string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (dependencyObject is FrameworkElement element)
                    return element.TryFindResource(key) is TTarget target ? target : default;
                return FindResource<TTarget>(key);
            }

            return default;
        }

        public static DataTemplate? FindTemplate(DataTemplateKey key)
        {
            if (Application.Current != null)
            {
                object obj = Application.Current.TryFindResource(key);
                if (obj is DataTemplate)
                {
                    return (DataTemplate)obj;
                }

                return default;
            }

            return default;
        }

        public static DataTemplate? FindTemplate(this DependencyObject dependencyObject, DataTemplateKey key)
        {
            if (dependencyObject is FrameworkElement element)
                return element.TryFindResource(key) is DataTemplate target ? target : default;
            return FindTemplate(key);
        }

    }
}