using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Persist.Infrastructure
{
    public interface IPersist
    {

        void Save(Guid key);



        void Load(Guid key);


    }
}
