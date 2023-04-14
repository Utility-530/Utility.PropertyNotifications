using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Utility.WPF.Attached
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/563195/bind-textbox-on-enter-key-press"></a>
    /// </summary>
    /// <example>
    /// <TextBox Name="itemNameTextBox"
    ///     Text="{Binding Path=ItemName, UpdateSourceTrigger=PropertyChanged}"
    ///     b:InputBindingsManager.UpdatePropertySourceWhenEnterPressed="TextBox.Text"/>
    ///
    /// <Window.Resources>
    ///   <Style TargetType = "{x:Type TextBox}" BasedOn="{StaticResource {x:Type TextBox}}">
    ///      <Style.Setters>
    ///          <Setter Property = "b:InputBindingsManager.UpdatePropertySourceWhenEnterPressed" Value="TextBox.Text"/>
    ///       </Style.Setters>
    ///    </Style>
    ///</Window.Resources>
    /// </example>
    public static class InputBindingsManager
    {
        public static readonly DependencyProperty UpdatePropertySourceWhenEnterPressedProperty = DependencyProperty.RegisterAttached(
                "UpdatePropertySourceWhenEnterPressed", typeof(DependencyProperty), typeof(InputBindingsManager), new PropertyMetadata(null, OnUpdatePropertySourceWhenEnterPressedPropertyChanged));

        static InputBindingsManager()
        {
        }

        public static void SetUpdatePropertySourceWhenEnterPressed(DependencyObject dp, DependencyProperty value)
        {
            dp.SetValue(UpdatePropertySourceWhenEnterPressedProperty, value);
        }

        public static DependencyProperty GetUpdatePropertySourceWhenEnterPressed(DependencyObject dp)
        {
            return (DependencyProperty)dp.GetValue(UpdatePropertySourceWhenEnterPressedProperty);
        }

        private static void OnUpdatePropertySourceWhenEnterPressedPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = dp as UIElement;

            if (element == null)
            {
                return;
            }

            if (e.OldValue != null)
            {
                element.PreviewKeyDown -= HandlePreviewKeyDown;
            }

            if (e.NewValue != null)
            {
                element.PreviewKeyDown += new KeyEventHandler(HandlePreviewKeyDown);
            }
        }

        private static void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoUpdateSource(e.Source);
            }
        }

        private static void DoUpdateSource(object source)
        {
            DependencyProperty property =
                GetUpdatePropertySourceWhenEnterPressed(source as DependencyObject);

            if (property == null)
            {
                return;
            }

            UIElement elt = source as UIElement;

            if (elt == null)
            {
                return;
            }

            BindingExpression binding = BindingOperations.GetBindingExpression(elt, property);

            if (binding != null)
            {
                binding.UpdateSource();
            }
        }
    }
}