
namespace Utility.Interfaces
{
    public interface IRaisePropertyChanged
    {
        public void RaisePropertyChanged(object value, string? propertyName = null);

    }
}
