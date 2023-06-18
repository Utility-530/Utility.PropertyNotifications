using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Media;

namespace Utility.Tasks.DemoApp.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView
    {
        public LoginView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                var brush = LoginButton.Foreground;
                this.Bind(this.ViewModel, vm => vm.UserName, v => v.UserNameTextBox.Text)
                    .DisposeWith(disposable);

                this.Bind(this.ViewModel, vm => vm.PassWord, v => v.PassWordTextBox.Password)
                    .DisposeWith(disposable);

                this.OneWayBind(this.ViewModel, vm => vm.Success, v => v.LoginButton.Foreground, a => a.HasValue ? a.Value ? Brushes.Green : Brushes.Red : brush)
                    .DisposeWith(disposable);

                this.BindCommand(this.ViewModel, vm => vm.Check, v => v.LoginButton)
                    .DisposeWith(disposable);

                this.ViewModel.WhenAnyValue(a => a.IsBusy).
                Subscribe(a => MaterialDesignThemes.Wpf.ButtonProgressAssist.SetIsIndicatorVisible(LoginButton, a));

            });
        }
    }
}
