using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;

namespace Utility.Common
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Determines if this property is marked as init-only.
        /// </summary>
        /// <remarks>
        /// <a href="https://alistairevans.co.uk/2020/11/01/detecting-init-only-properties-with-reflection-in-c-9/"/></a>
        /// </remarks>
        /// <param name="property">The property.</param>
        /// <returns>True if the property is init-only, false otherwise.</returns>
        public static bool IsInitOnly(this PropertyInfo property)
        {
            if (!property.CanWrite)
            {
                return false;
            }

            var setMethod = property.SetMethod;

            // Get the modifiers applied to the return parameter.
            var setMethodReturnParameterModifiers = setMethod.ReturnParameter.GetRequiredCustomModifiers();

            // Init-only properties are marked with the IsExternalInit type.
            return setMethodReturnParameterModifiers.Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
        }

        public static IEnumerable<Assembly> LoadedUtilitySolutionAssemblies()
        {
            return Store.LoadedSolutionAssemblies(a => a.Name?.StartsWith(Meta.Constants.GeneralAssemblyName) ?? false);
        }

        private class Store
        {
            public List<Assembly> assemblies = new List<Assembly>();
            public HashSet<string> assemblyNames = new HashSet<string>();

            public static IEnumerable<Assembly> LoadedSolutionAssemblies(Predicate<AssemblyName> predicate)
            {
                var x = new Store();
                foreach (var assembly in Utility.Helpers.Reflection.ReflectionHelper.GetAssemblies(predicate))
                {
                    Recursive(assembly, predicate, ref x);
                }
                return x.assemblies;

                static void Recursive(Assembly assembly, Predicate<AssemblyName> predicate, ref Store x)
                {
                    if (!x.assemblies.Contains(assembly))
                        x.assemblies.Add(assembly);
                    var references = assembly.GetReferencedAssemblies().ToArray();
                    foreach (var assemblyName in from a in references
                                                 where predicate(a)
                                                 select a.Name)
                    {
                        if (x.assemblyNames.Add(assemblyName) && Assembly.Load(assemblyName) is Assembly refAssembly)
                        {
                            Recursive(refAssembly, predicate, ref x);
                        }
                    }
                }
            }
        }
    }
}