using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Utility.Commands;
using Utility.WPF.Adorners;

namespace Utility.WPF.Demo.Adorners
{
    /// <summary>
    /// Interaction logic for AdornerUser.xaml
    /// </summary>
    public partial class HoverUserControl : UserControl
    {
        public HoverUserControl()
        {
            InitializeComponent();
            TextCommand = new Command(() => TextBlock1.Text += " New Text");
            PlusCommand = new Command(() => TextBlock1.Text += " Plus");
            Grid1.DataContext = this;

            //adornerController = new(Square3Grid);
        }

        //Square3Grid.DataContext = new Characters();
        //var adorner = new SettingsAdorner(Square3Grid);
        //Square3Grid.SetValue(AdornerEx.AdornerProperty, adorner);

        public ICommand TextCommand { get; }
        public ICommand PlusCommand { get; }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (TextBlock1.Text.Length >= 9)
            {
                TextBlock1.Text = TextBlock1.Text.Remove(TextBlock1.Text.Length - 9);
            }
        }
    }

    public class PlusAdornerBehavior : AdornerBehavior
    {
        protected override Adorner CreateAdorner() => new PlusAdorner(AssociatedObject, Command);
    }

    public class PopUpAdornerBehavior : AdornerBehavior
    {
        protected override Adorner CreateAdorner() => new FrameworkElementAdorner<PopupBox>(AssociatedObject as FrameworkElement, () => Application.Current.Resources["PopupBox"] as FrameworkElement);
    }
}