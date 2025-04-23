using System.Text.RegularExpressions;

namespace Pather.CSharp.PathElements
{
    public class ValuesEnumerableFactory : IPathElementFactory
    {
        public IPathElement Create(string path, out string newPath)
        {
            var matches = Regex.Matches(path, @"^(\$values\[(\d+)\])");
            Match match = matches[0];
            int index = int.Parse(match.Groups[2].Value);
            newPath = path.Remove(0, match.Value.Length);
            return new Enumerable(index);
        }

        public bool IsApplicable(string path)
        {
            return Regex.IsMatch(path, @"^(\$values\[\d+\])");
        }
    }
}
