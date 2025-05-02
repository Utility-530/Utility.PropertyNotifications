using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Linq;
using Microsoft.Xaml.Behaviors;

namespace Leepfrog.WpfFramework.Actions
{
    /// <summary>
    /// Trigger action just invoke a delegate
    /// </summary>
    public class InvokeDelegateAction : TriggerAction<UIElement>
    {
        private Action _action;
        public Action Action
        {
            get { return _action; }
            set { _action = value; }
        }

        protected override void Invoke(object parameter)
        {
            Action();
        }
    }
}
