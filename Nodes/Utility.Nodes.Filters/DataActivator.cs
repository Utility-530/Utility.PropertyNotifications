using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes.Filters
{
    internal static class DataActivator
    {

        public static object Activate(Structs.Repos.Key? a)
        {
            var _data = ActivateAnything.Activate.New(a.Value.Type);
            //_a.Data = a.Value.Instance ?? a.Value.Name;
            if (_data is IName name)
            {
                name.Name = a.Value.Name;
            }
            if (_data is ISetName sname)
            {
                sname.Name = a.Value.Name;
            }
            return _data;
        }
    }
}