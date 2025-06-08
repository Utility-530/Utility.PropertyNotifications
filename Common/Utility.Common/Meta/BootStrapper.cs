using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utility.Helpers.Reflection;

namespace Utility.Common
{
    public interface IBootStrapper
    {
        void Register(ContainerBuilder containerBuilder);
    }

    public class Resolver
    {
        private IContainer? container;

        public void AutoRegister(IEnumerable<Assembly>? assembliesToScan = null)
        {

            ContainerBuilder builder = new();
            foreach (IBootStrapper? bootStrapper in BootStrappers())
            {
                bootStrapper?.Register(builder);
            }

            container = builder.Build();

            IEnumerable<IBootStrapper?> BootStrappers()
            {
                return (assembliesToScan ?? AssemblySingleton.Instance.Assemblies.Where(a => !a.IsDynamic)).TypesOf<IBootStrapper>();
            }
        }

        public static Resolver Instance { get; } = new Resolver();


        public T Resolve<T>() where T : notnull
        {
            return (container ?? throw new Exception("tm=75 776m,r")).Resolve<T>() ?? throw new Exception("m6776m,,ffjr");
        }
    }
}