using DryIoc;
using MintPlayer.ObservableCollection;

namespace Utility.PropertyTrees.WPF.Demo.Views
{
    /// <summary>
    /// Interaction logic for TemplatesView.xaml
    /// </summary>
    public partial class TemplatesView : UserControl
    {
        public TemplatesView(IContainer container)
        {
            InitializeComponent();
            this.DataContext = new TemplateController(container);
        }
    }

    public class TemplateController
    {
        private readonly IContainer container;

        ContentTemplateSelector selector => container.Resolve<ContentTemplateSelector>();

        public TemplateController(IContainer container)
        {
            this.container = container;
            selector.Subscribe(a =>
            {
                Events.Add(a);
            });
        }


        public ObservableCollection<SelectTemplateEvent> Events { get; } = new();
    }
}
