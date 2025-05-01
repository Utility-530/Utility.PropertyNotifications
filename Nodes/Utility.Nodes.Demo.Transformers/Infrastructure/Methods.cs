using System.Linq;

namespace Utility.Nodes.Demo.Transformers
{
    public class Methods
    {
        public static bool FirstMethod(bool value)
        {
            return value;
        }

        public static int SecondMethod(bool value, string stringParameter)
        {
            return value ? stringParameter.Count() : 0;
        }
    }
}
