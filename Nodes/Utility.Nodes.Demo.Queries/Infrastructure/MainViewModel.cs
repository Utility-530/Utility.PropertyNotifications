using Bogus.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QueryDesignerCore;
using QueryDesignerCore.Demo.WPF;
using System.IO;
using System.Windows.Input;
using Utility.Commands;
using Utility.Helpers;
using Utility.ViewModels;
using Utility.WPF.Controls.Objects;

namespace Utility.Nodes.Demo.Queries
{
    internal class MainViewModel : NotifyPropertyChangedBase
    {
        private const string test = "../../../Data/test.json";

        private JToken filter;
        private Lazy<User[]> users = new(() =>
        {
            string json = ResourceHelper.GetEmbeddedResource("Users.json").AsString();
            return JsonConvert.DeserializeObject<User[]>(json);
        });
        private Command refreshCommand, saveCommand;


        public MainViewModel()
        {
        }

        public JToken Filter
        {
            get
            {
                if (filter == null)
                {
                    string json2 = System.IO.File.ReadAllText(test);

                    // Deserialize the JSON into a list of users
                    //var x = JsonConvert.DeserializeObject<TreeFilter>(json2);
                    filter = JToken.Parse(json2);
                }
                return filter;
            }
        }




        public User[] Users { get; set; }

        public ICommand RefreshCommand
        {
            get
            {
                return refreshCommand ??= new Command(() =>
                {
                    Users = users.Value.Where(Filter.ToObject<TreeFilter>()).ToArray();
                    this.RaisePropertyChanged(nameof(Users));
                });
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return saveCommand ??= new Command(() =>
                {
                    Utility.Helpers.StreamHelper.OverWriteFile(new FileInfo(test), Filter.ToString());

                });
            }
        }


    }

    public static class SchemaLoader
    {
        public static Schema LoadSchema()
        {
            Schema schema = JsonConvert.DeserializeObject<Schema>(Utility.Helpers.ResourceHelper.GetEmbeddedResource("TreeFilter.schema.json").AsString());
            return schema;
        }
    }
}
