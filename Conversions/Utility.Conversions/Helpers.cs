namespace Utility.Conversions
{
    public static class Helpers
    {
        public static bool IsNullable(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}