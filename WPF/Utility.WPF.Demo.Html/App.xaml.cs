using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TheArtOfDev.HtmlRenderer.Demo.Common;
using TheArtOfDev.HtmlRenderer.Demo.WPF;
using TheArtOfDev.HtmlRenderer.WPF;
using Utility.WPF.Controls.Html;

namespace Utility.WPF.Demo.Html
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string PerformanceSamplesTreeNodeName = "Performance Samples";

        protected override void OnStartup(StartupEventArgs e)
        {
            SamplesLoader.Init("WPF", typeof(HtmlRender).Assembly.GetName().Version.ToString());

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(160) });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            var treeSel = new TreeSelectionView();
            //var mainView = new HtmlView();
            //var mainView = new Controls.Html.HtmlEditorControl();
            var mainView = new Controls.Html.TinyHtmlEditorControl();
            Grid.SetColumn(mainView, 1);
            treeSel.LoadSamples();
            treeSel.SelectionChanged += (s, e) =>
            {
                if (e.Data.Item.Parent is TreeViewItem tvi)
                {
                    if (!Equals(tvi.Header, PerformanceSamplesTreeNodeName))
                    {
                        //mainView.SetColoredText(e.Data.Sample.Html);
                    }
                }
                else
                {
                    return;
                }



                mainView.Html = e.Data.Sample.Html;
            };
            grid.Children.Add(treeSel);
            grid.Children.Add(mainView);
            var window = new Window { Content = grid };
            window.Show();
            base.OnStartup(e);
        }
    }

}
