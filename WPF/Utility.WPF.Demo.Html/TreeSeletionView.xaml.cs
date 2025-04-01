using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Demo.Common;
using TheArtOfDev.HtmlRenderer.WPF;


namespace Utility.WPF.Demo.Html
{
    public class HtmlSelectionChangedArgs 
    {
        public HtmlSelectionChangedArgs(TreeViewItem item, HtmlSample? sample)
        {
            Item = item;
            Sample = sample;
        }

        public TreeViewItem Item { get; }
        public HtmlSample? Sample { get; }
    }
    /// <summary>
    /// Interaction logic for TreeSeletionView.xaml
    /// </summary>
    public partial class TreeSelectionView : UserControl
    {



        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler<HtmlSelectionChangedArgs>), typeof(TreeSelectionView));
        /// <summary>
        /// the name of the tree node root for all performance samples
        /// </summary>

        public TreeSelectionView()
        {
            InitializeComponent();

        }

        /// <summary>
        /// Loads the tree of document samples
        /// </summary>
        public void LoadSamples()
        {
            var showcaseRoot = new TreeViewItem
            {
                Header = "HTML Renderer"
            };
            _samplesTreeView.Items.Add(showcaseRoot);

            foreach (var sample in SamplesLoader.ShowcaseSamples)
            {
                AddTreeItem(showcaseRoot, sample);
            }

            var testSamplesRoot = new TreeViewItem();
            testSamplesRoot.Header = "Test Samples";
            _samplesTreeView.Items.Add(testSamplesRoot);

            foreach (var sample in SamplesLoader.TestSamples)
            {
                AddTreeItem(testSamplesRoot, sample);
            }

            if (SamplesLoader.PerformanceSamples.Count > 0)
            {
                var perfTestSamplesRoot = new TreeViewItem();
                perfTestSamplesRoot.Header = App.PerformanceSamplesTreeNodeName;
                _samplesTreeView.Items.Add(perfTestSamplesRoot);

                foreach (var sample in SamplesLoader.PerformanceSamples)
                {
                    AddTreeItem(perfTestSamplesRoot, sample);
                }
            }

            showcaseRoot.IsExpanded = true;

            if (showcaseRoot.Items.Count > 0)
                ((TreeViewItem)showcaseRoot.Items[0]).IsSelected = true;
        }

        /// <summary>
        /// Add an html sample to the tree and to all samples collection
        /// </summary>
        private void AddTreeItem(TreeViewItem root, HtmlSample sample)
        {
            var html = sample.Html.Replace("$$Release$$", typeof(TheArtOfDev.HtmlRenderer.WPF.HtmlPanel).Assembly.GetName().Version.ToString());

            var node = new TreeViewItem();
            node.Header = sample.Name;
            node.Tag = new HtmlSample(sample.Name, sample.FullName, html);
            root.Items.Add(node);
        }

        /// <summary>
        /// On tree view node click load the html to the html panel and html editor.
        /// </summary>
        private void OnTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = ((TreeViewItem)e.NewValue);
            var sample = item.Tag as HtmlSample;
            this.RaiseEvent(new TheArtOfDev.HtmlRenderer.WPF.RoutedEvenArgs<HtmlSelectionChangedArgs>(SelectionChangedEvent, new HtmlSelectionChangedArgs(item, sample)));
        }

        public event RoutedEventHandler<HtmlSelectionChangedArgs> SelectionChanged
        {
            add
            {
                AddHandler(SelectionChangedEvent, value);
            }
            remove
            {
                RemoveHandler(SelectionChangedEvent, value);
            }
        }
    }
}
