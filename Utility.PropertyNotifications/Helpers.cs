using System;
using System.Collections.Generic;
using System.Text;
using Utility.Helpers.Reflection;

namespace Utility.PropertyNotifications
{
    internal class Helpers
    {
        public static bool Proceed<T>(T value, bool includeDefaultValue)
        {
            return includeDefaultValue || !EqualityComparer<T>.Default.Equals(value, default(T));
        }

        public static bool Proceed(object value, bool includeDefaultValue)
        {
            return includeDefaultValue || !Comparison.IsDefaultValue(value, isNullTreatedAsDefaultValue: true);
        }
    }
}
