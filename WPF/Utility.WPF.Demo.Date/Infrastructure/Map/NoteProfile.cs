using Utility.WPF.Demo.Date.Infrastructure.Entity;
using Utility.WPF.Demo.Date.Infrastructure.ViewModels;

namespace Utility.WPF.Demo.Date.Infrastructure.Map
{

    public class Profile : AutoMapper.Profile
    {
        public Profile() : base(nameof(Utility.WPF.Meta))
        {
            CreateMap<NoteEntity, NoteViewModel>().ReverseMap();
        }
    }

}
