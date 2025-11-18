using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Utility.Commands;
using Utility.PropertyNotifications;

namespace Utility.WPF.Demo.Common.ViewModels
{
    public record struct TickChange(string Text, bool NewValue);

    public struct Tick
    {
        public int Id;
        public string Text;
        public bool IsChecked;
    }

    public class TickViewModel : NotifyPropertyClass, IObservable<TickChange>, IEquatable<TickViewModel>
    {
        private readonly ReplaySubject<TickChange> tickChanges = new();
        private bool isChecked;
        private ICommand command;

        public TickViewModel(Guid id, string text)
        {
            Id = id;
            Text = text;
        }

        public TickViewModel([CallerMemberName] string? name = null)
        {
            throw new Exception("sdfsf444");
        }

        public Guid Id { get; init; }

        public bool IsChecked { get => isChecked; set => this.RaisePropertyChanged(ref isChecked, value); }

        public string Text { get; init; }

        public ICommand Command => command ??= CreateCommand();

        public IDisposable Subscribe(IObserver<TickChange> observer)
        {
            return tickChanges.Subscribe(observer);
        }

        private ICommand CreateCommand()
        {
            var command = new Command(() =>
            {
                IsChecked = !IsChecked;
                tickChanges.OnNext(new TickChange(Text!, IsChecked));
                //this.WhenAnyValue(a => a.Text).Select(a => a != null)
            });
            return command;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as TickViewModel);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text);
        }

        public bool Equals(TickViewModel? other)
        {
            return other is TickViewModel model && Text == model.Text;
        }

        public override string? ToString()
        {
            return Text;
        }

        public static bool operator ==(TickViewModel? left, TickViewModel? right)
        {
            return EqualityComparer<TickViewModel>.Default.Equals(left, right);
        }

        public static bool operator !=(TickViewModel? left, TickViewModel? right)
        {
            return !(left == right);
        }
    }
}