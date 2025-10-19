using System;
using System.Collections.Generic;
using CsvHelper;
using CsvHelper.Configuration;
using SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Utility.API.Entities;
using Utility.Entities;
using Utility.Helpers;
using Utility.Services.Meta;

namespace Utility.API.Services
{

    public class Halifax
    {
        public record InputParam() : Param<Halifax>(nameof(Halifax.InsertData));
        public record ReturnParam() : Param<Halifax>(nameof(Halifax.InsertData));



        public const string localDataFile = @"O:\Users\rytal\Data\General.sqlite";
        public const string halifaxDataFolder = @"O:\Users\rytal\Data\Halifax";
        public static readonly Regex regex = new(@"\d{8}");
        private static Transaction[] oldRecords;
        public static string Type = "Halifax_CSV_Read";

        public static void InsertData()
        {
            DateTime dateNow = DateTime.Now;
            var repo = new SQLiteConnection(localDataFile);

            repo.CreateTable<Client>();
            repo.CreateTable<Transaction>();
            repo.CreateTable<Event>();

            var client = Utility.API.Services.Infrastructure.Helpers.InsertClient(repo, "Halifax_CSV_Importer", default, default);
            foreach (var (file, name) in unMatchedFiles(repo))
            {
                Console.WriteLine(file);

                var records = Csv.GetRecords(file);

                Console.WriteLine("retrieved " + records.Length);

                var newRecords = records.Except(oldRecords ??= repo.Table<Transaction>().ToArray()).ToArray();

                repo.RunInTransaction(() =>
                {
                    int insert = repo.InsertAll(newRecords);

                    repo.Insert(new Event
                    {
                        Guid = Guid.NewGuid(),
                        ParentId = client.Guid,
                        Time = dateNow,
                        Source = name,
                        Type = Type,

                    });

                    Console.WriteLine("inserted " + insert + " rows");
                });
            }
            static IEnumerable<(string, string)> unMatchedFiles(SQLiteConnection repo) =>
                from file in Directory.EnumerateFiles(halifaxDataFolder)
                let date = File.GetCreationTime(file)
                let name = Path.GetFileNameWithoutExtension(file)
                join ev in repo.Table<Event>() on name equals ev.Source into gj
                from ev in gj.DefaultIfEmpty() // left join
                where ev == null && regex.IsMatch(name)
                orderby date
                select (file, name);
        }


        class Csv
        {
            public static Transaction[] GetRecords(string file)
            {
                using var reader = new StreamReader(file);
                return toArray(reader);
            }

            //public static HalifaxRow[] GetRecords2(FileInfo file)
            //{
            //    using var read = file.OpenRead();
            //    using var reader = new StreamReader(read);
            //    return GetRecords3(reader);
            //}

            static Transaction[] toArray(StreamReader reader)
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null,
                    ReadingExceptionOccurred = ex =>
                    {
                        // Log or handle exception
                        return true;
                    }
                };

                using var csv = new CsvReader(reader, config);
                csv.Context.RegisterClassMap<Map>();
                return csv.GetRecords<Transaction>().ToArray();
            }

            sealed class Map : ClassMap<Transaction>
            {
                public Map()
                {
                    AutoMap(CultureInfo.InvariantCulture);
                    Map(m => m.CreditAmount).Convert(row => row.Row["Credit Amount"] is { } c && c.Length > 0 ? (int?)(double.Parse(c) * 100) : (int?)null);
                    Map(m => m.Balance).Convert(row => (int)(double.Parse(row.Row["Balance"]) * 100));
                    Map(m => m.DebitAmount).Convert(row => row.Row["Debit Amount"] is { } c && c.Length > 0 ? (int?)(double.Parse(c) * 100) : (int?)null);
                    Map(m => m.SortCode).Convert(row =>
                    int.Parse(row.Row["Sort Code"].TakeDigits()));
                    Map(m => m.AccountNumber).Convert(row => int.Parse(row.Row["Account Number"]));
                    Map(m => m.Date).Convert(row => DateTime.Parse(row.Row["Transaction Date"]));
                    Map(m => m.Description).Name("Transaction Description");
                    Map(m => m.Type).Convert(row => row.Row["Transaction Type"] is { } s && s.Length == 0 ? TransactionType.None : (TransactionType)Enum.Parse(typeof(TransactionType), row.Row["Transaction Type"]));
                }

                //30/12/2019,DD,'11-08-52,00864878,NW WORLD MASTERCAR,36.92,,3115.81

            }
        }

    }



    static class Helper
    {
        private static readonly char[] Digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };

        public static string TakeDigits(this string input) => string.Concat(input.Join(Digits, a => a, a => a, (a, b) => a == b ? a : char.MinValue));

    }
}



