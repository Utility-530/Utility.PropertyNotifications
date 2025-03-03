using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
