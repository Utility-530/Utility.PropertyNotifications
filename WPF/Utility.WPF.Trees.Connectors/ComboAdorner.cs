using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Utility.WPF.Trees.Connectors
{
    public class ComboAdorner : Adorner
    {
        private readonly ComboBox _comboBox;
        private readonly Point position;

        public ComboAdorner(UIElement adornedElement, Point position) : base(adornedElement)
        {
            _comboBox = new ComboBox
            {
                Width = 100,
                ItemsSource = new[] { "Option 1", "Option 2", "Option 3" },
            };

            AddVisualChild(_comboBox);
            this.position = position;
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index) => _comboBox;

        protected override Size MeasureOverride(Size constraint)
        {
            _comboBox.Measure(constraint);
            return _comboBox.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _comboBox.Arrange(new Rect(new Point(position.X, position.Y -_comboBox.DesiredSize.Height), _comboBox.DesiredSize));
            return finalSize;
        }

    }

    public class Combo2Adorner : Adorner
    {
        private readonly ComboBox _comboBox;
        private readonly Point position;

        public Combo2Adorner(UIElement adornedElement, Point position) : base(adornedElement)
        {
            _comboBox = new ComboBox
            {
                Width = 100,
                ItemsSource = new[] { "1", "2" },
            };

            AddVisualChild(_comboBox);
            this.position = position;
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index) => _comboBox;

        protected override Size MeasureOverride(Size constraint)
        {
            _comboBox.Measure(constraint);
            return _comboBox.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _comboBox.Arrange(new Rect(new Point(position.X, position.Y - _comboBox.DesiredSize.Height), _comboBox.DesiredSize));
            return finalSize;
        }

    }

}
