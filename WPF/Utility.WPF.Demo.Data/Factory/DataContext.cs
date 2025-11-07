using System;
using Endless;

namespace Utility.WPF.Demo.Data.Factory
{
    public class DataContexts
    {
        private static Lazy<object>[] datacontexts = new[] {
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