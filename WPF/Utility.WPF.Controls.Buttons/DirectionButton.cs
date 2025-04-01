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

            static XYMovement convert(XYMovement movement)
            {
                switch (movement)
                {
                    case (XYMovement.BottomToTop):
                        return XYMovement.RightToLeft;
                    case XYMovement.None:
                        return XYMovement.LeftToRight;
                    case XYMovement.LeftToRight:
                        return XYMovement.BottomToTop;
                    case XYMovement.RightToLeft:
                        return XYMovement.TopToBottom;
                    case XYMovement.TopToBottom:
                        return XYMovement.LeftToRight;
                }
                throw new Exception(" 44 df");
            }
        }

        public XYMovement Movement
        {
            get { return (XYMovement)GetValue(MovementProperty); }
            set { SetValue(MovementProperty, value); }
        }

        public static readonly DependencyProperty MovementProperty =
            DependencyProperty.Register("Movement", typeof(XYMovement), typeof(DirectionButton), new PropertyMetadata(XYMovement.TopToBottom));


    }
}
