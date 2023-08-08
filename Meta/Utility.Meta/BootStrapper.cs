//using Autofac;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Utility.Helpers;

//namespace Utility.Common
//{
//    public interface IBootStrapper
//    {
//        void Register(ContainerBuilder containerBuilder);
//    }

//    public static class BootStrapHelper
//    {
//        public static ContainerBuilder AutoRegister(this ContainerBuilder builder, IEnumerable<Assembly>? assembliesToScan = null)
//        {
//            foreach (IBootStrapper? bootStrapper in BootStrappers())
//            {
//                bootStrapper?.Register(builder);
//            }

//            return builder;

//            IEnumerable<IBootStrapper?> BootStrappers()
//            {
//                return (assembliesToScan ?? AssemblySingleton.Instance.Assemblies.Where(a => !a.IsDynamic)).TypesOf<IBootStrapper>();
//            }
//        }
//    }
//}