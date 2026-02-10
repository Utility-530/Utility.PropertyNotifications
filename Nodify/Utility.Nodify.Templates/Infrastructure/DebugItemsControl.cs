using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Nodify;
using Utility.Nodes;
using Utility.WPF.Helpers;

namespace Utility.Nodify.Views.Infrastructure
{

    public class KeyValue : INotifyPropertyChanged
    {
        private double value;

        public KeyValue(string key, double value)
        {
            Key = key;
            Value = value;

        }

        public string Key { get; set; }

        public double Value
        { get { return value; } set { this.value = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value))); } }


        public event PropertyChangedEventHandler? PropertyChanged;
    }

    internal class DebugItemsControl : ItemsControl
    {
        private Canvas? canvas;
        Dictionary<string, KeyValue> keyValuePairs = new Dictionary<string, KeyValue>() { 
            { Node_Position_X, new KeyValue(Node_Position_X, 0.0) }, 
            { Node_Position_Y, new KeyValue(Node_Position_Y, 0.0) }, 
            { Scale_X, new KeyValue(Scale_X, 0.0) }, 
            { Scale_Y, new KeyValue(Scale_Y, 0.0) }, 
            { Translate_X, new KeyValue(Translate_X, 0.0) }, 
            { Translate_Y, new KeyValue(Translate_Y, 0.0) }, 
            { Space, new KeyValue(Space, 0.0) }, 
            { _Width, new KeyValue(_Width, 0.0) }, 
            { _Height, new KeyValue(_Height, 0.0) } 
        
        };

        const string Node_Position_X = nameof(Node_Position_X);
        const string Node_Position_Y = nameof(Node_Position_Y);
        const string Scale_X = nameof(Scale_X);
        const string Scale_Y = nameof(Scale_Y);
        const string Translate_X = nameof(Translate_X);
        const string Translate_Y = nameof(Translate_Y);
        const string Space = nameof(Space);
        const string _Width = nameof(Width);
        const string _Height = nameof(Height);

        public NodifyEditor Editor
        {
            get { return (NodifyEditor)GetValue(EditorProperty); }
            set { SetValue(EditorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Editor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditorProperty =
            DependencyProperty.Register(nameof(Editor), typeof(NodifyEditor), typeof(DebugItemsControl), new PropertyMetadata());



        public override void OnApplyTemplate()
        {
            canvas = Editor.FindChild<Canvas>();
            Editor.SelectionChanged += Editor_SelectionChanged;


            ItemsSource = keyValuePairs.Values;
            base.OnApplyTemplate();
        }

        private void Editor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1)
                return;
            if (e.AddedItems.OfType<NodeViewModel>().ToArray() is not { Length: > 0 } array)
                return;
            var node = (ItemContainer)Editor.ItemContainerGenerator.ContainerFromItem(array.First());
            var renderTransform = Editor.ViewportTransform;

            var transformGroup = Editor.ViewportTransform as TransformGroup;
            if (transformGroup == null) return;

            var scaleTransform = transformGroup.Children.OfType<ScaleTransform>().FirstOrDefault();
            var translateTransform = transformGroup.Children.OfType<TranslateTransform>().FirstOrDefault();

            if (scaleTransform == null || translateTransform == null) return;

            // Get ComboBox position
            var nodePosition = node.TransformToAncestor(canvas).Transform(new Point(0, 0));
            var nodePosition2 = node.TransformToAncestor(canvas).Transform(new Point(-translateTransform.X, -translateTransform.Y));
            //var transformedY = (nodePosition.Y * scaleTransform.ScaleY) + translateTransform.Y;
            //var comboBoxBottom = transformedY + node.ActualHeight;

            // Calculate node's bottom edge in canvas coordinates
            var nodeBottom = nodePosition.Y + node.ActualHeight;

            // Apply transformations to get viewport coordinates
            var transformedNodeBottom = (nodeBottom * scaleTransform.ScaleY) + translateTransform.Y;

            // Space from node bottom to canvas bottom in viewport coordinates
            var spaceFromBottom = canvas.ActualHeight - transformedNodeBottom;

            keyValuePairs[Node_Position_X].Value = nodePosition.X;
            keyValuePairs[Node_Position_Y].Value = nodePosition.Y;
            keyValuePairs[Scale_X].Value = scaleTransform.ScaleX;
            keyValuePairs[Scale_Y].Value = scaleTransform.ScaleY;
            keyValuePairs[Translate_X].Value = translateTransform.X;
            keyValuePairs[Translate_Y].Value = translateTransform.Y;
            keyValuePairs[Translate_Y].Value = translateTransform.Y;
            keyValuePairs[Space].Value = spaceFromBottom/ scaleTransform.ScaleY;
            keyValuePairs[_Width].Value = canvas.ActualWidth;
            keyValuePairs[_Height].Value = canvas.ActualHeight;

        }
    }
}
