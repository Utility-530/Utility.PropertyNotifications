using AutoMapper;
using LiteDB;
using Newtonsoft.Json;
using Splat;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using TinyHtml.Wpf;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes.Demo.Filters.Services;
using Utility.Nodes.Filters;
using Utility.Nodes.WPF;
using Utility.Repos;
using Utility.WPF.Templates;


namespace Utility.Nodes.Demo.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            Locator.CurrentMutable.RegisterConstant<ITreeRepository>(TreeRepository.Instance);
            //Locator.CurrentMutable.RegisterConstant<INodeSource>(NodeSource.Instance);
            Locator.CurrentMutable.RegisterConstant<INodeSource>(NodeEngine.Instance);
            Locator.CurrentMutable.RegisterConstant<IFilter>(TreeViewFilter.Instance);
            Locator.CurrentMutable.RegisterConstant<IExpander>(Expander.Instance);
            Locator.CurrentMutable.RegisterConstant<IContext>(Context.Instance);
            Splat.Locator.CurrentMutable.RegisterLazySingleton<MainViewModel>(() => new MainViewModel());
            Splat.Locator.CurrentMutable.RegisterLazySingleton<System.Windows.Controls.DataTemplateSelector>(() => CustomDataTemplateSelector.Instance);


            //var x = DataTemplateSelector.Instance;

            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;

            TransformerService service = new();
            ControlsService _service = new();
            ParserService parserService = new();


            var mainViewModel = Locator.Current.GetService<MainViewModel>();
            //WpfHtmlControlBase.SetMasterStylesheet(combine());
            combine();
            var window = new Window() { Content = mainViewModel };

            window.Show();



            base.OnStartup(e);

        }

        void combine()
        {
            string cssFile = "master.css";
            //string cssFile = "pico.min.css";
            // string cssFile = "entireframework.min.css";
            //string cssFile = "pico.classless.blue.css";

            string name = typeof(App).Assembly.GetManifestResourceNames().Where(a => a.Contains(cssFile)).First();

            using (var s = typeof(App).Assembly.GetManifestResourceStream(name))
            {
                WpfHtmlControlBase.SetMasterStylesheet(new StreamReader(s).ReadToEnd());
            }
            //StringBuilder result = new StringBuilder();
            //foreach (var filename in System.IO.Directory.EnumerateFiles("../../../bootstrap-5.3.3-dist/css"))
            //{
            //    using (StreamReader reader = new StreamReader(filename))
            //    {
            //        if (filename.Contains(".css"))
            //        {
            //            result.Append(reader.ReadToEnd());
            //            result.AppendLine();
            //        }                     
            //    }
            //}
            //return result.ToString();            
        }
    }
}
