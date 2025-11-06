using System;
using System.Linq;
using Utility.Interfaces.NonGeneric;

namespace Utility.Common.Assemblies
{
    public class TypeObjectsService //: ITypeObjectsService
    {
        public virtual TypeObject[] SelectTypeObjects(Type[] types)
        {
            var xs = types
                   .SelectMany(type =>
                   {
                       var arr = Splat.Locator.Current.GetServices(type).Select(a => (a, type)).ToArray();
                       if (arr.Length == 0 && type.GetConstructor(Type.EmptyTypes) != null)
                           return new[] { (Activator.CreateInstance(type), type) };
                       return arr;
                   }).ToArray();

            var aa = xs.Select(st =>
             {
                 var (service, type) = st;
                 var name = typeof(IName).IsAssignableFrom(type) ?
                                                 (service as IGetName).Name :
                                                  type.Name;
                 return new TypeObject { TypeName = type.Name, Key = name, Object = service, Type = type };
             }).ToArray();

            return aa;
        }
    }
}