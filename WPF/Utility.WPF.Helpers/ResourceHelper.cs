using System;
using System.Reflection;
using System.Windows;

namespace Utility.WPF.Helpers
{
    public static class ResourceHelper
    {

        public static T Load<T>(this Uri uri, string key) where T : class
        {
            ResourceDictionary res;
            if (Application.Current.Resources.Contains(uri) == false)
            {
                Application.Current.Resources.Add(uri, res = Application.LoadComponent(uri) as ResourceDictionary);
            }
            else
            {
                res = Application.Current.Resources[uri] as ResourceDictionary;
            }

            return res[key] as T;
        }


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

        public static TTarget? FindResource<TTarget>(string key)
        {
            if (!string.IsNullOrEmpty(key) && Application.Current != null)
            {
                return Application.Current.TryFindResource(key) is TTarget ? (TTarget)Application.Current.TryFindResource(key) : default;
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
            return Application.Current?.TryFindResource(key) is DataTemplate template ? template : default;
        }

        public static DataTemplate? FindTemplate(string key)
        {
            return Application.Current?.TryFindResource(key) is DataTemplate template ? template : default;
        }

        public static DataTemplate? FindTemplate(this DependencyObject dependencyObject, DataTemplateKey key)
        {
            if (dependencyObject is FrameworkElement element)
                return element.TryFindResource(key) is DataTemplate target ? target : default;
            return FindTemplate(key);
        }
    }
}