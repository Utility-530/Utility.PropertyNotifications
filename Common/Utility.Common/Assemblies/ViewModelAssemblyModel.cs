using System.Threading.Tasks;
using Autofac;
using Nito.AsyncEx;
using Utility.WPF.Models;

namespace Utility.Common.Assemblies
{
    public class ViewModelAssemblyModel
    {
        private readonly AsyncLazy<TypeObject[]> typeObjects;

        public ViewModelAssemblyModel(TypeModel typeModel, TypeObjectsService typeObjectsService)
        {
            typeObjects = new AsyncLazy<TypeObject[]>(() =>
            Task.Run(async () => typeObjectsService.SelectTypeObjects(await typeModel.Collection)));
        }

        public async Task Register(ContainerBuilder containerBuilder)
        {
            foreach (var type in await typeObjects.Task)
                containerBuilder.RegisterType(type.Type);
        }

        public Task<TypeObject[]> Collection => typeObjects.Task;
    }
}