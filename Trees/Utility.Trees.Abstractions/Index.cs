using System.Text;

//

namespace Utility.Trees
{
    public record Index
    {
        public Index(params int[] indexes)
        {
            Collection = indexes;
        }

        public IReadOnlyCollection<int> Collection { get; init; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            if (Collection.Count == 0)
                return string.Empty;
            foreach (var item in Collection)
            {
                stringBuilder.Append(item);
                stringBuilder.Append('.');
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }
    }
}