using ReactiveUI;
using System;
using Utility.Common.Models;
using Utility.Models;
using Utility.ViewModels;

namespace Utility.WPF.Demo.Date.Infrastructure.ViewModels;

public class NoteViewModel : ViewModel, IEquatable<NoteViewModel>, IComparable<NoteViewModel>
{
    private DateTime date;
    private string? text;

    public NoteViewModel(string key) : base(key)
    {
    }

    public string? Text
    {
        get => text;
        set => this.RaiseAndSetIfChanged(ref text, value);
    }

    public DateTime Date
    {
        get => date;
        set => this.RaiseAndSetIfChanged(ref date, value);
    }

    public DateTime CreateTime { get; set; }
    public override Property Model { get; }

    public int CompareTo(NoteViewModel? other)
    {
        return this.CreateTime.CompareTo(other.CreateTime);
    }

    public bool Equals(NoteViewModel? other)
    {
        return this.Model.Equals(other?.Model);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (ReferenceEquals(obj, null))
        {
            return false;
        }

        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public static bool operator ==(NoteViewModel left, NoteViewModel right)
    {
        if (ReferenceEquals(left, null))
        {
            return ReferenceEquals(right, null);
        }

        return left.Equals(right);
    }

    public static bool operator !=(NoteViewModel left, NoteViewModel right)
    {
        return !(left == right);
    }

    public static bool operator <(NoteViewModel left, NoteViewModel right)
    {
        return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
    }

    public static bool operator <=(NoteViewModel left, NoteViewModel right)
    {
        return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
    }

    public static bool operator >(NoteViewModel left, NoteViewModel right)
    {
        return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
    }

    public static bool operator >=(NoteViewModel left, NoteViewModel right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
    }
}