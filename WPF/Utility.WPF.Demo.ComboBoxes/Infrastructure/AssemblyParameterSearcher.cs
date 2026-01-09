using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MaterialDesignExtensions.Model;
using Utility.Interfaces;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes;

namespace AssemblySearch
{
    /// <summary>
    /// Filter criteria for searching method parameters
    /// </summary>
    public record ParameterSearchFilter(IEnumerable<Assembly> Assemblies, bool? IsStatic, Type ParameterType, string? MethodNameMatch);

    /// <summary>
    /// Represents a found parameter with its context
    /// </summary>
    public record ParameterSearchResult(Assembly Assembly, Type DeclaringType, MethodBase Method, ParameterInfo Parameter, bool IsStatic) : IParameterInfo
    {
        public override string ToString()
        {
            return Parameter.Name;
        }
    }

    public class AssemblyParameterSearcher
    {
        /// <summary>
        ///builds tree of parameters matching the specified filters
        /// </summary>
        /// <param name="assemblies">Collection of assemblies to search through</param>
        /// <param name="filter">Filter criteria for the search</param>
        /// <returns>matching parameter results</returns>
        public static IEnumerable<NodeViewModel> Search(ParameterSearchFilter filter) =>
            from assembly in filter.Assemblies
            select new Model(() =>  from type in assembly.GetTypes()
                                    where matchesTypeFilter(type, filter)
                                    select new Model(() => 
                                            from method in type.GetMethods(                
                                            BindingFlags.Public |
                                            //BindingFlags.NonPublic |
                                            BindingFlags.Static |
                                            BindingFlags.Instance |
                                            BindingFlags.DeclaredOnly)
                                            .Cast<MethodBase>().Concat(
                                                type.GetConstructors(
                                                    BindingFlags.Public |
                                                    //BindingFlags.NonPublic |
                                                    BindingFlags.Instance |
                                                    BindingFlags.DeclaredOnly))
                                            where matchesMethodFilter(method, filter)
                                            select new Model(() =>  from parameter in method.GetParameters()
                                                                    where matchesParameterFilter(parameter, filter)
                                                                    select new Model()
                                                                    {
                                                                        Data = new ParameterSearchResult(assembly, type, method, parameter, method.IsStatic)
                                                                    })
                                            { Data = method })
                                    { Data = type })
            { Data = assembly };

        private static bool matchesTypeFilter(Type type, ParameterSearchFilter filter)
        {
            if (filter.ParameterType ==null || filter.ParameterType == type)
                return true;
            return false;
        }

        private static bool matchesMethodFilter(MethodBase method, ParameterSearchFilter filter)
        {
            // Check static/instance filter
            if (filter.IsStatic.HasValue)
            {
                if (filter.IsStatic.Value && !method.IsStatic)
                    return false;
                if (!filter.IsStatic.Value && method.IsStatic)
                    return false;
            }

            // Check method name partial match
            if (!string.IsNullOrEmpty(filter.MethodNameMatch))
            {
                if (!method.Name.Contains(filter.MethodNameMatch, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }

        private static bool matchesParameterFilter(ParameterInfo parameter, ParameterSearchFilter filter)
        {
            // Check parameter type parity
            if (filter.ParameterType != null)
            {
                if (parameter.ParameterType != filter.ParameterType)
                    return false;
            }

            return true;
        }
    }
}
