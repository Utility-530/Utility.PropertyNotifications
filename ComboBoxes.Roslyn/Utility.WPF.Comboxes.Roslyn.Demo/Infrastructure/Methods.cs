using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Utility.WPF.ComboBoxes.Roslyn.Demo.Infrastructure
{
    public class TestMethods
    {
        public int Test(bool x, int i)
        {
            return 0;
        }
    }

    public class Test
    {
        // Public properties
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsEnabled { get; set; }
        public double Score { get; set; }
        public DateTime CreatedAt { get; set; }
        public Orientation CurrentStatus { get; set; }
        public List<string> Tags { get; set; }
        public Dictionary<string, int> Scores { get; set; }
        public string? OptionalDescription { get; set; }
        public decimal Balance { get; set; }

        // Private properties
        private string SecretCode { get; set; }
        private int Counter { get; set; }
        private bool Flag { get; set; }
        private double InternalValue { get; set; }
        private DateTime LastModified { get; set; }

        // Protected properties
        protected string ProtectedString { get; set; }
        protected int ProtectedInt { get; set; }
        protected bool ProtectedBool { get; set; }
        protected DateTime ProtectedDate { get; set; }
        protected double ProtectedDouble { get; set; }

        // Internal properties
        internal string InternalName { get; set; }
        internal int InternalAge { get; set; }
        internal bool InternalFlag { get; set; }
        internal List<int> InternalList { get; set; }
        internal Orientation InternalStatus { get; set; }

        // Mixed access modifiers (protected internal, private protected)
        protected internal string MixedAccessString { get; set; }
        protected internal int MixedAccessInt { get; set; }
        private protected bool PrivateProtectedBool { get; set; }
        private protected DateTime PrivateProtectedDate { get; set; }

        // Nullable types
        public int? NullableInt { get; set; }
        public bool? NullableBool { get; set; }
        public double? NullableDouble { get; set; }
        public DateTime? NullableDate { get; set; }
        public Orientation? NullableStatus { get; set; }

        // Collections
        public List<int> Numbers { get; set; }
        public Dictionary<string, string> KeyValues { get; set; }
        public HashSet<string> UniqueTags { get; set; }
        public string[] StringArray { get; set; }
        public int[] IntArray { get; set; }

        // Read-only / write-only properties
        public string ReadOnlyString { get; }
        public int WriteOnlyInt { private get; set; }

        // Other types
        public Guid Id { get; set; }
        public TimeSpan Duration { get; set; }
        public Uri Website { get; set; }
        public char Initial { get; set; }
        public float Percentage { get; set; }

        // More misc
        public byte ByteValue { get; set; }
        public sbyte SignedByte { get; set; }
        public short ShortValue { get; set; }
        public long LongValue { get; set; }
        public uint UnsignedInt { get; set; }
    }
}
