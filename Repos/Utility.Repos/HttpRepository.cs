using System.Text.Json;
using System.Web;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using _Key = Utility.Models.Key;
namespace Utility.Repos
{
    public class HttpRepository : IRepository
    {
        private readonly HttpClient client;

        public HttpRepository()
        {
            client = new HttpClient { BaseAddress = new Uri("http://localhost:5084/"), Timeout = TimeSpan.FromSeconds(5) };
        }

        public IEquatable Key => new Key<HttpRepository>(Guids.Http);

        public async Task<IEquatable[]> FindKeys(IEquatable key)
        {
            if (key is not _Key { Guid: var guid, Name: var name, Type: var type } _key)
            {
                throw new Exception("reg 43cs ");
            }

            var query = HttpUtility.ParseQueryString("");
            query["key"] = guid.ToString();
            query["name"] = name.ToString();
            string queryString = query.ToString();
            HttpResponseMessage response = await client.GetAsync("GetKeyByParent?" + queryString);
            response.EnsureSuccessStatusCode();
            string jsonResponseBody = await response.Content.ReadAsStringAsync();
            Guid? model = JsonSerializer.Deserialize<Guid>(jsonResponseBody);
            if (model.HasValue == false)
                throw new Exception("sdfklj3 fsdfsd3433");
            return new[] { new _Key(model.Value, name, type) };
        }

        public async Task<object?> FindValue(IEquatable key)
        {
            if (key is not _Key { Guid: var guid, Name: var name, Type: var type } _key)
            {
                throw new Exception("reg 43cs ");
            }

            var query = HttpUtility.ParseQueryString("");
            query["key"] = guid.ToString();
            //query["name"] = name.ToString();
            string queryString = query.ToString();
            var response = await client.GetAsync("GetValue?" + queryString);
            response.EnsureSuccessStatusCode();
            string jsonResponseBody = await response.Content.ReadAsStringAsync();
            if (jsonResponseBody.Length > 0)
                return JsonSerializer.Deserialize(jsonResponseBody, type);
            return default;
        }

        public async Task Update(IEquatable key, object value)
        {
            if (key is not _Key { Guid: var guid, Name: var name, Type: var type } _key)
            {
                throw new Exception("reg 43cs ");
            }

            var str = JsonSerializer.Serialize(value, value.GetType());
            var query = HttpUtility.ParseQueryString("");
            query["key"] = guid.ToString();
            query["value"] = str;
            string queryString = query.ToString();

            var response = await client.PostAsync("PostValue?" + queryString, default);
            response.EnsureSuccessStatusCode();
            string jsonResponseBody = await response.Content.ReadAsStringAsync();
        }
    }
}