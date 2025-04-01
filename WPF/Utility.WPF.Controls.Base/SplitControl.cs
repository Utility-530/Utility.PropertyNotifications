using Evan.Wpf;
using PropertyTools.Wpf;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using Utility.Common.Helper;
using Utility.Enums;
using Utility.Reactives;
using Utility.WPF.Abstract;

namespace Utility.WPF.Controls.Base
{
    public class SplitControl : HeaderedContentControl
    {
        public static readonly DependencyProperty MovementProperty =
            DependencyProperty.Register("Movement", typeof(XYMovement), typeof(SplitControl), new PropertyMetadata(XYMovement.LeftToRight));


        static SplitControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitControl), new FrameworkPropertyMetadata(typeof(SplitControl)));
        }

        public SplitControl()
        {
        
        }

        #region properties


        public XYMovement Movement
        {
            get => (XYMovement)GetValue(MovementProperty);
            set => SetValue(MovementProperty, value);
        }

        #endregion properties

   
    }
}