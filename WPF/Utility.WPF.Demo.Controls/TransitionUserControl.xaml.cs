using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace Utility.WPF.Demo.View
{
    /// <summary>
    /// Interaction logic for TransitionControlUserControl.xaml
    /// </summary>
    public partial class TransitionUserControl : UserControl
    {
        private readonly IEnumerator<PackIconKind> arr;

        public TransitionUserControl()
        {
            InitializeComponent();

            arr = Enum.GetValues(typeof(PackIconKind)).Cast<PackIconKind>().GetEnumerator();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (arr.MoveNext())
            {
                TransitionControl.UnCheckedContent = new PackIcon
                {
                    Width = 70,
                    Height = 70,
                    Margin = new Thickness(4),
                    Kind = arr.Current
                };

                TransitionControl.CheckedContent = new PackIcon
                {
                    Width = 70,
                    Height = 70,
                    Margin = new Thickness(4),
                    Kind = arr.Current
                };
            }
        }
    }
}