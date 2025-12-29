using Coinbase;
using CsvHelper;
using CsvHelper.Configuration;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utility.API.Entities;
using Utility.Attributes;
using Utility.Entities;
using Utility.Enums;
using Utility.Helpers;
using Utility.Services.Meta;
using Task = System.Threading.Tasks.Task;

namespace Utility.API.Services
{
    public class Coinbase
    {
        public record InputParam() : Param<Coinbase>(nameof(Coinbase.Import));
        public record ReturnParam() : Param<Coinbase>(nameof(Coinbase.Import));

        private const string Name = "Coinbase_CSV_Importer";
        private const string CoinbaseTransactionImport = "Coinbase_Transaction_Import";
        private static CoinbaseClient client;

        public enum TransactionType
        {
            [Description("Reward Income")]
            RewardIncome,
            [Description("Advanced Trade Sell")]
            AdvancedTradeSell,
            [Description("Advanced Trade Buy")]
            AdvancedTradeBuy,
            [Description("Retail Unstaking Transfer")]
            RetailUnstakingTransfer,
            [Description("Withdrawal")]
            Withdrawal,
            [Description("Deposit")]
            Deposit,
            [Description("Pro Deposit")]
            ProDeposit,
            [Description("Pro Withdrawal")]
            ProWithdrawal,
        }

        public class CoinbaseTransaction : Entity
        {
            public DateTime Timestamp { get; set; }
            public TransactionType TransactionType { get; set; }
            public CryptoCoin Asset { get; set; }
            public decimal QuantityTransacted { get; set; }
            public Currency PriceCurrency { get; set; }
            public decimal PriceAtTransaction { get; set; }
            public decimal Subtotal { get; set; }
            public decimal Total { get; set; }
            public decimal FeesSpread { get; set; }
            public string Notes { get; set; }
        }

        public sealed class TransactionMap : ClassMap<CoinbaseTransaction>
        {
            public TransactionMap()
            {
                Map(m => m.Id).Convert(args => parseID(args.Row));
                Map(m => m.Timestamp).Convert(args => DateTime.Parse(args.Row.GetField("Timestamp").Replace(" UTC", ""), CultureInfo.InvariantCulture));
                Map(m => m.TransactionType).Convert(args => parseTransactionType(args.Row));
                Map(m => m.Asset).Convert(args => parseCryptoCoin(args.Row));
                Map(m => m.QuantityTransacted).Name("Quantity Transacted");
                Map(m => m.PriceCurrency).Convert(args => Utility.Helpers.EnumHelper.Parse<Currency>(args.Row["Price Currency"]));
                Map(m => m.PriceAtTransaction).Convert(args => decimal.Parse(args.Row.GetField("Price at Transaction").Replace("£", ""), CultureInfo.InvariantCulture));
                Map(m => m.Subtotal).Convert(args =>
                    decimal.Parse(args.Row.GetField("Subtotal").Replace("£", ""), CultureInfo.InvariantCulture));
                Map(m => m.Total).Convert(args =>
                    decimal.Parse(args.Row.GetField("Total").Replace("£", ""), CultureInfo.InvariantCulture));
                Map(m => m.FeesSpread).Convert(args =>
                    decimal.Parse(args.Row.GetField("Fees_Spread").Replace("£", ""), CultureInfo.InvariantCulture));
                Map(m => m.Notes).Name("Notes");
            }

            Guid parseID(IReaderRow field)
            {
                var raw = field.GetField("Id");
                var padded = raw.PadRight(32, '0'); // Add trailing zeros
                return Guid.ParseExact(padded, "N"); // No hyphens format

            }
            TransactionType parseTransactionType(IReaderRow field)
            {
                var raw = field.GetField("Transaction Type");
                var padded = EnumHelper.MatchByAttribute<TransactionType, DescriptionAttribute>(a => a.Description == raw);
                return padded; // No hyphens format
            }
            CryptoCoin parseCryptoCoin(IReaderRow field)
            {
                var raw = field.GetField("Asset");
                var padded = EnumHelper.MatchByAttribute<CryptoCoin, DescriptionAttribute>(a => a.Description == raw);
                return padded; // No hyphens format
            }

        }

        public static Task Import()
        {
            return Task.Run(() =>
            {
                var csvFileDirectory = Path.Combine(Utility.Constants.Paths.DefaultDataPath, "Coinbase");
                var connection = new SQLiteConnection(Utility.Constants.Paths.DefaultModelsFilePath);
                connection.CreateTable<Event>();
                connection.CreateTable<Client>();
                connection.CreateTable<CoinbaseTransaction>();

                var client = Utility.API.Services.Infrastructure.Helpers.InsertClient(connection, Name, default, default);
                connection.RunInTransaction(() =>
                {
                    foreach (var file in Directory.EnumerateFiles(csvFileDirectory, "*.csv"))
                    {
                        var fileName = Path.GetFileName(file);
                        if (connection.FindWithQuery<Event>($"SELECT * FROM {nameof(Event)} WHERE {nameof(Event.Source)} = ? AND {nameof(Event.Type)} = ?", fileName, CoinbaseTransactionImport) is { } ev)
                        {
                            return;
                        }
                        foreach (var transaction in readTransactionsFromCSV(file))
                        {
                            connection.Insert(transaction);
                        }
                        connection.Insert(new Event
                        {
                            Guid = Guid.NewGuid(),
                            ParentId = client.Guid,
                            Time = DateTime.Now,
                            Source = fileName,
                            Type = CoinbaseTransactionImport,
                        });
                    }
                });
            });

            static IEnumerable<CoinbaseTransaction> readTransactionsFromCSV(string filePath)
            {
                using var reader = new StreamReader(filePath);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                };

                using var csv = new CsvReader(reader, config);
                csv.Context.RegisterClassMap<TransactionMap>();
                return csv.GetRecords<CoinbaseTransaction>().ToList();
            }
        }

        [Param(ratePerMinute: 5)]
        public record CryptoCoinParam() : Param<Coinbase>(nameof(Coinbase.SpotPrice), "coin");
        public record CurrencyParam() : Param<Coinbase>(nameof(Coinbase.SpotPrice), "currency");
        public record ReturnPriceParam() : Param<Coinbase>(nameof(Coinbase.SpotPrice));



        public static async Task<decimal> SpotPrice(CryptoCoin coin, Currency currency)
        {
            //var client = new CoinbaseClient(new OAuthConfig { AccessToken = "..." });

            ////using API Key + Secret authentication
            //var client = new CoinbaseClient(new ApiKeyConfig { ApiKey = "...", ApiSecret = "..." });

            //No authentication
            //  - Useful only for Data Endpoints that don't require authentication.
            client ??= new CoinbaseClient();
            var spot = await client.Data.GetSpotPriceAsync(EnumHelper.GetDescription(coin) + "-" + currency.ToString());
            return spot.Data.Amount;
            //spot.Data.Amount.Should().BeGreaterThan(5);
            //spot.Data.Currency.Should().Be("USD");
            //spot.Data.Base.Should().Be("ETH");
        }
    }
}

