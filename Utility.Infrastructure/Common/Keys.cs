using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Models;

namespace Utility.Infrastructure
{
    public class Keys
    {
        static readonly Guid playback = Guid.Parse("07e2070f-3aa6-43e6-8924-d705c7f958af");

        public static readonly Key Playback = new(playback, nameof(Playback), typeof(Playback));

    }
}
