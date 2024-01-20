namespace Utility.ProjectStructure
{
    static class Helpers
    {
        public static string Remove(this string str, string substring)
        {
            return str.Replace(substring, "");
        }
    }
}

