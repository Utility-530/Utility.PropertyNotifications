using System;
using Splat;

namespace Utility.Helpers.Ex
{
    public class SplatHelper
    {
        public static T GetServiceUnSafe<T>(Type type)
        {
            if (Locator.Current.GetService(type) is T thing)
            {
                return thing;
            }

            if (Locator.Current.GetService(type) is { })
            {
                throw new Exception(
                    $"Unable to get object associated {type.Name} derived from {typeof(T).Name}. " +
                    $"Check that the type, {type.Name}, derives from {typeof(T).Name}");
            }

            throw new Exception(
                $"Unable to get object associated {type.Name}. Check that the type, {type.Name}, has been registered by Splat.");
        }
    }
}
