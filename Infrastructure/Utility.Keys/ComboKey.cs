using System.Collections;

namespace Utility.Keys
{
    public record ComboKey : StringKey, IEnumerable<int>
    {
        private readonly int[] value;

        public ComboKey(params int[] value) : base(string.Join( ",", value))
        {
            this.value = value;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return (IEnumerator<int>)value.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return value.GetEnumerator();
        }
    }
}