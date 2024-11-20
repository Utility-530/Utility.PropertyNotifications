namespace Utility.Trees.Demo.Connections
{
    public class Methods
    {
        public static string MethodA(string a, string b)
        {
            return a.Substring(3) + b.Substring(3);
        }

        public static string MethodB(string a, string b)
        {
            return string.Join("," + b);
        }
    }
}
