using System.Text;

//

namespace Utility.Trees
{
    public record Index
    {
        private IList<int>  collection;
        public Index(params int[] indexes)
        {
            collection = indexes;
        }

        public int this[int key]
        {
             get{ return collection[key];  }
            set { collection[key] = value; }

        }

        public bool IsEmpty => collection.Any() == false;

        //public IReadOnlyCollection<int> Collection { get; init; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            if (collection.Count == 0)
                return string.Empty;
            foreach (var item in collection)
            {
                stringBuilder.Append(item);
                stringBuilder.Append('.');
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }
    }
}