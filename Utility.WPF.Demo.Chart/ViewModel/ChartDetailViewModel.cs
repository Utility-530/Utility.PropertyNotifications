using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using static Utility.WPF.Controls.Chart.ViewModels.MultiTimeModel;

namespace Utility.WPF.Chart.Demo.ViewModels
{
    public class ChartDetailViewModel : ReactiveObject, IEquatable<ChartDetailViewModel>
    {
        public string Id { get; }
        public Color Color { get; }

        public ICommand Command { get; }
        public bool IsChecked { get; private set; }

        public ReadOnlyObservableCollection<DateTimePoint> Data { get; }

        public ChartDetailViewModel(string id, Color color, ReadOnlyObservableCollection<DateTimePoint> data)
        {
            Id = id;
            Color = color;
            Data = data;
            Command = ReactiveUI.ReactiveCommand.Create<object, object>(a =>
            {
                IsChecked = !IsChecked;
                this.RaisePropertyChanged(nameof(IsChecked));
                return a;
            });
        }

        public override bool Equals(object obj)
        {
            return obj is ChartDetailViewModel other &&
                  Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public bool Equals(ChartDetailViewModel other)
        {
            return Id == other.Id;
            //Color.Equals(other.Color) &&
            //EqualityComparer<ReadOnlyObservableCollection<DateTimePoint>>.Default.Equals(Data, other.Data);
        }
    }
}