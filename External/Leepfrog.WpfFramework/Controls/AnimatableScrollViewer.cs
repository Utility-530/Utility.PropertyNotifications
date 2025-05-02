using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Leepfrog.WpfFramework.Controls
{
    public class AnimatableScrollViewer : ScrollViewer
    {
        //Register a DependencyProperty which has a onChange callback
        public static DependencyProperty CurrentVerticalOffsetProperty = DependencyProperty.Register("CurrentVerticalOffset", typeof(double), typeof(AnimatableScrollViewer), new PropertyMetadata(new PropertyChangedCallback(OnVerticalChanged)));
        public static DependencyProperty CurrentHorizontalOffsetProperty = DependencyProperty.Register("CurrentHorizontalOffset", typeof(double), typeof(AnimatableScrollViewer), new PropertyMetadata(new PropertyChangedCallback(OnHorizontalChanged)));
        public static DependencyProperty FollowControlProperty = DependencyProperty.Register("FollowControl", typeof(FrameworkElement), typeof(AnimatableScrollViewer), new PropertyMetadata(new PropertyChangedCallback(OnFollowControlChanged)));
        public static DependencyProperty FollowDataProperty = DependencyProperty.Register("FollowData", typeof(object), typeof(AnimatableScrollViewer), new PropertyMetadata(new PropertyChangedCallback(OnFollowDataChanged)));
        public static DependencyProperty ReflectChangesInCurrentOffsetProperty = DependencyProperty.Register("ReflectChangesInCurrentOffset", typeof(bool), typeof(AnimatableScrollViewer), new PropertyMetadata(false));


        


        private bool _preventReentry;

        //When the DependencyProperty is changed change the vertical offset, thus 'animating' the scrollViewer
        private static void OnVerticalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatableScrollViewer viewer = d as AnimatableScrollViewer;
            if (viewer._preventReentry)
            {
                return;
            }
            viewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        //When the DependencyProperty is changed change the vertical offset, thus 'animating' the scrollViewer
        private static void OnHorizontalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatableScrollViewer viewer = d as AnimatableScrollViewer;
            if (viewer._preventReentry)
            {
                return;
            }
            viewer.ScrollToHorizontalOffset((double)e.NewValue);
        }

        //When the DependencyProperty is changed change the horizontal/vertical offset to match, thus 'animating' the scrollViewer in time
        private static void OnFollowControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatableScrollViewer viewer = d as AnimatableScrollViewer;
            viewer.onFollowControlChanged(d, e);
        }

        private static void OnFollowDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatableScrollViewer viewer = d as AnimatableScrollViewer;
            viewer.onFollowDataChanged(d, e);
        }

        private void onFollowControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var oldControl = e.OldValue as FrameworkElement;
            var newControl = e.NewValue as FrameworkElement;

            // unsubscribe previous
            if (oldControl != null)
            {
                DependencyPropertyDescriptor.FromProperty(RenderTransformProperty, typeof(FrameworkElement)).RemoveValueChanged(oldControl, RenderTransformPropertyChanged);
                var trans = oldControl.RenderTransform;
                var isFrozen = trans.IsFrozen;
                if (isFrozen)
                {
                    trans = trans.Clone();
                }
                trans.Changed -= RenderTransform_Changed;
                if (isFrozen)
                {
                    oldControl.RenderTransform = trans;
                }
            }
            if (newControl != null)
            {
                RenderTransformPropertyChanged(null, null);
                DependencyPropertyDescriptor.FromProperty(RenderTransformProperty, typeof(FrameworkElement)).AddValueChanged(newControl, RenderTransformPropertyChanged);
            }
            if (newControl?.DataContext != FollowData)
            {
                FollowData = null;
            }
        }

        private void RenderTransformPropertyChanged(object sender, EventArgs e)
        {
            var newControl = FollowControl;
            var trans = newControl.RenderTransform;
            var isFrozen = trans.IsFrozen;
            if (isFrozen)
            {
                trans = trans.Clone();
            }
            trans.Changed += RenderTransform_Changed;
            if (isFrozen)
            {
                newControl.RenderTransform = trans;
            }
            RenderTransform_Changed(null, null);
        }

        private void onFollowDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                return;
            }
            this.AddLog("FOLLOW", $"Trying to follow new data {e.NewValue}");
            FollowControl = findDescendantWithDataContext(e.NewValue);
        }

        private Point _destination = new Point();
        private DateTimeOffset _destinationTime = DateTimeOffset.MinValue;
        private bool _isScrolling = false;

        private void RenderTransform_Changed(object sender, EventArgs e)
        {
            var followControl = FollowControl;

            if (followControl == null)
            {
                return;
            }
            
            // IF LIST HAS BEEN REFRESHED, SO THE FOLLOW CONTROL NO LONGER BELONGS...
            if (!IsAncestorOf(followControl))
            {
                this.AddLog("FOLLOW", "NOT A CHILD!");
                // FIND A NEW MATCH
                FollowControl = findDescendantWithDataContext(FollowData);
                return;
            }

            GeneralTransform childTransform = followControl.TransformToAncestor(this);
            Rect rectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), followControl.RenderSize));
            bool moveToCentre = true;
            /*
            // IF WE'RE ONLY JUST IN VIEW (OR OUT OF VIEW!) AT THE TOP...
            //if (rectangle.Top < (ActualHeight * 0.2))
            {
                //this.AddLog("FOLLOW", $"OUT OF VIEW AT TOP { rectangle.Top } < { ActualHeight * 0.2}");
                moveToCentre = true;
            }
            // IF WE'RE ONLY JUST IN VIEW (OR OUT OF VIEW!) AT THE BOTTOM...
            else if (rectangle.Bottom > (ActualHeight * 0.8))
            {
                //this.AddLog("FOLLOW", $"OUT OF VIEW AT BOTTOM { rectangle.Bottom } > { ActualHeight * 0.8}");
                moveToCentre = true;
            }
            else
            {
                //this.AddLog("FOLLOW", $"STILL IN VIEW { rectangle.Top } - { rectangle.Bottom } IS WITHIN 0 - { ActualHeight }");
            }
            */
            // IF WE NEED TO RECENTRE, OR WE WERE ALREADY RECENTRING...
            if ((moveToCentre) || (_isScrolling))
            {
                var newCentre = new Point(0, rectangle.Top + (rectangle.Height / 2));
                newCentre.Offset(0, CurrentVerticalOffset);
                var newTop = newCentre;
                newTop.Offset(0, -ActualHeight / 2);
                // ADJUST FOR TOP AND BOTTOM
                if (newTop.Y < 0)
                {
                    newTop.Y = 0;
                }
                else if (newTop.Y > ScrollableHeight)
                {
                    newTop.Y = ScrollableHeight;
                    //this.AddLog("FOLLOW", $"TOP WILL BE { newTop.Y }");
                }
                if (_destination.Y != newTop.Y)
                {
                    this.AddLog("FOLLOW", $"TOP WILL BE { newTop.Y }, EXT { ExtentHeight }, SCR { ScrollableHeight }, ACT {ActualHeight}");
                    _destination = newTop;
                    _destinationTime = DateTimeOffset.Now + TimeSpan.FromSeconds(0.5);
                    if (_destination.Y == CurrentVerticalOffset)
                    {
                        if (_isScrolling)
                        {
                            _isScrolling = false;
                            _momentum = 0.0;
                            CompositionTarget.Rendering -= animateFollow;
                        }
                        return;
                    }
                    if (!_isScrolling)
                    {
                        _isScrolling = true;
                        CompositionTarget.Rendering += animateFollow;
                    }
                }
            }
        }

        private FrameworkElement findDescendantWithDataContext(object dataContext)
        {
            if (dataContext == null)
            {
                this.AddLog("FOLLOW", $"DATA IS NULL");
                return null;
            }

            // loop through all level 1 children first
            // then if no match, loop through next level
            var children = new List<FrameworkElement>() { this };

            while (children.Any())
            {
                var childIndex = 0;
                // use previous children as new set of parents!
                var parents = children.Select(child => new Tuple<FrameworkElement, int>(child, VisualTreeHelper.GetChildrenCount(child)) ).ToList();
                children.Clear();
                while (parents.Any())
                { 
                    for (var parentIndex = parents.Count-1; parentIndex >= 0; parentIndex--)
                    {
                        var parent = parents[parentIndex];
                        // if this parent doesn't have enough children...
                        if (childIndex >= parent.Item2)
                        {
                            parents.RemoveAt(parentIndex);
                        }
                        else
                        {
                            // get this child
                            FrameworkElement item = null;
                            try
                            {
                                item = VisualTreeHelper.GetChild(parent.Item1, childIndex) as FrameworkElement;
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                // ignore this error, but we can't use this control 
                                this.AddLog("FOLLOW", "child out of range, ignoring...");
                                parents.RemoveAt(parentIndex);
                            }
                            if (item?.DataContext == dataContext)
                            {
                                //this.AddLog("FOLLOW", $"MATCH! {item}, data={dataContext}={item.DataContext}, childindex={childIndex},parentindex={parentIndex}/{parents.Count}");
                                try
                                {
                                    item = VisualTreeHelper.GetChild(item, 0) as FrameworkElement;
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    // this can't be the right one after all, just keep searching
                                    this.AddLog("FOLLOW", "found right data context, but no child, ignoring...");
                                }
                                return item;
                            }
                            else
                            {
                                children.Add(item);
                            }
                        }
                    }
                    childIndex++;
                }
            }
            // NOTHING FOUND
            this.AddLog("FOLLOW", $"NO MATCH");
            return null;
        }


        private double _momentum = 0.0;

        private void animateFollow(object sender, EventArgs e)
        {
            //this.AddLog("FOLLOW", $"ANIMATING FROM { CurrentVerticalOffset } TO { _destination.Y }");

            var diff = _destination.Y - CurrentVerticalOffset;

            if (Math.Abs(diff) < 2)
            {
                _isScrolling = false;
                _momentum = 0.0;
                CompositionTarget.Rendering -= animateFollow;
                this.AddLog("FOLLOW", "(done)");
                return;
            }

            var total = 0.0;
            var i = 0;
            // SLOWDOWN GRACEFULLY!
            for (i = 1; i <= 20; i++)
            {
                total += i * (20-i);
                if (Math.Abs(diff) <= total)
                {
                    break;
                }
            }
            var targetSpeed = i * Math.Sign(diff);

            if (_momentum < targetSpeed )
            {
                _momentum += 1;
            }
            else if (_momentum > targetSpeed )
            {
                _momentum -= 1;
            }
            try
            {
                CurrentVerticalOffset += _momentum;
            }
            catch
            {

            }
        }

        public bool ReflectChangesInCurrentOffset
        {
            get { return (bool)this.GetValue(ReflectChangesInCurrentOffsetProperty); }
            set { this.SetValue(ReflectChangesInCurrentOffsetProperty, value); }
        }


        public double CurrentHorizontalOffset
        {
            get { return (double)this.GetValue(CurrentHorizontalOffsetProperty); }
            set { this.SetValue(CurrentHorizontalOffsetProperty, value); }
        }

        public double CurrentVerticalOffset
        {
            get { return (double)this.GetValue(CurrentVerticalOffsetProperty); }
            set { this.SetValue(CurrentVerticalOffsetProperty, value); }
        }

        public FrameworkElement FollowControl
        {
            get { return (FrameworkElement)this.GetValue(FollowControlProperty); }
            set { this.SetValue(FollowControlProperty, value); }
        }

        public object FollowData
        {
            get { return this.GetValue(FollowDataProperty); }
            set { this.SetValue(FollowDataProperty, value); }
        }

        protected override void OnScrollChanged(ScrollChangedEventArgs e)
        {
            base.OnScrollChanged(e);
            if (ReflectChangesInCurrentOffset)
            {
                _preventReentry = true;
                if (CurrentHorizontalOffset != e.HorizontalOffset)
                {
                    CurrentHorizontalOffset = e.HorizontalOffset;
                }
                if (CurrentVerticalOffset != e.VerticalOffset)
                {
                    CurrentVerticalOffset = e.VerticalOffset;
                }
                _preventReentry = false;
            }
        }

    }
}

