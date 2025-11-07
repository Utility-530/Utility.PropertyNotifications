using System.Text.RegularExpressions;

namespace Pather.CSharp.PathElements
{
    public class PropertyFactory : IPathElementFactory
    {
        public virtual IPathElement Create(string path, out string newPath)
        {
            string property = Regex.Matches(path, @"^\w+")[0].Value;
            newPath = path.Remove(0, property.Length);
            return new Property(property);
        }

        public bool IsApplicable(string path)
        {
            return Regex.IsMatch(path, @"^\w+");
        }
    }
}