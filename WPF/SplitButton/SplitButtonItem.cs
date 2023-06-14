//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

// ReSharper disable once CheckNamespace

namespace SniffCore.Buttons
{
    /// <summary>
    ///     A single command entry in the <see cref="SplitButton" />.
    /// </summary>
    public class SplitButtonItem : ComboBoxItem
    {
        /// <summary>
        ///     The DependencyProperty for the Command property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SplitButtonItem), new PropertyMetadata(null));

        /// <summary>
        ///     The DependencyProperty for the CommandParameter property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(SplitButtonItem), new PropertyMetadata(null));

        /// <summary>
        ///     The RoutedEvent for the Click event.
        /// </summary>
        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SplitButtonItem));

        static SplitButtonItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButtonItem), new FrameworkPropertyMetadata(typeof(SplitButtonItem)));
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="SplitButtonItem" /> object.
        /// </summary>
        public SplitButtonItem()
        {
            KeyDown += OnKeyDown;
            PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
        }

        /// <summary>
        ///     Gets or sets the command of the button item.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        ///     Gets or sets the command parameter of the button item.
        /// </summary>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        ///     Add or removes the event handler for the click routed event.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
                RaiseClick();
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RaiseClick();
        }

        private void RaiseClick()
        {
            var newEventArgs = new RoutedEventArgs(ClickEvent);
            RaiseEvent(newEventArgs);

            if (Command != null && Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
        }
    }
}