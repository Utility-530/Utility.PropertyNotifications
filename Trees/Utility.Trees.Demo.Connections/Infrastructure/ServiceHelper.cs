using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Utility.Trees.Demo.Connections
{
    public static class ServiceHelper
    {
        public static IEnumerable<Service> ToServices(this Type type)
        {
            return from a in type.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                   select
                       new Service
                       {
                           Name = a.Name,
                           MethodInfo = a,
                           Instance = null,
                           Inputs = new ObservableCollection<Parameter>(
                            a.GetParameters()
                           .Select(p =>
                           {
                               return new Parameter(p, a);
                           })),
                           Outputs = new ObservableCollection<Parameter>(new Parameter[] { new(a.ReturnParameter, a) })

                       };
        }
    }
}
