namespace Utility.WPF.Abstract
{
    public interface IChange
    {
        event CollectionChangedEventHandler Change;
    }
}