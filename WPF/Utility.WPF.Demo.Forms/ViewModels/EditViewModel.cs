using MintPlayer.ObservableCollection;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Utility.Helpers.Ex;
using Utility.Models;
using Utility.PropertyNotifications;
using Utility.ViewModels;

namespace Utility.WPF.Demo.Forms.ViewModels
{
    public interface ICollection<T>
    {
        IReadOnlyCollection<T> Collection { get; set; }
    }

    public class EditViewModel : ViewModel, ICollection<INotifyPropertyChanged>, IEquatable<EditViewModel>
    {
        public EditViewModel() : base("Edit")
        {
            (this as ICollection<INotifyPropertyChanged>).WhenAnyValue(a => a.Collection)
                .Select(a => a.ToObservable())
                .Switch()
                .SelectMany(a => a.WhenChanged())
                .Subscribe(a => this.RaisePropertyChanged());
        }

        private void TitleViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public override bool Equals(object? obj)
        {
            return obj is EditViewModel model && TitleViewModel.Title == model.TitleViewModel.Title;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TitleViewModel);
        }

        public bool Equals(EditViewModel? other)
        {
            return TitleViewModel.Title == other?.TitleViewModel.Title;
        }

        //public Guid Id { get => id; set => this.RaiseAndSetIfChanged(ref id, value); }

        public TitleViewModel TitleViewModel { get; private set; } = new();

        public NotesViewModel DescriptionViewModel { get; private set; } = new("Description");

        public ImagesViewModel ImagesViewModel { get; private set; } = new("Images");

        public NotesViewModel ShippingViewModel { get; private set; } = new("Shipping");

        public NotesViewModel DisclaimersViewModel { get; private set; } = new("Disclaimers");

        public MeasurementsViewModel MeasurementsViewModel { get; private set; } = new("Measurements", new[] { new MeasurementViewModel() });

        IReadOnlyCollection<INotifyPropertyChanged> ICollection<INotifyPropertyChanged>.Collection
        {
            get
                => new INotifyPropertyChanged[]
                {
                    TitleViewModel, DescriptionViewModel, ImagesViewModel, MeasurementsViewModel, ShippingViewModel, DisclaimersViewModel
                };
            set
            {
                foreach (var item in value)
                {
                    (item switch
                    {
                        TitleViewModel vm => () => TitleViewModel = vm,
                        NotesViewModel { Key: "Description" } vm => new Action(() => DescriptionViewModel = vm),
                        ImagesViewModel { Key: "Images" } vm => () => ImagesViewModel = vm,
                        NotesViewModel { Key: "Shipping" } vm => () => ShippingViewModel = vm,
                        NotesViewModel { Key: "Disclaimers" } vm => () => DisclaimersViewModel = vm,
                        MeasurementsViewModel vm => () => MeasurementsViewModel = vm,
                        _ => throw new NotImplementedException(),
                    }).Invoke();
                }
                IReactiveObjectExtensions.RaisePropertyChanged(this);
            }
        }

        public override ICollection Children => new ObservableCollection<INotifyPropertyChanged>((this as ICollection<INotifyPropertyChanged>).Collection);

        public override Property Model { get; }

        public static bool operator ==(EditViewModel? left, EditViewModel? right)
        {
            return EqualityComparer<EditViewModel>.Default.Equals(left, right);
        }

        public static bool operator !=(EditViewModel? left, EditViewModel? right)
        {
            return !(left == right);
        }
    }
}