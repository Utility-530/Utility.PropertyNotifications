using System.Windows.Controls;

namespace DemoApp
{

    /// <summary>
    /// Interaction logic for MainUserControl.xaml
    /// </summary>
    public partial class MainUserControl : UserControl
    {
        private MainViewModel windowViewModel;

        public MainUserControl()
        {
            InitializeComponent();
            windowViewModel = new MainViewModel();
            //windowViewModel.Progress += ProgressChanged;
            //windowViewModel.Clear += Clear;
            this.DataContext = windowViewModel;

        }

        //private void Clear(object sender, EventArgs e)
        //{

        //    MainCanvas.Children.Clear();

        //}


        //private void ProgressChanged(object sender, ProgressChangedEventArgs<WorkerArgument> e)
        //{
        //    FireflyHelper.Add(MainCanvas, e.UserState.Rand);
        //}



    }
}



