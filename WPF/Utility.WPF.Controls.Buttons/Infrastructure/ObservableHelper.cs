using System;
using System.Linq;
using System.Reactive.Linq;

namespace Utility.WPF.Controls.Buttons.Infrastructure
{
    public static class ObservableHelper
    {
        public static IObservable<(bool? isChecked, object value)> SelectToggles(this SwitchControl switchView)
        {
            return Observable.FromEventPattern<SwitchControl.ToggleEventHandler, SwitchControl.ToggleEventArgs>(
                   a => switchView.ButtonToggle += a,
                   a => switchView.ButtonToggle -= a)
                .Select(a => (a.EventArgs.IsChecked, a.EventArgs.Key));
        }

        //public static IObservable<(Access access, Dock dock)> SelectOpenClosingChanges(this DrawerHost drawerHost)
        //{
        //    return ClosingEvents(drawerHost).Merge(OpenEvents(drawerHost));

        //    static IObservable<(Access access, Dock dock)> ClosingEvents(DrawerHost drawerHost)
        //    {
        //        return Observable.FromEventPattern<EventHandler<DrawerClosingEventArgs>, DrawerClosingEventArgs>(
        //               a => drawerHost.DrawerClosing += a,
        //               a => drawerHost.DrawerClosing -= a)
        //            .Select(a => (Access.Open, a.EventArgs.Dock));
        //    }

        //    static IObservable<(Access access, Dock dock)> OpenEvents(DrawerHost drawerHost)
        //    {
        //        return Observable.FromEventPattern<EventHandler<DrawerOpenedEventArgs>, DrawerOpenedEventArgs>(
        //               a => drawerHost.DrawerOpened += a,
        //               a => drawerHost.DrawerOpened -= a)
        //            .Select(a => (Access.Open, a.EventArgs.Dock));
        //    }
        //}
    }
}