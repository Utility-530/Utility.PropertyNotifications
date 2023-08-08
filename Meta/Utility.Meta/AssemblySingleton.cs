using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Utility.Meta
{
    public sealed class AssemblySingleton
    {
        private AssemblySingleton()
        {
            Assemblies = UtilitySolutionAssemblies().ToArray();

            static IEnumerable<Assembly> UtilitySolutionAssemblies()
            {
                var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*" + Meta.Constants.GeneralAssemblyName + "*.dll").ToArray();
                return files
                    //.Where(a => a.Contains("System") == false)
                    .Select(x =>
                    {
                        return Assembly.Load(AssemblyName.GetAssemblyName(x));
                    });
                //.Where(a => { return a == a; });
            }
        }

        public IReadOnlyCollection<Assembly> Assemblies { get; }

        public static AssemblySingleton Instance { get; } = new AssemblySingleton();
    }
}