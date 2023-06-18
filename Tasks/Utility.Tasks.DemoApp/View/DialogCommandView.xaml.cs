using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using Utility.Infrastructure;

namespace Utility.Tasks.DemoApp.View
{
    /// <summary>
    /// Interaction logic for AccountButtonView.xaml
    /// </summary>
    public partial class DialogCommandView
    {
        readonly Subject<DialogClosingEventArgs> closeSubject = new();

        public DialogCommandView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.BindCommand(this.ViewModel, vm => vm.Command, v => v.MainButton).DisposeWith(disposable);

                this.ViewModel
                       .Do(a =>
                       {
                           a.Where(c => c.Value).ObserveOnDispatcher().Subscribe(c =>
                           {
                               if (c.Value)
                                   DialogHost.CloseDialogCommand.Execute(default, MainButton);
                           },
                           e =>
                           {

                           },
                           ()=>
                           {
                           });
                       })
                .Select(viewModel =>
                new ViewModelViewHost
                {
                    ViewModel = viewModel,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch
                })

                .SelectMany(view => DialogHost.Show(view, "RootDialog", null, ExtendedClosingEventHandler).ToObservable())
                .Subscribe()
                .DisposeWith(disposable);


                closeSubject
                .WithLatestFrom(this.ViewModel, (a, b) => a)
                .Subscribe(eventArgs =>
                {
                    if (eventArgs.Session.IsEnded == false)
                        eventArgs.Session.Close(false);
                    ViewModel.OnNext(new CloseRequest(ViewModel.Key, true));
                });

                this.OneWayBind(this.ViewModel, vm => vm.Key, v => v.MainButton.Content).DisposeWith(disposable);
            });

            void ExtendedClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
            {
                if ((bool?)eventArgs.Parameter == false) return;
                closeSubject.OnNext(eventArgs);
            }
        }
    }
}
