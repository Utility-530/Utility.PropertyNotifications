using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.FileSystem.Transfer.Demo.Infrastructure
{
    public class DirectoryHelper
    {
        public static IEnumerable<DirectoryInfo> SplitDirectory(DirectoryInfo parent)
        {
            return NewMethod(parent).Reverse();
        }

        private static IEnumerable<DirectoryInfo> NewMethod(DirectoryInfo di)
        {
            if (di == null)
                throw new ArgumentNullException(nameof(di));

            while (di.Name != di.Root.Name)
            {
                yield return di;
                di = di.Parent;
            }
            yield return di.Root;
        }
    }
}
