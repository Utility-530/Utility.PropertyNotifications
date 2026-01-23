using System;
using System.Collections.Generic;
using System.Text;
using Utility.Helpers.Reflection;

namespace Utility.PropertyNotifications
{
    internal class Helpers
    {
        public static bool Include<T>(T value, bool includeNulls, bool includeDefaultValue) 
   {
            if (value is null)
            {
                if (includeNulls)
                    return true;
            }

            bool isDefaultValue = Comparison.IsDefaultValue(value);
            if (!isDefaultValue || includeDefaultValue)
            {
                return true;
            }
            return false;
        }

    }
}
