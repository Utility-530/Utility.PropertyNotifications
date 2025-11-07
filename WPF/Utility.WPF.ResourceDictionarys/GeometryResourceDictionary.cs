using System;

namespace Utility.WPF.ResourceDictionarys
{
    public class GeometryResourceDictionary : SharedResourceDictionary
    {
        public GeometryResourceDictionary()
        {
            Source = new Uri($"/Utility.WPF.Geometries;component/Resources.xaml", UriKind.RelativeOrAbsolute);
        }
    }
}