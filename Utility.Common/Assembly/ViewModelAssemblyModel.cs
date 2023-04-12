using Autofac;
using Nito.AsyncEx;
using System.Threading.Tasks;
using Utility.Common;
using UtilityWpf.Service;

namespace UtilityWpf.Model
{
    public class ViewModelAssemblyModel //: IViewModelAssemblyModel
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