using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using TheArtOfDev.HtmlRenderer.Demo.Common;
using TheArtOfDev.HtmlRenderer.Demo.WPF;
using TheArtOfDev.HtmlRenderer.WPF;
using Utility.WPF.Controls.Base;
using Utility.WPF.Controls.Buttons;
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
            var treeSel = new TreeSelectionView();
            var mainView = new TinyHtmlEditorControl();
            var splitControl = new SplitControl()
            {

                Content = mainView,
                Header = treeSel
            };

            var button = new DirectionButton() { Movement = Enums.XYMovement.RightToLeft, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top };
            splitControl.SetBinding(SplitControl.MovementProperty, new Binding(nameof(DirectionButton.Movement)) { Source = button });

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
            grid.Children.Add(splitControl);
            grid.Children.Add(button);
            var window = new Window { Content = grid };
            window.Show();
            base.OnStartup(e);
        }
    }

}
