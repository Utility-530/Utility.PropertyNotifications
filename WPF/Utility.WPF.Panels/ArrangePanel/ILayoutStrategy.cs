using System.Windows;
//
namespace Utility.WPF.Panels.Infrastructure
{
    public interface ILayoutStrategy
    {
        Size ResultSize { get; }
        void Calculate(Size availableSize, Size[] sizes);
        Rect GetPosition(int index);
        int GetIndex(Point position);
    }
}