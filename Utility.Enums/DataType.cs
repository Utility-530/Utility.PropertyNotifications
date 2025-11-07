namespace Utility.Enums
{
    public enum DataType
    {
        Money,
        Currency, //decimal / double / long Non-negative, 2 decimal places(cents)  12.50 USD
        Percentage,//  double / int	0–100%	85.5%
        Distance,// double Non-negative, unit specified	12.3 km
        Weight,// / Mass double Non-negative, unit specified	70 kg
        Mass,
        Temperature,// double Usually °C or °F	36.6 °C
        Duration,// / Time Interval    int / long In seconds, milliseconds, or minutes	3600 s
        Speed, // double Unit-dependent 60 km/h
        Interest,// double Percentage	3.5%
        PIN,// string / int Typically 4–6 digits, numeric	"1234"
        Password,// / Passphrase string Min/max length, complexity rules	"S3cur3P@ss"
        Token,// / API Key string Unique, alphanumeric	"AB12-CD34-EF56"
        Hash,// string / byte[] Fixed-length	"5f4dcc3b5aa765d61d8327deb882cf99"
        GUID,// string	36-char canonical format	"550e8400-e29b-41d4-a716-446655440000"
        Barcode,// / QR Code   string Numeric/alphanumeric, specific length	"012345678905"
        Date,// string / long Format YYYY-MM-DD or timestamp	"2025-10-03"
        Time,// string / int Format HH:MM:SS or seconds from midnight	"14:30:00"
        Datetime,// / Timestamp long Milliseconds/seconds since epoch	1735739400000
        Year,
        Month, //int Year: 1900–2100, Month: 1–12	2025, 10
        Weekday, //int	0=Sunday…6=Saturday	5 (Friday)
        Timezone,   // string Standard TZ identifiers	"UTC", "America/New_York"
        Latitude,
        Longitude,// double Latitude −90 to 90, Longitude −180 to 180	37.7749, −122.4194
        PostalCode,
        ZipCode, // string / int Country-specific length	"94103"
        CountryCode,   // string ISO 3166-1 alpha-2	"US"
        PhoneNumber,   // string Format varies by country	"+1-415-555-2671"
        Address,// string Free-form text	"123 Market St, San Francisco, CA"

        //Invoice,// / Order ID  string / int Unique identifier	"INV-2025-001"
        //Account Number  string Length depends on bank	"1234567890"
        IBAN,// / SWIFT Code   string Fixed format	"GB29NWBK60161331926819"

        CreditCardNumber,// string	16 digits, Luhn-valid	"4111111111111111"
        StockTickerSymbol, //string Uppercase letters, 1–5 chars	"AAPL"
        ProductSKU, //string Alphanumeric	"SKU-12345"
        FilePath,   //string OS-specific path	"/usr/local/bin"
        URL,// string Must be valid URI	"https://example.com
        EmailAddress,//   string Must match regex	"user@example.com
        IPAddress,//  string IPv4 or IPv6	"192.168.1.1"
        MACAddress,// string Format XX:XX:XX:XX:XX:XX	"00:1A:2B:3C:4D:5E"
        Color,// string / tuple<int, int, int>`	Hex code or RGB	"#FF5733" or(255,87,51)
        VersionNumber, // string / int Semantic versioning	"1.2.3"
    }
}