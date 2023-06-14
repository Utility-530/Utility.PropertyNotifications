using Autofac;
using Utility.Common;

namespace Utility.WPF.Demo.Common.Meta
{
    internal class BootStrapper : IBootStrapper
    {
        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FactoryLogger>().SingleInstance();
            containerBuilder.RegisterType<Factory>().SingleInstance();
            containerBuilder.RegisterType<KeyStore>().SingleInstance();
        }
    }
}