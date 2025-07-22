using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using Utility.Enums;
using Utility.WPF.Attached;

namespace Utility.WPF.Controls.Buttons
{
    public class DirectionButton : RepeatButton
    {
        static DirectionButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DirectionButton), new FrameworkPropertyMetadata(typeof(DirectionButton)));
        }
        int i = 0;
        public DirectionButton()
        {
            this.Click += (s, e) =>
            {
                this.Movement = convert(this.Movement);
                this.SetValue(Base.IntegerProperty, (++i % 4) + 1);

            };

            static XYTraversal convert(XYTraversal movement)
            {
                switch (movement)
                {
                    case (XYTraversal.BottomToTop):
                        return XYTraversal.RightToLeft;
                    case XYTraversal.None:
                        return XYTraversal.LeftToRight;
                    case XYTraversal.LeftToRight:
                        return XYTraversal.BottomToTop;
                    case XYTraversal.RightToLeft:
                        return XYTraversal.TopToBottom;
                    case XYTraversal.TopToBottom:
                        return XYTraversal.LeftToRight;
                }
                throw new Exception(" 44 df");
            }
        }

        public XYTraversal Movement
        {
            get { return (XYTraversal)GetValue(MovementProperty); }
            set { SetValue(MovementProperty, value); }
        }

        public static readonly DependencyProperty MovementProperty =
            DependencyProperty.Register("Movement", typeof(XYTraversal), typeof(DirectionButton), new PropertyMetadata(XYTraversal.TopToBottom));


    }
}
