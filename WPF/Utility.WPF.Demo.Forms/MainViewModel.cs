using ReactiveUI;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Utility.Persists;
using Utility.PropertyNotifications;
using Utility.WPF.Demo.Forms.Infrastructure;
using Utility.WPF.Demo.Forms.Model;
using Utility.WPF.Demo.Forms.ViewModels;

namespace Utility.WPF.Demo.Forms
{
    public class MainViewModel : IReactiveObject
    {
        public MainViewModel()
        {
            MapperFactory.RegisterBsonMapper();

            var databaseService = new LiteDbRepository(new(typeof(EditModel), nameof(EditModel.Id)));
            var mapper = MapperFactory.CreateMapperConfiguration().CreateMapper();

            if (databaseService.FindBy(new FirstOrDefaultQuery()) is { } editModel)
            {
                EditViewModel = mapper.Map<EditViewModel>(editModel);
            }

            EditViewModel
                .WhenChanged()
                .StartWith(new PropertyChange(EditViewModel, default, default, default))
                .Subscribe(a =>
                {
                    EditModel = mapper.Map<EditModel>(a.Source);
                    databaseService.Upsert(EditModel);
                    String = Utility.Helpers.Ex.JsonHelper.Serialize(EditModel);
                    this.RaisePropertyChanged(new(nameof(EditModel)));
                    this.RaisePropertyChanged(new(nameof(String)));
                });
        }

        public EditViewModel EditViewModel { get; } = new() { Id = Guid.NewGuid() };
        public EditModel? EditModel { get; set; }

        public string? String { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public event PropertyChangingEventHandler? PropertyChanging;

        public void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            this.PropertyChanged?.Invoke(this, args);
        }

        public void RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            this.PropertyChanging?.Invoke(this, args);
        }
    }
}