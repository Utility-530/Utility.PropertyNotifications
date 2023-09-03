using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using Utility.WPF.Models;

namespace Utility.WPF.Controls.Lists.Infrastructure
{
    public class CheckProfile : AutoMapper.Profile
    {
        public CheckProfile()
        {
            CreateMap<CheckedRoutedEventArgs, Dictionary<object, bool?>>()
                .ConvertUsing(a => (a.Source as Selector).ToDictionary());
        }
    }
}
