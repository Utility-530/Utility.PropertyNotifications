//using Autofac;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using Utility.Common;
//using Utility.WPF.Model;
//using Utility.WPF.Service;

//namespace Utility.WPF.Meta
//{
//    public class BootStrapper : IBootStrapper
//    {
//        public void Register(ContainerBuilder containerBuilder)
//        {
//            containerBuilder.Register(c => new TypeModel(typeof(BootStrapper)));
//            containerBuilder.RegisterType<TypeObjectsService>().SingleInstance();
//            containerBuilder.RegisterType<ViewModelAssemblyModel>().SingleInstance();
//        }
//    }
//}