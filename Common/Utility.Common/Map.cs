using System;

namespace Utility.Common
{
    public class Map<T, TR> where T : class where TR : class
    {
        protected Map(TR value)
        {
            Value = value;
        }

        public static explicit operator Map<T, TR>(T b)
        {
            var types = (typeof(T), typeof(TR));
            if (types == (typeof(string), typeof(System.Version)))
            {
                return new Map<T, TR>((TR)(object)(Version)(Map)(object)b);
            }
            var conversion = (TR)(object)b;
            if (conversion == null)
                throw new InvalidCastException("gg 44 44ddfsd");
            return new Map<T, TR>(conversion);
        }

        public static implicit operator TR(Map<T, TR> map)
        {
            return map.Value;
        }

        public TR Value { get; protected set; }
    }

    public class Map
    {
        protected readonly object value;

        private Map(object value)
        {
            this.value = value;
        }

        public static explicit operator Map(string value)
        {
            return new Map(new Version(value));
        }

        public static explicit operator Version(Map map)
        {
            return (Version)map.value;
        }
    }
}