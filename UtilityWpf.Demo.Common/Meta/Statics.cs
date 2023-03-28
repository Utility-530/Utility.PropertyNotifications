using Splat;
using System;

namespace UtilityWpf.Demo.Common.Meta
{
    public static class Statics
    {
        public static Random Random { get; } = new Random();

        //public static T Service<T>() => Locator.Current.GetService<T>() ?? throw new Exception("Problem retrieveing " + typeof(T).Name);

        //public static FactoryLogger FactoryLogger { get; } = new FactoryLogger();
    }
}