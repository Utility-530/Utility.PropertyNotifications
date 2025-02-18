using Jellyfish;
using PropertyTools.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utility.WPF.Demo.External
{
    /// <summary>
    /// Interaction logic for PushButtonUserControl.xaml
    /// </summary>
    public partial class PushButtonUserControl : UserControl
    {
        public PushButtonUserControl()
        {
            InitializeComponent();
        }
    }

    public class ViewModel
    {
        // Note: Prism MVVM is installed only to provide the DelegateCommand implementation of ICommand
        public ViewModel()
        {
            SayHello = new DelegateCommand(Hello);
            SayHelloMe = new RelayCommand<string>(HelloMe);
        }

        public ICommand SayHello { get; }
        public ICommand SayHelloMe { get; }

        public void HelloMe(string name)
        {
            MessageBox.Show("Hello " + name);
        }

        public void Hello()
        {
            // Test with no command params
            // If you're testing the press and release, notice that the message box blocks the thread
            MessageBox.Show("Hello !");
        }
    }

    namespace TouchFriendlyControls
    {
        public delegate void PressedChangedEventHandler(object sender, bool isPressed);

        public partial class PushButton : Button
        {
            public event PressedChangedEventHandler PressedChanged;

            public static readonly DependencyProperty PressChangedCommandProperty = DependencyProperty.Register(
            "PressChangedCommand", typeof(ICommand), typeof(PushButton), new PropertyMetadata(default(ICommand)));

            public static readonly DependencyProperty PressChangedCommandParameterProperty = DependencyProperty.Register(
                "PressChangedCommandParameter", typeof(object), typeof(PushButton), new PropertyMetadata(default(object)));

            public static readonly DependencyProperty ExecuteOnPressAndReleaseProperty = DependencyProperty.Register(
                "ExecuteOnPressAndRelease", typeof(bool), typeof(PushButton), new PropertyMetadata(default(bool)));

            /// <summary>
            /// Command that gets executed on press (default). You can configure <see cref="ExecuteOnPressAndRelease"/>
            /// to make the command fire on press and release
            /// </summary>
            public ICommand PressChangedCommand
            {
                get { return (ICommand)GetValue(PressChangedCommandProperty); }
                set { SetValue(PressChangedCommandProperty, value); }
            }

            /// <summary>
            /// Parameter for <see cref="PressChangedCommand"/>
            /// </summary>
            public object PressChangedCommandParameter
            {
                get { return (object)GetValue(PressChangedCommandParameterProperty); }
                set { SetValue(PressChangedCommandParameterProperty, value); }
            }

            /// <summary>
            /// Defines whether the button will execute the command on press only or press and release -theoretically-.
            /// in case you want release only, just use the normal Button Command
            /// test this by trying to do: click -> keep mouse down for a while -> release
            /// </summary>
            public bool ExecuteOnPressAndRelease
            {
                get { return (bool)GetValue(ExecuteOnPressAndReleaseProperty); }
                set { SetValue(ExecuteOnPressAndReleaseProperty, value); }
            }

            public PushButton()
            {
            }

            protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
            {
                OnPressedChanged();
                base.OnIsPressedChanged(e);
            }

            private void OnPressedChanged()
            {
                PressedChanged?.Invoke(this, this.IsPressed);

                // Don't execute the command on release

                if (IsPressed)
                {
                    FireCommand();
                }
                else
                {
                    // TODO I'm not sure if it executes on the "Release" correctly
                    // try to do: click -> keep mouse down for a while -> release
                    if (ExecuteOnPressAndRelease)
                        FireCommand();
                }
            }

            private void FireCommand()
            {
                if (PressChangedCommand != null && PressChangedCommand.CanExecute(PressChangedCommandParameter))
                {
                    PressChangedCommand.Execute(PressChangedCommandParameter);
                }
            }
        }
    }
}