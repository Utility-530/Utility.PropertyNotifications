using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace animTextBox
{
    /// <summary>
    /// <a href="https://www.codeproject.com/Tips/1111076/WPF-TextBox-with-Animated-Overflow"></a>
    /// </summary>
    public partial class AnimatedTextBox : Grid
    {
        private HeightAnimation anim;
        private double _animationDuration;

        public AnimatedTextBox()
        {
            InitializeComponent();

            // Initialize the animation
            anim = new HeightAnimation(this);
            AnimationDuration = 500; //default value

            // Add the handlers to the required events
            txtHidden.SizeChanged += TxtHidden_SizeChanged;
            txtVisible.TextChanged += TxtVisible_TextChanged;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is animated on loaded.
        /// </summary>
        public bool AnimateOnLoaded { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the control is animated.
        /// </summary>
        public bool IsAnimated { get; set; } = true;

        /// <summary>
        /// Gets or sets the duration of the animation.
        /// </summary>
        public double AnimationDuration
        {
            get { return _animationDuration; }
            set
            {
                _animationDuration = value;
                anim.Duration = new Duration(TimeSpan.FromMilliseconds(value));
            }
        }

        /// <summary>
        /// Gets or sets the text contents of the AnimatedTextBox.
        /// </summary>
        public string Text
        {
            get { return txtHidden.Text; }
            set
            {
                txtHidden.Text = value;
                txtVisible.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets how the AnimatedTextBox should wrap text.
        /// </summary>
        public TextWrapping TextWrapping
        {
            get { return txtHidden.TextWrapping; }
            set
            {
                txtHidden.TextWrapping = value;
                txtVisible.TextWrapping = value;
            }
        }

        private void TxtVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            // When the user's writing in txtVisible, we copy the text to txtHidden
            txtHidden.Text = txtVisible.Text;
        }

        private void TxtHidden_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            /* If the property IsAnimated is set to true :
             *
             *     - If the property AnimateOnLoaded is set to true,
             *       And the Height property has changed,
             *     - Or the property AnimateOnLoaded is set to false,
             *       And the AnimatedTextBox is Loaded
             *       And the Height property has changed,
             *     - Then we go. */
            if (IsAnimated && ((AnimateOnLoaded && e.HeightChanged) || (!AnimateOnLoaded && this.IsLoaded && e.HeightChanged)))
            {
                OnHeightChanged(e.PreviousSize.Height, e.NewSize.Height);
            }
        }

        /// <summary>
        /// To execute when the txtHidden's Height has changed.
        /// </summary>
        private void OnHeightChanged(double previousHeight, double newHeight)
        {
            /* Set the type of the Height change :
             *     - if (newHeight > previousHeight) then Height has increased, else the Height has decreased */
            anim.ChangeType = (newHeight > previousHeight) ? HeightAnimation.ChangeTypes.Increased : HeightAnimation.ChangeTypes.Decreased;

            // Animate the Height from the txtHidden's previousHeight to its newHeight
            anim.From = previousHeight;
            anim.To = newHeight;

            // Start the animation
            anim.BeginAnimation();
        }

        /// <summary>
        /// Manages the AnimatedTextBox Height's animation.
        /// </summary>
        private class HeightAnimation
        {
            private Storyboard sb;
            private DoubleAnimation anim;
            private double _from;
            private double _to;
            private Duration _duration;
            private FrameworkElement _fe;
            private ChangeTypes _changeType;

            /// <summary>
            /// The possible types of the Height change.
            /// </summary>
            public enum ChangeTypes
            {
                Increased,
                Decreased
            }

            /// <summary>
            /// Constructor of the class.
            /// </summary>
            public HeightAnimation(FrameworkElement fe)
            {
                // Set the FrameworkElement which manages the animation
                _fe = fe;

                // Initialize the Storyboard
                sb = new Storyboard { AutoReverse = false };

                // Initialize the animation
                anim = new DoubleAnimation
                {
                    Name = "anim",
                    EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut }
                };

                // Set the EasingFunction on a new instance of CubicEase whose EasingMode is EaseInOut

                // Bind the Animation with the txtVisible TextBox
                Storyboard.SetTargetName(anim, "txtVisible");

                // Add the animation to the Storyboard's children
                sb.Children.Add(anim);
            }

            /// <summary>
            /// Gets or sets the type of the Height change.
            /// </summary>
            public ChangeTypes ChangeType
            {
                get => _changeType;
                set
                {
                    _changeType = value;

                    /* If the Height has inreased, set the target property to MaxHeight, else to MinHeight
                     * (instead of animating directly the Height, we animate MaxHeight/MinHeight to prevent the AnimatedTextBox
                     * from growing/shrinking suddenly) */
                    Storyboard.SetTargetProperty(anim, new PropertyPath($"(TextBox.{((value == ChangeTypes.Increased) ? "MaxHeight" : "MinHeight")})"));
                }
            }

            /// <summary>
            /// Gets or sets the animation's starting Height.
            /// </summary>
            public double From
            {
                get { return _from; }
                set
                {
                    _from = value;
                    anim.From = value;
                }
            }

            /// <summary>
            /// Gets or sets the animation's ending Height.
            /// </summary>
            public double To
            {
                get { return _to; }
                set
                {
                    _to = value;
                    anim.To = value;
                }
            }

            /// <summary>
            /// Gets or sets the animation's duration.
            /// </summary>
            public Duration Duration
            {
                get { return _duration; }
                set
                {
                    _duration = value;
                    anim.Duration = value;
                }
            }

            /// <summary>
            /// Begins the animation.
            /// </summary>
            public void BeginAnimation()
            {
                _fe.BeginStoryboard(sb);
            }
        }
    }
}