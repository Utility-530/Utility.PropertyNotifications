using System;
using System.Collections;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

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

        public bool Equals(IKey<string> other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IEquatable other)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return this.Key + " " + DateTime.Now.ToString("t") + " " + this.Collection.Count;
        }
    }
}