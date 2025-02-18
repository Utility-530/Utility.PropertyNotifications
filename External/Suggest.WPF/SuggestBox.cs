using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Popup.WPF;
using Suggest.Infrastructure.Interfaces;


namespace Suggest.WPF {
   public class SuggestBox : PopupTextBox {

      //public static readonly RoutedEvent QueryChangedEvent = EventManager.RegisterRoutedEvent(nameof(QueryChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>), typeof(SuggestBox));
    //  public static readonly DependencyProperty SuggestSourceProperty = DependencyProperty.Register(nameof(SuggestSource), typeof(IAsyncSuggest), typeof(SuggestBox), new PropertyMetadata(null));
      public static readonly DependencyProperty IsSuggestEnabledProperty = DependencyProperty.Register("IsSuggestEnabled", typeof(bool), typeof(SuggestBox), new PropertyMetadata(true));
      public static readonly DependencyProperty SuggestSourceProperty = SuggestBehavior.SuggestSourceProperty.AddOwner(typeof(SuggestBox), new PropertyMetadata(null, Changed));
      SuggestBehavior behavior = new SuggestBehavior();
      private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) {

         if(d is SuggestBox suggestBox) {
            suggestBox.behavior.SuggestSource = (IAsyncSuggest)e.NewValue;
         }
      }

      static SuggestBox() {
         DefaultStyleKeyProperty.OverrideMetadata(typeof(SuggestBox), new FrameworkPropertyMetadata(typeof(SuggestBox)));
      }

      public SuggestBox() {


         this.IsEnabledChanged += (s, e) => behavior.IsEnabled = (bool)e.NewValue;
            

         behavior.Attach(this);
      }

      //public event RoutedPropertyChangedEventHandler<string> QueryChanged {
      //   add => AddHandler(QueryChangedEvent, value);
      //   remove => RemoveHandler(QueryChangedEvent, value);
      //}

      public IAsyncSuggest SuggestSource {
         get => (IAsyncSuggest)GetValue(SuggestSourceProperty);
         set => SetValue(SuggestSourceProperty, value);
      }


      public bool IsSuggestEnabled {
         get { return (bool)GetValue(IsSuggestEnabledProperty); }
         set { SetValue(IsSuggestEnabledProperty, value); }
      }



   }
}
