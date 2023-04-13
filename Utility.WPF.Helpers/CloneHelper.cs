using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

        public static T DeepClone<T>(T from)
        {
            using (MemoryStream s = new())
            {
                BinaryFormatter f = new();
                f.Serialize(s, from);
                s.Position = 0;
                object clone = f.Deserialize(s);
                return (T)clone;
            }
        }
    }
}
