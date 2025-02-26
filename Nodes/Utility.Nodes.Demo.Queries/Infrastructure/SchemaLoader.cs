using Newtonsoft.Json;
using Utility.Helpers;
using Utility.Models;

namespace Utility.Nodes.Demo.Queries
{
    public static class SchemaLoader
    {
        public static Schema LoadSchema()
        {
            Schema schema = JsonConvert.DeserializeObject<Schema>(Utility.Helpers.ResourceHelper.GetEmbeddedResource("TreeFilter.schema.json").AsString());
            return schema;
        }
    }
}
