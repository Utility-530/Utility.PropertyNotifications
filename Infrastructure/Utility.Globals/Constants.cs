using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class Constants
    {
        public const string DefaultDataPath = "O:\\Users\\rytal\\Data";
        public const string DefaultModelsFileName = "models.sqlite";
        public static readonly string DefaultModelsFilePath = Path.Combine(DefaultDataPath, DefaultModelsFileName);
    }
}
