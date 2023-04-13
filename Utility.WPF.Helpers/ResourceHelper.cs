using System;
using System.Reflection;

namespace Utility.WPF
{
    public class ResourceHelper
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
    }
}