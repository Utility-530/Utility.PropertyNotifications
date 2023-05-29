using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Simulation.ViewModel
{

    public class MainViewModel
    {
        public PlayerViewModel Player { get; set; }
        public RateViewModel Rate { get; set; }
        public ProgressViewModel Progress { get; set; }

    }
}
