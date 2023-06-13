using AutoMapper;

namespace Utility.Common
{
    public class AutoMapperSingleton
    {
        private IMapper mapper;

        private AutoMapperSingleton()
        {
            mapper = new MapperConfiguration(cfg => cfg.AddMaps(AssemblySingleton.Instance.Assemblies))
                .CreateMapper();
        }

        public static IMapper Instance { get; } = new AutoMapperSingleton().mapper;
    }
}