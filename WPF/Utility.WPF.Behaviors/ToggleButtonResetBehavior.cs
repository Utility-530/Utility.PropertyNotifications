using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;

namespace Utility.WPF.Behaviors;



public class ToggleButtonResetBehavior : Behavior<ToggleButton>
{
    /// <summary>
    /// Time in milliseconds after which the toggle button is reset.
    /// </summary>
    public int ResetMilliseconds
    {
        get { return (int)GetValue(ResetMillisecondsProperty); }
        set { SetValue(ResetMillisecondsProperty, value); }
    }

    public static readonly DependencyProperty ResetMillisecondsProperty =
        DependencyProperty.Register(nameof(ResetMilliseconds), typeof(int), typeof(ToggleButtonResetBehavior), new PropertyMetadata(500));

    private DispatcherTimer _timer;

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Checked += AssociatedObject_Checked;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Checked -= AssociatedObject_Checked;
        _timer?.Stop();
        _timer = null;
    }

    private void AssociatedObject_Checked(object sender, RoutedEventArgs e)
    {
        if (_timer == null)
        {
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
        }

        _timer.Interval = TimeSpan.FromMilliseconds(ResetMilliseconds);
        _timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        _timer.Stop();
        AssociatedObject.IsChecked = false;
    }
}
