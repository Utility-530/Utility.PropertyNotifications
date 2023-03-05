using ReactiveUI;
using System;
using System.ComponentModel;
using Utility.Common;
using Utility.Common.Collection;
using Utility.Persists;
using UtilityWpf.Demo.Forms.Infrastructure;
using UtilityWpf.Demo.Forms.Model;
using UtilityWpf.Demo.Forms.ViewModels;

namespace UtilityWpf.Demo.Forms
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
                .Changes(startWithSource: true)
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