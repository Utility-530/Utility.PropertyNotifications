using Newtonsoft.Json.Linq;
using Splat;
using Utility.Repos;


namespace Utility.Nodes.Demo.Queries.Infrastructure
{
    internal class JObjectService : IJObjectService
    {
        Lazy<IMainViewModel> mainViewmModel = new(() => Locator.Current.GetService<IMainViewModel>());

        public JObject? Get()
        {
            if (mainViewmModel.Value.Filter is FilterEntity { Body: { } body } entity)
            {
                return JObject.Parse(body);
            }
            //throw new Exception("222E 44444");
            return null;

        }

        public void Set(JObject jObject)
        {
            if (mainViewmModel.Value.Filter is FilterEntity { Body: { } body } entity)
            {
                entity.Body = jObject.ToString();
            }
            else
                throw new Exception("E 44444");
        }
    }
}
