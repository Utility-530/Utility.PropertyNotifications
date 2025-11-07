using System.IO;
using System.Windows.Markup;
using System.Xml;

namespace Utility.WPF.Helpers
{
    public static class CloneHelper
    {
        public static T XamlClone<T>(T from)
        {
            string gridXaml = XamlWriter.Save(from);
            //Load it into a new object:
            StringReader stringReader = new(gridXaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            T newGrid = (T)XamlReader.Load(xmlReader);
            return newGrid;
        }
    }
}