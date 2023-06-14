using System.Collections;
using System.Linq;
using System.Windows;
using Utility.WPF.Demo.Data.Model;

namespace Utility.WPF.Demo.Data.Factory
{
    public class Characters
    {

        public static IEnumerable Value => (Application.Current.FindResource("Characters") as IEnumerable).Cast<Character>();
    }
}
