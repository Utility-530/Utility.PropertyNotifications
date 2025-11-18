using System;
using System.Reactive;
using System.Windows.Input;
using Utility.Commands;
using Utility.Interfaces.NonGeneric.Data;
using Utility.Persists;
using Utility.PropertyNotifications;
using Utility.WPF.Demo.Common.ViewModels;
using Utility.WPF.Demo.Master.Infrastructure;

namespace Utility.WPF.Demo.Master.ViewModels
{
    public class MasterDetailVariableRepositoryViewModel : MasterDetailViewModel
    {
        public MasterDetailVariableRepositoryViewModel() : base()
        {
            this.WithChangesTo(a => a.DatabaseService)
                .Subscribe(a => { service.OnNext(new(a)); });

            ChangeRepositoryCommand = new Command<bool>((a) =>
            {
                if (DatabaseService is LiteDbRepository service)
                {
                    DatabaseService = new MockDatabaseService();
                }
                else
                    DatabaseService = new LiteDbRepository(new(typeof(ReactiveFields), nameof(ReactiveFields.Id)));

            });
        }

        public ICommand ChangeRepositoryCommand { get; }

        public IRepository DatabaseService { get => repository; private set => this.RaisePropertyChanged(ref repository, value); }
    }
}