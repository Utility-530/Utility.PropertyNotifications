using AutoMapper;

namespace Utility.Meta
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


