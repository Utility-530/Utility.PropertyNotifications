using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable RCS1104 // Simplify conditional expression

namespace Utility.Common.Collections
{
    /// <summary>
    /// Represents a generic collection of keys and values where keys can be
    /// retrieved by their associated value. The backing store consists of
    /// dictionaries with the values residing in a hashset.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys
    /// <see cref="Map{TKey, TVal}"/>.</typeparam>
    /// <typeparam name="TVal">The type of the values
    /// <see cref="Map{TKey, TVal}"/>.</typeparam>
    public class Map<TKey, TVal> : IDictionary<TKey, TVal>
    {

        /// <summary>
        /// An empty read-only collection to avoid instantiating new empty
        /// collections when one needs to be returned.
        /// </summary>
        private static readonly IReadOnlyCollection<TKey> empty = new List<TKey>();

        private readonly Dictionary<TKey, TVal> forward;
        private readonly Dictionary<TVal, HashSet<TKey>> reverse;

        /// <summary>
        /// Holds all of the keys that are associated with null values. We need
        /// it because dictRev cannot hold null as keys.
        /// </summary>
        private readonly HashSet<TKey> keysWithNullValue;


        /// <summary>
        /// Initializes a new instance of the <see cref="Map{TKey, TVal}"/>
        /// class that is empty, has the default initial capacity, and uses
        /// the default equality comparers for the key and value types.
        /// </summary>
        public Map() : this(0, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Map{TKey, TVal}"/>
        /// class that is empty, has the specified initial capacity, and uses
        /// the default equality comparers for the key and value types.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the
        /// <see cref="Map{TKey, TVal}"/> can contain.</param>
        public Map(int capacity) : this(capacity, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Map{TKey, TVal}"/>
        /// class that is empty, has the default initial capacity, and uses
        /// the specified equality comparers for the key and value types.
        /// </summary>
        /// <param name="comparerKey">The <see cref="IEqualityComparer{TKey}"/>
        /// implementation to use when comparing keys, or null to use the
        /// default <see cref="EqualityComparer{TKey}"/> for the type of the
        /// key.</param>
        /// <param name="comparerValue">The
        /// <see cref="IEqualityComparer{TVal}"/> implementation to use when
        /// comparing values, or null to use the
        /// default <see cref="EqualityComparer{TVal}"/> for the type of the
        /// value.</param>
        public Map(
            IEqualityComparer<TKey> comparerKey,
            IEqualityComparer<TVal> comparerValue)
            : this(0, comparerKey, comparerValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Map{TKey, TVal}"/>
        /// class that is empty, has the specified initial capacity, and uses
        /// the specified equality comparers for the key and value types.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the
        /// <see cref="Map{TKey, TVal}"/> can contain.</param>
        /// <param name="comparerKey">The <see cref="IEqualityComparer{TKey}"/>
        /// implementation to use when comparing keys, or null to use the
        /// default <see cref="EqualityComparer{TKey}"/> for the type of the
        /// key.</param>
        /// <param name="comparerValue">The
        /// <see cref="IEqualityComparer{TVal}"/> implementation to use when
        /// comparing values, or null to use the
        /// default <see cref="EqualityComparer{TVal}"/> for the type of the
        /// value.</param>
        public Map(
            int capacity,
            IEqualityComparer<TKey>? comparerKey,
            IEqualityComparer<TVal>? comparerValue)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(capacity), "Capacity cannot be less than zero.");
            }

            forward = new Dictionary<TKey, TVal>(capacity, comparerKey);
            reverse = new Dictionary<TVal, HashSet<TKey>>(
                capacity,
                comparerValue);
            keysWithNullValue = new HashSet<TKey>(comparerKey);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Map{TKey, TVal}"/>
        /// class that contains elements copied from the specified KeyValuePair
        /// collection and uses the default equality comparers for the key and
        /// value types.
        /// </summary>
        /// <param name="keyValuePairs">The KeyValuePair collection whose
        /// elements are copied to the new <see cref="Map{TKey, TVal}"/>.
        /// </param>
        public Map(ICollection<KeyValuePair<TKey, TVal>> keyValuePairs)
            : this(keyValuePairs, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Map{TKey, TVal}"/>
        /// class that contains elements copied from the specified
        /// KeyValuePair collection and uses the specified equality comparers
        /// for the key and value types.
        /// </summary>
        /// <param name="keyValuePairs">The KeyValuePair collection whose
        /// elements are copied to the new <see cref="Map{TKey, TVal}"/>.
        /// </param>
        /// <param name="comparerKey">The <see cref="IEqualityComparer{TKey}"/>
        /// implementation to use when comparing keys, or null to use the
        /// default <see cref="EqualityComparer{TKey}"/> for the type of the
        /// key.</param>
        /// <param name="comparerValue">The
        /// <see cref="IEqualityComparer{TVal}"/> implementation to use when
        /// comparing values, or null to use the
        /// default <see cref="EqualityComparer{TVal}"/> for the type of the
        /// value.</param>
        public Map(
            ICollection<KeyValuePair<TKey, TVal>> keyValuePairs,
            IEqualityComparer<TKey>? comparerKey,
            IEqualityComparer<TVal>? comparerValue)
            : this(keyValuePairs.Count, comparerKey, comparerValue)
        {
            if (keyValuePairs == null)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }
            forward = keyValuePairs.ToDictionary(a => a.Key, a => a.Value);

            PopulateReverseFromOtherDictionary(keyValuePairs);
        }




        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}"/> that is used to
        /// determine equality of keys for the
        /// <see cref="Map{TKey, TVal}"/>.
        /// </summary>
        public IEqualityComparer<TKey> ComparerKey => forward.Comparer;

        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}"/> that is used to
        /// determine equality of values for the
        /// <see cref="Map{TKey, TVal}"/>.
        /// </summary>
        public IEqualityComparer<TVal> ComparerValue => reverse.Comparer;

        /// <summary>
        /// Gets the number of unique values contained in the
        /// <see cref="Map{TKey, TVal}"/>
        /// </summary>
        public int UniqueValueCount => reverse.Count;

        /// <summary>
        /// Gets the number of elements contained in the
        /// <see cref="Map{TKey, TVal}"/>
        /// </summary>
        public int Count => forward.Count;

        /// <summary>
        /// Gets a collection containing the keys in the
        /// <see cref="Map{TKey, TVal}"/>.
        /// </summary>
        public IReadOnlyCollection<TKey> Keys => forward.Keys;

        /// <summary>
        /// Gets a collection containing the values in the
        /// <see cref="Map{TKey, TVal}"/>.
        /// </summary>
        public IReadOnlyCollection<TVal> Values => forward.Values;

        ICollection<TKey> IDictionary<TKey, TVal>.Keys
            => ((IDictionary<TKey, TVal>)forward).Keys;


        bool ICollection<KeyValuePair<TKey, TVal>>.IsReadOnly
            => ((IDictionary<TKey, TVal>)forward).IsReadOnly;

        ICollection<TVal> IDictionary<TKey, TVal>.Values
            => ((IDictionary<TKey, TVal>)forward).Values;



        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key. If the
        /// specified key is not found, a get operation throws a
        /// <see cref="KeyNotFoundException"/>, and a set operation creates a
        /// new element with the specified key.</returns>
        public TVal this[TKey key]
        {
            get => forward[key];
            set
            {
                if (ContainsKey(key))
                {
                    ReplaceValue(key, value);
                }
                else
                {
                    Add(key, value);
                }
            }
        }


        public TKey this[TVal val]
        {
            get => reverse[val].Single();
            set
            {
                if (ContainsValue(val))
                {
                    ReplaceValue(value, val);
                }
                else
                {
                    Add(value, val);
                }
            }
        }



        /// <summary>
        /// Adds the specified key and value to the
        /// <see cref="Map{TKey, TVal}"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can
        /// be null for reference types.</param>
        public void Add(TKey key, TVal value)
        {
            forward.Add(key, value);
            AddRevEntry(key, value);
        }

        /// <summary>
        /// Removes all keys and values from the
        /// <see cref="Map{TKey, TVal}"/>. The internal key
        /// collections are cleared before being removed from the
        /// <see cref="Map{TKey, TVal}"/> which will reflect in any
        /// variable referencing them.
        /// </summary>
        public void Clear()
        {
            foreach (HashSet<TKey> hashSetKeys in reverse.Values)
            {
                hashSetKeys.Clear();
            }

            forward.Clear();
            reverse.Clear();
            keysWithNullValue.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="Map{TKey, TVal}"/>
        /// contains a specific key.
        /// </summary>
        /// <param name="key">The key to locate in the
        /// <see cref="Map{TKey, TVal}"/>.</param>
        /// <returns>true if the <see cref="Map{TKey, TVal}"/> contains an
        /// element with the specified key; otherwise, false.</returns>
        public bool ContainsKey(TKey key) => forward.ContainsKey(key);

        /// <summary>
        /// Determines whether the <see cref="Map{TKey, TVal}"/>
        /// contains an element that has the specified value.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>true if the <see cref="Map{TKey, TVal}"/>
        /// contains an element that has the specified value; otherwise,
        /// false.</returns>
        public bool ContainsValue(TVal value) => value == null
            ? keysWithNullValue.Count > 0
            : reverse.ContainsKey(value);

        /// <summary>
        /// Returns an enumerator that iterates through the key-value pairs of
        /// the <see cref="Map{TKey, TVal}"/>.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TVal}.Enumerator"/> structure
        /// for the <see cref="Map{TKey, TVal}"/>.</returns>
        public IEnumerator<KeyValuePair<TKey, TVal>> GetEnumerator()
            => forward.GetEnumerator();

        /// <summary>
        /// Gets all keys associated with the specified value.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>A read-only collection containing all keys associated
        /// with the specified value.</returns>
        public IEnumerable<TKey> GetKeysWithValue(TVal value)
            => ContainsValue(value)
                ? value == null ? keysWithNullValue : reverse[value]
                : empty;

        /// <summary>
        /// Removes the element with the specified key from the
        /// <see cref="Map{TKey, TVal}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully found and removed;
        /// otherwise, false. This method returns false if key is not found in
        /// the <see cref="Map{TKey, TVal}"/>.</returns>
        public bool Remove(TKey key)
            => forward.TryGetValue(key, out var value) && forward.Remove(key)
                ? RemoveRevEntry(key, value)
                : false;

        /// <summary>
        /// Replaces the value currently associated with the specified key with
        /// a new value.
        /// </summary>
        /// <param name="key">The key of the value to replace.</param>
        /// <param name="value">The new value.</param>
        public void Replace(TKey key, TVal value) => ReplaceValue(key, value);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <param name="value">When this method returns, contains the value
        /// associated with the specified key, if the key is found; otherwise,
        /// the default value for the type of the value parameter.</param>
        /// <returns>true if the <see cref="Map{TKey, TVal}"/> contains an
        /// element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(TKey key, out TVal value)
            => forward.TryGetValue(key, out value);

        void ICollection<KeyValuePair<TKey, TVal>>.Add(KeyValuePair<TKey, TVal> item)
            => ((ICollection<KeyValuePair<TKey, TVal>>)forward).Add(item);

        bool ICollection<KeyValuePair<TKey, TVal>>.Contains(KeyValuePair<TKey, TVal> item)
            => ((ICollection<KeyValuePair<TKey, TVal>>)forward).Contains(item);

        bool ICollection<KeyValuePair<TKey, TVal>>.Remove(KeyValuePair<TKey, TVal> item)
            => ((ICollection<KeyValuePair<TKey, TVal>>)forward).Remove(item);

        void ICollection<KeyValuePair<TKey, TVal>>.CopyTo(
            KeyValuePair<TKey, TVal>[] array,
            int arrayIndex)
            => ((IDictionary<TKey, TVal>)forward).CopyTo(array, arrayIndex);

        IEnumerator IEnumerable.GetEnumerator()
            => ((IDictionary<TKey, TVal>)forward).GetEnumerator();



        private void AddRevEntry(TKey key, TVal value)
        {
            if (value == null)
            {
                keysWithNullValue.Add(key);
            }
            else
            {
                if (reverse.TryGetValue(value, out var keys))
                {
                    keys.Add(key);
                }
                else
                {
                    reverse.Add(value, new HashSet<TKey>(ComparerKey) { key });
                }
            }
        }

        private bool RemoveRevEntry(TKey key, TVal value)
            => value == null
                ? keysWithNullValue.Remove(key)
                : reverse[value].Remove(key);

        private void PopulateReverseFromOtherDictionary(
            ICollection<KeyValuePair<TKey, TVal>> kvps)
        {
            foreach (KeyValuePair<TKey, TVal> kvp in kvps)
            {
                if (kvp.Value == null)
                {
                    keysWithNullValue.Add(kvp.Key);
                    continue;
                }

                if (reverse.ContainsKey(kvp.Value))
                {
                    reverse[kvp.Value].Add(kvp.Key);
                }
                else
                {
                    reverse[kvp.Value]
                        = new HashSet<TKey>(ComparerKey) { kvp.Key };
                }
            }
        }

        private void ReplaceValue(TKey key, TVal value)
        {
            TVal oldValue = forward[key];
            forward[key] = value;

            RemoveRevEntry(key, oldValue);
            AddRevEntry(key, value);
        }
    }
}