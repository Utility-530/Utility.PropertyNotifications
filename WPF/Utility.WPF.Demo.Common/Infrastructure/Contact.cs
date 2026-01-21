using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using Bogus.DataSets;
using CsvHelper;
using LanguageExt.Pipes;
using LanguageExt.Pretty;
using Moq;
using NodaTime;
using SQLitePCL;
using Utility.Entities.Comms;
using Utility.Helpers.Ex;

namespace Utility.WPF.Demo.Common.Infrastructure
{
    // Class to represent each row of data
    public record Contact(string Name, string Phone, string Company)
    {
        public bool IsPlaceholder => false;

        public override string ToString()
        {
            return $"Name: {Name}, Phone: {Phone}, Company: {Company}";
        }
    }

    public static class Program
    {
        public static List<Contact> Contacts()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Utility.WPF.Demo.Common.Data.Contacts.csv";
            string csvData;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                csvData = reader.ReadToEnd();
            }

            // Method 1: Parse manually
            return ParseCsv(csvData);
        }

        static List<Contact> ParseCsv(string csvData)
        {
            using var reader = new StringReader(csvData);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<Contact>();
            return new List<Contact>(records);
        }


    }

}
