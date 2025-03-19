using Newtonsoft.Json.Linq;
using Splat;
using Utility.Repos;

namespace Utility.Nodes.Demo.Queries.Infrastructure
{
    internal class JObjectService : IJObjectService
    {
        const string fileName = "../../../Data/data.json";

        public JObjectService()
        {
        }

        Lazy<string?> body = new Lazy<string>(() =>
        {
            if (System.IO.File.Exists(fileName))
            {
                return System.IO.File.ReadAllText(fileName);
            }
            return null;
        });

        public JObject? Get()
        {
            if (string.IsNullOrEmpty(body.Value))
                return null;
            return JObject.Parse(body.Value);
        }

        public void Set(JObject jObject)
        {
            System.IO.File.WriteAllText(fileName, jObject.ToString());
        }
    }
}
