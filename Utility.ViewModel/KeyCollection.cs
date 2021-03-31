using System;
using System.Collections;
using UtilityInterface.Generic;

namespace Utility.ViewModel
{
    public class KeyCollection : IKey<string>, IComparable, IComparable<KeyCollection>
    {
        public KeyCollection(string key, ICollection value)
        {
            Key = key;
            Collection = value;
        }

        public string Key { get; }

        public ICollection Collection { get; }

        public int CompareTo(object obj)
        {
            return obj is IKey<string> key ? Key.CompareTo(key.Key) : 0;
        }

        public int CompareTo(KeyCollection other)
        {
            return Key.CompareTo(other.Key);
        }

        public override string ToString()
        {
            return this.Key + " " + DateTime.Now.ToString("t") + " " + this.Collection.Count;
        }
    }
}