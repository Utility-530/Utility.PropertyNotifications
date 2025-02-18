using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using Popup.WPF;
using Suggest.WPF.Infrastructure;
using Suggest.Infrastructure.Interfaces;
using Suggest.Infrastructure.Model;

namespace Suggest.WPF {


   public class SuggestBehavior : Behavior<PopupTextBox> {



      protected override void OnAttached() {

         ItemsControl.ItemsSourceProperty.OwnerType.IsAssignableFrom(AssociatedType);
         AssociatedObject.TextChanged += (s, e) => OnTextChanged();
         AssociatedObject.IsVisibleChanged += (s, e) => OnTextChanged();
         base.OnAttached();
      }

      protected override void OnDetaching() {
         AssociatedObject.TextChanged -= (s, e) => OnTextChanged();
         AssociatedObject.IsVisibleChanged -= (s, e) => OnTextChanged();
         base.OnDetaching();
      }


      public FastObservableCollection<object> QueryResults { get; } = new FastObservableCollection<object>(new[] { new Tip("Enter a file-system-path or the Space key") });


      //  public static readonly RoutedEvent QueryChangedEvent = EventManager.RegisterRoutedEvent(nameof(QueryChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>), typeof(SuggestBox));
      //public static readonly DependencyProperty TextChangedCommandProperty = DependencyProperty.Register(nameof(TextChangedCommand), typeof(ICommand), typeof(SuggestBox), new PropertyMetadata(null));
      public static readonly DependencyProperty SuggestSourceProperty = DependencyProperty.Register(nameof(SuggestSource), typeof(IAsyncSuggest), typeof(SuggestBehavior), new PropertyMetadata(null, Changed));
      public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(SuggestBehavior), new PropertyMetadata(true, Changed));

      private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) {
         if (d is SuggestBehavior beh)
            beh.OnTextChanged();
      }

      //public static readonly DependencyProperty QueryProperty = DependencyProperty.Register("Query", typeof(string), typeof(SuggestBehavior), new PropertyMetadata(null));



      public SuggestBehavior() {
         //var a = SuggestSource;
      }

     


      public IAsyncSuggest SuggestSource {
         get => (IAsyncSuggest)GetValue(SuggestSourceProperty);
         set => SetValue(SuggestSourceProperty, value);
      }


      /// <summary>
      /// Gets/sets whether suggestions should currently be queried and viewed or not.
      /// </summary>
      public bool IsEnabled {
         get { return (bool)GetValue(IsEnabledProperty); }
         set { SetValue(IsEnabledProperty, value); }
      }


      ///// <summary>
      ///// Gets/sets a command that should be executed whenever the text in the textbox
      ///// portion of this control has changed.
      ///// </summary>
      //public ICommand TextChangedCommand {
      //   get { return (ICommand)GetValue(TextChangedCommandProperty); }
      //   set { SetValue(TextChangedCommandProperty, value); }
      //}


      /// <summary>
      /// Method executes when new text is entered in the textbox portion of the control.
      /// </summary>
      protected void OnTextChanged() {

         AssociatedObject.IsHintVisible = string.IsNullOrEmpty(AssociatedObject.Text);

         QueryForSuggestions(AssociatedObject, this);
      }

      private static async void QueryForSuggestions(PopupTextBox textBox, SuggestBehavior suggestBehavior) {
         // A change during disabled state is likely to be caused by a bound property
         // in a viewmodel (a machine based edit rather than user input)
         // -> Lets break the message loop here to avoid unnecessary CPU processings...
         if (textBox.IsEnabled == false || textBox.IsLoaded == false)
            return;

         // Text change is likely to be from property change so we ignore it
         // if control is invisible or suggestions are currently not requested
         if (textBox.Visibility != Visibility.Visible || suggestBehavior.IsEnabled == false)
            return;

         if (textBox.ParentWindowIsClosing)
            return;

         if (suggestBehavior.SuggestSource != null) {
            textBox.ItemsSource = suggestBehavior.QueryResults;
            suggestBehavior.QueryResults.Clear();
            suggestBehavior.QueryResults.Add(new Tip("Loading..."));
            var suggestionResult = await suggestBehavior.SuggestSource.SuggestAsync(textBox.Text);
            textBox.ValidText = suggestionResult.IsValid;
            if (suggestionResult.Suggestions != null)
               await Application.Current.Dispatcher.InvokeAsync(() => {
                  suggestBehavior.QueryResults.Clear();
                  suggestBehavior.QueryResults.AddItems(suggestionResult.Suggestions);
               });
         }

         //this.RaiseEvent(new RoutedPropertyChangedEventArgs<string>(string.Empty, Text, QueryChangedEvent));

         //// Check whether this attached behaviour is bound to a RoutedCommand
         //if (this.TextChangedCommand is RoutedCommand command) {
         //   // Execute the routed command
         //   command.Execute(this.Text, this);
         //}
         //else {
         //   // Execute the Command as bound delegate if anything bound
         //   TextChangedCommand?.Execute(this.Text);
         //}
      }
   }
}
