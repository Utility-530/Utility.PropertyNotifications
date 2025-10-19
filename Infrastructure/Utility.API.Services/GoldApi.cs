using RestSharp;
using SQLite;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utility.API.Entities;
using Utility.API.Services.Infrastructure;
using Utility.Attributes;
using Utility.Entities;
using Utility.Enums;
using Utility.Helpers;

namespace Utility.API.Services
{
    public class GoldApi
    {
        public const string Gold = nameof(Gold);
        public const string Silver = nameof(Silver);
        public const string GramUnit = "g";
        public const string OunceUnit = "oz";
        public Task<GoldApi> Task { get; }

        public decimal GoldPricePerGram { get; set; } = 10;
        public decimal GoldPricePerOunce { get; set; } = 10;
        public decimal SilverPricePerGram { get; set; } = 1.2m;
        public decimal SilverPricePerOunce { get; set; } = 38;

        public GoldApi(CancellationToken? token = null)
        {
            Task = Initialise(token);
        }

        public async Task<GoldApi> Initialise(CancellationToken? token = null)
        {
            var connection = initialiseConnection();
            var client = connection.InsertClient(nameof(GoldApi), maxCalls, timeInterval);
            setPrice(findLatestPrice(Type.Gold));
            setPrice(findLatestPrice(Type.Silver));
            var availableCalls = findAvailableCalls();
            if (availableCalls > 0)
                await tryFetchAndInsertPrice(metalType: Type.Gold, client, connection);

            if (availableCalls > 1)
                await tryFetchAndInsertPrice(metalType: Type.Silver, client, connection);

            return this;

            static SQLiteConnection initialiseConnection()
            {
                var connection = new SQLiteConnection(Utility.Constants.DefaultAPIFilePath);
                connection.CreateTable<Client>();
                connection.CreateTable<Event>();
                connection.CreateTable<MetalPrice>();
                return connection;
            }

            MetalPrice? findLatestPrice(Type metal)
            {
                string symbol = EnumHelper.GetAttribute<Type, SymbolAttribute>(metal).Symbol;
                return connection.FindWithQuery<MetalPrice>(
                                    $"SELECT * FROM {nameof(MetalPrice)} WHERE {nameof(MetalPrice.Metal)} = ? AND {nameof(MetalPrice.Guid)} = " +
                                    $"(SELECT {nameof(Event.Guid)} " +
                                    $"FROM {nameof(Event)} " +
                                    $"ORDER BY {nameof(Event.Time)} DESC " +
                                    $"LIMIT 1); ", symbol);
            }

            async Task tryFetchAndInsertPrice(Type metalType, Client client, SQLiteConnection connection)
            {
                var time = DateTime.Now;
                var result = await GetPriceAsync(metalType, cancellationToken: token ?? CancellationToken.None);
                result.Data.Guid = Guid.NewGuid();

                if (result.IsSuccessful)
                {
                    connection.RunInTransaction(() =>
                    {
                        connection.Insert(result.Data);
                        connection.Insert(new Event { ParentId = client.Guid, Guid = result.Data.Guid, Time = time, Source = result.ResponseUri.AbsoluteUri });
                    });

                    setPrice(result.Data);
                }
            }

            int findAvailableCalls()
            {
                var calls = connection.Query<Event>($"SELECT {nameof(Event)}.* " +
                    $"FROM {nameof(Event)} " +
                    $"JOIN {nameof(Client)} " +
                    $"ON {nameof(Event)}.{nameof(Event.ParentId)} = {nameof(Client)}.{nameof(Client.Guid)} " +
                    $"WHERE {nameof(Client)}.{nameof(Client.Name)} = ?", nameof(GoldApi));

                DateTime[] x = [.. calls.Select(a => a.Time)];
                return Infrastructure.Helpers.AvailableCalls(x, client.MaxCalls, client.MaxCallsTimeFrame, TimeInterval.Day);
            }

            async Task setPrice(MetalPrice result)
            {
                if (result != null)
                {
                    var metal = EnumHelper.MatchByAttribute<Type, SymbolAttribute>(a => result.Metal == a.Symbol);
                    if (metal == Type.Gold)
                    {
                        GoldPricePerGram = result.PriceGram24k == 0 ? result.Price / 28.35m : result.PriceGram24k;
                        GoldPricePerOunce = result.Price;
                    }
                    else if (metal == Type.Silver)
                    {
                        SilverPricePerGram = result.PriceGram24k == 0 ? result.Price / 28.35m : result.PriceGram24k;
                        SilverPricePerOunce = result.Price;
                    }
                }
            }



            static void remainingCalls()
            {
                //var remainingCalls = connection.ExecuteScalar<int>(
                //    "SELECT MAX(0, c.MaxCalls - COUNT(call.Time)) AS AvailableCalls " +
                //    "FROM Client c " +
                //    "LEFT JOIN Call call ON call.ClientId = c.Guid WHERE c.Name = ? AND call.Time >= datetime('now', '-' || CASE c.MaxCallsTimeFrame  " +
                //    "      WHEN 7 THEN '1 second'     " +
                //    "      WHEN 8 THEN '1 minute'" +
                //    "      WHEN 9 THEN '1 hour'" +
                //    "      WHEN 10 THEN '1 day'" +
                //    "      WHEN 11 THEN '7 days'" +
                //    "      WHEN 12 THEN '14 days'" +
                //    "      WHEN 13 THEN '1 month'" +
                //    "      WHEN 14 THEN '3 months'" +
                //    "      WHEN 15 THEN '1 year'" +
                //    "      WHEN 16 THEN '10 years'" +
                //    "      WHEN 17 THEN '100 years'" +
                //    "      WHEN 18 THEN '1000 years'" +
                //    "      WHEN 19 THEN '1000000 years'" +
                //    "      ELSE '0 seconds'" +
                //    "  END);",
                //    nameof(Utility.API.GoldApi));
            }


        }

        public const string baseUrl = "https://www.goldapi.io/api/";
        public const string apiKey = "goldapi-a2wgrsmgv1mtf4-io";
        public const int maxCalls = 100;
        public const TimeInterval timeInterval = TimeInterval.Month;

        public enum Type
        {
            [Symbol("XAU")]
            Gold,
            [Symbol("XAG")]
            Silver,
            [Symbol("XPT")]
            Platinum,
            [Symbol("XPD")]
            Palladium
        }

        public static async Task<RestResponse<MetalPrice>> GetPriceAsync(Type metalType = Type.Gold, Utility.Enums.Currency currency = Enums.Currency.GBP, DateTime? date = default, CancellationToken cancellationToken = default)
        {
            cancellationToken = cancellationToken == default ? CancellationToken.None : cancellationToken;
            string symbol = EnumHelper.GetAttribute<Type, SymbolAttribute>(metalType).Symbol;
            string dateString = date.HasValue ? $"/{date.Value:YYYYMMDD}" : string.Empty;

            var options = new RestClientOptions(baseUrl);
            var client = new RestClient(options);

            var endpoint = $"{symbol}/{currency}/{date}";
            var request = new RestRequest(endpoint, Method.Get);

            request.AddHeader("x-access-token", apiKey);
            request.AddHeader("Content-Type", "application/json");

            return await client.ExecuteAsync<MetalPrice>(request, cancellationToken);
        }
    }
}
