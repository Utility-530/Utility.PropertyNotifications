//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

// ReSharper disable once CheckNamespace

namespace SniffCore.Buttons
{
    /// <summary>
    ///     A button with a drop down where more commands can be available.
    /// </summary>
    /// <example>
    ///     <code lang="xaml">
    /// <![CDATA[
    /// <buttons:SplitButton Content="Any Button" Padding="12,4" Command="{Binding SplitButtonCommand}">
    ///     <buttons:SplitButtonItem Content="Sub Item 1" Command="{Binding SplitButtonItemCommand}" CommmandParameter="1" />
    ///     <buttons:SplitButtonItem Content="Sub Item 2" Command="{Binding SplitButtonItemCommand}" CommmandParameter="2" />
    ///     <buttons:SplitButtonItem Content="Sub Item 3" Command="{Binding SplitButtonItemCommand}" CommmandParameter="3" />
    /// </buttons:SplitButton>
    /// 
    /// <buttons:SplitButton Content="Any Button" Padding="12,4" ItemsSource="{Binding Items}" Command="{Binding SplitButtonCommand}">
    ///     <buttons:SplitButton.ItemContainerStyle>
    ///         <Style TargetType="{x:Type buttons:SplitButtonItem}">
    ///             <Setter Property="Command" Value="{Binding DataContext.SplitButtonItemCommand, RelativeSource={RelativeSource AncestorType={x:Type buttons:SplitButton}}}" />
    ///             <Setter Property="CommandParameter" Value="{Binding Index}" />
    ///             <Setter Property="HorizontalContentAlignment" Value="Left" />
    ///         </Style>
    ///     </buttons:SplitButton.ItemContainerStyle>
    ///     <buttons:SplitButton.ItemTemplate>
    ///         <DataTemplate>
    ///             <TextBlock Text="{Binding }" />
    ///         </DataTemplate>
    ///     </buttons:SplitButton.ItemTemplate>
    /// </buttons:SplitButton>
    /// ]]>
    /// </code>
    /// </example>
    [TemplatePart(Name = "PART_MainButton", Type = typeof(ButtonBase))]
    public class SplitButton : ComboBox
    {
        /// <summary>
        ///     The DependencyProperty for the Command property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SplitButton), new PropertyMetadata(null));

        /// <summary>
        ///     The DependencyProperty for the CommandParameter property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(SplitButton), new PropertyMetadata(null));

        /// <summary>
        ///     The DependencyProperty for the Content property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(SplitButton), new PropertyMetadata(null));

        /// <summary>
        ///     The DependencyProperty for the ContentTemplate property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(SplitButton), new PropertyMetadata(null));

        /// <summary>
        ///     The DependencyProperty for the ContentTemplateSelector property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty =
            DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(SplitButton), new PropertyMetadata(null));

        /// <summary>
        ///     The RoutedEvent for the Click event.
        /// </summary>
        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SplitButton));



        public static readonly DependencyProperty PlacementProperty = Popup.PlacementProperty.AddOwner(typeof(SplitButton), new PropertyMetadata(PlacementMode.Bottom));
        
        public static readonly DependencyProperty PlacementRectangleProperty = Popup.PlacementRectangleProperty.AddOwner(typeof(SplitButton), new PropertyMetadata(new Rect()));



        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
        }

        /// <summary>
        ///     Gets or sets the content template for the main button.
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get => (DataTemplate) GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        /// <summary>
        ///     Gets or sets the content template selector for the main button.
        /// </summary>
        public DataTemplateSelector ContentTemplateSelector
        {
            get => (DataTemplateSelector) GetValue(ContentTemplateSelectorProperty);
            set => SetValue(ContentTemplateSelectorProperty, value);
        }

        /// <summary>
        ///     Gets or sets the content of the main button.
        /// </summary>
        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        /// <summary>
        ///     Gets or sets the command of the main button.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        ///     Gets or sets the command parameter of the main button.
        /// </summary>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

         public Rect PlacementRectangle
        {
            get { return (Rect)GetValue(PlacementRectangleProperty); }
            set { SetValue(PlacementRectangleProperty, value); }
        }
 

        /// <summary>
        ///     Add or removes the event handler for the click routed event.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        /// <summary>
        ///     The template of the control got applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_MainButton") is ButtonBase button)
                button.Click += OnMainButtonClick;
        }

        private void OnMainButtonClick(object sender, RoutedEventArgs e)
        {
            var newEventArgs = new RoutedEventArgs(ClickEvent);
            RaiseEvent(newEventArgs);
        }

        /// <summary>
        ///     Checks if the item is already the right container.
        /// </summary>
        /// <param name="item">The item added to the collection.</param>
        /// <returns>True if the items is already the correct child container; otherwise false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SplitButtonItem;
        }

        /// <summary>
        ///     Returns a new child container.
        /// </summary>
        /// <returns>A new child container.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SplitButtonItem();
        }
    }
}