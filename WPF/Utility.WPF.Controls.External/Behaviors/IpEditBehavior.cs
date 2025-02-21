using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace AR.WPF.Behaviors
{
    /// <summary>
    /// <a href="https://github.com/kurchavy/IpEditBehavior"></a>
    /// </summary>
    public class IpEditBehavior : Behavior<TextBox>
    {
        private string _ip = "";
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.TextChanged += AssociatedObject_TextChanged;
            _ip = this.AssociatedObject.Text;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
        }



        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            var pointsCount = this.AssociatedObject.Text.Count(c => c == '.');
            var splt = this.AssociatedObject.Text.Split('.');

            if (pointsCount > 3)
                UndoChange(e);
            else
            {
                foreach (var ot in splt)
                    if (!VerifyOctetText(ot, e))
                        return;
                _ip = this.AssociatedObject.Text;
            }
        }

        private bool VerifyOctetText(string txt, TextChangedEventArgs e)
        {
            if ((byte.TryParse(txt, out var oct) && txt.Length <= 3) || string.IsNullOrEmpty(txt))
            {
                return true;
            }

            UndoChange(e);
            return false;
        }

        private void UndoChange(TextChangedEventArgs e)
        {
            this.AssociatedObject.Text = _ip;
            var firstChange = e.Changes.First();
            if (firstChange != null)
                this.AssociatedObject.CaretIndex = firstChange.Offset;
        }

    }

}