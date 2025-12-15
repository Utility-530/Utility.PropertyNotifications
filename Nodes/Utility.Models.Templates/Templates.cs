namespace Utility.Models.Templates
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Utility.ServiceLocation;

    public partial class Templates : ResourceDictionary
    {
        public Templates()
        {
            this["DataTemplateSelector"] ??= Globals.Resolver.TryResolve<DataTemplateSelector>();
            this["StyleSelector"] ??= Globals.Resolver.TryResolve<StyleSelector>();     
            InitializeComponent();   
        }

        public static Templates Instance { get; } = new();

        public static string EnumTemplate => nameof(EnumTemplate);
        public static string SearchEditor => nameof(SearchEditor);
        public static string MoneySumTemplate => nameof(MoneySumTemplate);
        public static string PriceTemplate => nameof(PriceTemplate);
        public static string ActionTemplate => nameof(ActionTemplate);
        public static string MoneyTemplate => nameof(MoneyTemplate);
        public static string EditTemplate => nameof(EditTemplate);
        public static string Json => nameof(Json);
        public static string BooleanTemplate => nameof(BooleanTemplate);
        public static string DataGridTemplate => nameof(DataGridTemplate);
        public static string ReadOnlyStringTemplate => nameof(ReadOnlyStringTemplate);
        public static string StringTemplate => nameof(StringTemplate);
        public static string NullTemplate => nameof(NullTemplate);
        public static string HtmlEditor => nameof(HtmlEditor);
        public static string HtmlWebViewer => nameof(HtmlWebViewer);
        public static string Html => nameof(Html);
        public static string ChromiumOutput => nameof(ChromiumOutput);
        public static string DirectoryEditor => nameof(DirectoryEditor);
        public static string FilePathEditor => nameof(FilePathEditor);

        public static string HeaderTypeModel => nameof(HeaderTypeModel);
        public static string HeaderTemplate => nameof(HeaderTemplate);
        public static string NameTemplate => nameof(NameTemplate);
        public static string Missing => nameof(Missing);
    }
}
