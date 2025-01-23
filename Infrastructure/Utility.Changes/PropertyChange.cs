namespace Utility.Changes
{
    public record PropertyChange(object Source, string PropertyName, object NewValue, object? PreviousValue = null) : Change(NewValue, PreviousValue, Type.Update);

}
