using Endless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.WPF.Demo.Data.Factory
{
    public class DataContexts
    {
        static Lazy<object>[] datacontexts = new[] {
            new Lazy<object>(()=> new Characters())
        };
        public static object Random
        {
            get
            {
                return datacontexts.Random().Value;
            }
        }
    }
}
