using Leepfrog.WpfFramework.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;


namespace Leepfrog.WpfFramework.Behaviors
{
    public class RankBehavior
    {


        #region GoCommand
        // ********************************************************************
        public static ICommand GoCommand => _goCommand;
        private static readonly ICommand _goCommand = new RelayCommand((param) => go(param as ItemsControl));

        // ********************************************************************
        #endregion




        #region IsTranslateTransform
        // ********************************************************************
        public static bool GetIsTranslateTransform(DependencyObject obj) => (bool)obj.GetValue(IsTranslateTransformProperty);
        public static void SetIsTranslateTransform(DependencyObject obj, bool value)
        {
            obj.SetValue(IsTranslateTransformProperty, value);
        }
        public static readonly DependencyProperty IsTranslateTransformProperty =
            DependencyProperty.RegisterAttached("IsTranslateTransform", typeof(bool), typeof(RankBehavior), new PropertyMetadata(default(bool)));
        // ********************************************************************
        #endregion
        #region IsScaleTransform
        // ********************************************************************
        public static bool GetIsScaleTransform(DependencyObject obj) => (bool)obj.GetValue(IsScaleTransformProperty);
        public static void SetIsScaleTransform(DependencyObject obj, bool value)
        {
            obj.SetValue(IsScaleTransformProperty, value);
        }
        public static readonly DependencyProperty IsScaleTransformProperty =
            DependencyProperty.RegisterAttached("IsScaleTransform", typeof(bool), typeof(RankBehavior), new PropertyMetadata(default(bool)));
        // ********************************************************************
        #endregion

        // add attached property called Sequence
        // update it when ranks change!

        // Using a DependencyProperty as the backing store for Rank.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SequenceProperty =
            DependencyProperty.RegisterAttached("Sequence", typeof(int?), typeof(RankBehavior), new PropertyMetadata(0));

        public static int? GetSequence(DependencyObject obj)
        {
            return (int?)obj.GetValue(SequenceProperty);
        }

        public static void SetSequence(DependencyObject obj, int? value)
        {
            obj.SetValue(SequenceProperty, value);
        }



        // Using a DependencyProperty as the backing store for Rank.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RankProperty =
            DependencyProperty.RegisterAttached("Rank", typeof(int?), typeof(RankBehavior), new PropertyMetadata(-1, rank_PropertyChanged));

        public static int? GetRank(DependencyObject obj)
        {
            return (int?)obj.GetValue(RankProperty);
        }

        public static void SetRank(DependencyObject obj, int? value)
        {
            obj.SetValue(RankProperty, value);
        }

        private static void rank_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //-----------------------------------------------------------------
            //e.AddLog("RANK", $"UPDATING {e.OldValue} TO {e.NewValue}");
            var frameworkElement = d as FrameworkElement;
            var li = FindAncestor<ContentPresenter>(frameworkElement);
            var itemsControl = FindAncestor<ItemsControl>(li);
            if ( itemsControl == null )
            {
                //-----------------------------------------------------------------
                // NOTHING WE CAN DO HERE!
                return;
                //-----------------------------------------------------------------
            }
            RankGroup rankGroup = null;
            //-----------------------------------------------------------------
            //e.AddLog("RANK", $"UPDATING {e.OldValue} TO {e.NewValue} - ABOUT TO LOCK");
            lock ( _collectionLock )
            {
                //-----------------------------------------------------------------
                // MAKE SURE THIS ELEMENT IS REGISTERED WITH THE LIST
                if ( !_rankGroups.ContainsKey(itemsControl) )
                {
                    //-----------------------------------------------------------------
                    // CREATE NEW LIST OF ELEMENTS FOR THIS LIST!
                    rankGroup = new RankGroup();
                    rankGroup.ItemsControl = itemsControl;
                    _rankGroups.Add(itemsControl, rankGroup);
                    //-----------------------------------------------------------------
                }
                else
                {
                    //-----------------------------------------------------------------
                    // GET EXISTING LIST OF ELEMENTS
                    rankGroup = _rankGroups[itemsControl];
                    //-----------------------------------------------------------------
                }
                //-----------------------------------------------------------------
                // IF THIS ELEMENT DOESN'T ALREADY EXIST IN THE LIST...
                if ( !rankGroup.Elements.Contains(frameworkElement) )
                {
                    // ADD IT
                    rankGroup.Elements.Add(frameworkElement);
                    // AND HOOK UP THE UNLOAD EVENT
                    frameworkElement.Unloaded += frameworkElement_Unloaded;
                    rankGroup.ElementsUpdatedCount = 0;
                    //itemsControl.Dispatcher.BeginInvoke((Action)(() => processUpdate(rankGroup, true)));
                    var item = new Tuple<RankGroup, bool>(rankGroup, true);
                    if ( !_updateQueue.Contains(item) )
                    {
                        _updateQueue.Add(item);
                    }
                    itemsControl.Dispatcher.BeginInvoke((Action)( () => processUpdate() ));
                }
                else
                {
                    // IT DID EXIST IN THE LIST, SO START COUNTING
                    // WHEN ALL PROPERTIES HAVE CHANGED, WE CAN START THE UPDATE
                    rankGroup.ElementsUpdatedCount++;
                    if ( rankGroup.ElementsUpdatedCount >= rankGroup.Elements.Count )
                    {
                        rankGroup.ElementsUpdatedCount = 0;
                        //itemsControl.Dispatcher.BeginInvoke((Action)(() => processUpdate(rankGroup, false)));
                    }
                }
                //-----------------------------------------------------------------
            }
            //-----------------------------------------------------------------
            //e.AddLog("RANK", $"UPDATING {e.OldValue} TO {e.NewValue} - FINISHED");
            //-----------------------------------------------------------------
        }

        static void frameworkElement_Unloaded(object sender, RoutedEventArgs e)
        {
            var rankGroups = _rankGroups.Where(rg => rg.Value.Elements.Contains(sender));
            if ( !rankGroups.Any() )
            {
                return;
            }
            var rankGroup = rankGroups.First();
            rankGroup.Value.Elements.Remove(sender as FrameworkElement);
            if ( !rankGroup.Value.Elements.Any() )
            {
                _rankGroups.Remove(rankGroup.Key);
            }
        }

        static void processUpdate()
        {
            //-----------------------------------------------------------------
            while ( _updateQueue.Any() )
            {
                //-----------------------------------------------------------------
                var item = _updateQueue[0];
                _updateQueue.RemoveAt(0);
                var rankGroup = item.Item1;
                var isFirstRun = item.Item2;
                //-----------------------------------------------------------------
                rankGroup.AddLog("RANK", "UPDATING");
                ItemsControl itemsControl = rankGroup.ItemsControl;
                List<FrameworkElement> elements = rankGroup.Elements;
                //-----------------------------------------------------------------
                var newRanks = new Dictionary<FrameworkElement, int?>();
                foreach ( var frameworkElement in elements )
                {
                    var bindingExpression = frameworkElement.GetBindingExpression(RankBehavior.RankProperty);
                    bindingExpression?.UpdateTarget();
                    var newRank = (int?)frameworkElement.GetValue(RankBehavior.RankProperty);
                    newRanks.Add(frameworkElement, newRank);
                }
                //-----------------------------------------------------------------            
                var orderedRanks = newRanks.OrderBy(r => r.Value ?? -1).Select(r => r.Key).ToArray();
                //-----------------------------------------------------------------
                foreach ( var frameworkElement in elements )
                {
                    //-----------------------------------------------------------------
                    var li = FindAncestor<ContentPresenter>(frameworkElement);
                    //-----------------------------------------------------------------
                    TranslateTransform trans = null;
                    ScaleTransform scales = null;
                    //-----------------------------------------------------------------
                    // does translate transform already exist?
                    if (!_translates.ContainsKey(frameworkElement))
                    {
                        // no, so lets create one
                        var transPrev = frameworkElement.RenderTransform;
                        var transGroupPrev = transPrev as TransformGroup;
                        if (transGroupPrev != null)
                        {
                            foreach (var tran in transGroupPrev.Children)
                            {
                                if ((bool)tran.GetValue(RankBehavior.IsTranslateTransformProperty))
                                {
                                    trans = tran as TranslateTransform;
                                    break;
                                }
                            }
                        }
                        if (trans == null)
                        {
                            trans = new TranslateTransform();
                            var transGroup = new TransformGroup();
                            if (transPrev != null)
                            {
                                transGroup.Children.Add(frameworkElement.RenderTransform);
                            }
                            transGroup.Children.Add(trans);
                            frameworkElement.RenderTransform = transGroup;
                        }
                        _translates.Add(frameworkElement, trans);
                    }
                    else
                    {
                        trans = _translates[frameworkElement];
                    }
                    //-----------------------------------------------------------------
                    // does scale scalesform already exist?
                    if (!_scales.ContainsKey(frameworkElement))
                    {
                        // no, so lets create one
                        var scalesPrev = frameworkElement.RenderTransform;
                        var scalesGroupPrev = scalesPrev as TransformGroup;
                        if (scalesGroupPrev != null)
                        {
                            foreach (var scale in scalesGroupPrev.Children)
                            {
                                if ((bool)scale.GetValue(RankBehavior.IsScaleTransformProperty))
                                {
                                    scales = scale as ScaleTransform;
                                    break;
                                }
                            }
                        }
                        if (scales == null)
                        {
                            scales = new ScaleTransform();
                            var scalesGroup = new TransformGroup();
                            if (scalesPrev != null)
                            {
                                scalesGroup.Children.Add(frameworkElement.RenderTransform);
                            }
                            scalesGroup.Children.Add(scales);
                            frameworkElement.RenderTransform = scalesGroup;
                        }
                        _scales.Add(frameworkElement, scales);
                    }
                    else
                    {
                        scales = _scales[frameworkElement];
                    }
                    //-----------------------------------------------------------------
                    var newRank = Array.IndexOf(orderedRanks, frameworkElement) + 1;
                    frameworkElement.SetValue(SequenceProperty, newRank);
                    double newOffset = 0.0;
                    {
                        newOffset = elements.Take(newRank - 1).Sum(uiE => FindAncestor<ContentPresenter>(uiE).ActualHeight);
                        newOffset -= li.TranslatePoint(new Point(0, 0), itemsControl).Y;
                        li.SetValue(Canvas.ZIndexProperty, 999 - newRank);
                    }
                    //-----------------------------------------------------------------
                    if ( isFirstRun )
                    {
                        //-----------------------------------------------------------------
                        trans.SetValue(TranslateTransform.YProperty, newOffset);
                        //-----------------------------------------------------------------
                    }
                    else
                    {
                        //-----------------------------------------------------------------
                        var ani = new DoubleAnimationUsingKeyFrames();
                        //-----------------------------------------------------------------
                        /*
                        if ( trans.Y < newOffset )
                        {
                            ani.KeyFrames.Add(new EasingDoubleKeyFrame(newOffset, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new BounceEase() { Bounces = 3, Bounciness = ( newOffset - (double)trans.GetValue(TranslateTransform.YProperty) ) / 20, EasingMode = EasingMode.EaseOut }));
                        }
                        else
                        {
                            ani.KeyFrames.Add(new EasingDoubleKeyFrame(newOffset - 30, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.6)), new PowerEase() { Power = 3, EasingMode = EasingMode.EaseOut }));
                            ani.KeyFrames.Add(new EasingDoubleKeyFrame(newOffset, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new PowerEase() { Power = 3, EasingMode = EasingMode.EaseInOut }));
                        }
                        */
                        ani.KeyFrames.Add(new EasingDoubleKeyFrame(newOffset, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new CubicEase() { EasingMode = EasingMode.EaseInOut }));
                        //-----------------------------------------------------------------
                        trans.BeginAnimation(TranslateTransform.YProperty, ani);
                        //-----------------------------------------------------------------
                        ani = new DoubleAnimationUsingKeyFrames();
                        if (trans.Y < newOffset)
                        {
                            ani.KeyFrames.Add(new EasingDoubleKeyFrame(1.1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5)), new CubicEase() { EasingMode = EasingMode.EaseInOut }));
                        }
                        else
                        {
                            ani.KeyFrames.Add(new EasingDoubleKeyFrame(0.9, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5)), new CubicEase() { EasingMode = EasingMode.EaseInOut }));
                        }
                        //-----------------------------------------------------------------
                        ani.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new CubicEase() { EasingMode = EasingMode.EaseInOut }));
                        scales.BeginAnimation(ScaleTransform.ScaleXProperty, ani);
                        //-----------------------------------------------------------------
                    }
                    //-----------------------------------------------------------------
                }
                //-----------------------------------------------------------------
            }
            //-----------------------------------------------------------------
        }

        private class RankGroup
        {
            public ItemsControl ItemsControl = null;
            public List<FrameworkElement> Elements = new List<FrameworkElement>();
            public int ElementsUpdatedCount = 0;
        }

        private static Dictionary<DependencyObject, TranslateTransform> _translates = new Dictionary<DependencyObject, TranslateTransform>();
        private static Dictionary<DependencyObject, ScaleTransform> _scales = new Dictionary<DependencyObject, ScaleTransform>();
        private static Dictionary<ItemsControl, RankGroup> _rankGroups = new Dictionary<ItemsControl, RankGroup>();
        private static object _collectionLock = new object();
        private static List<Tuple<RankGroup, bool>> _updateQueue = new List<Tuple<RankGroup, bool>>();

        private static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            return FindAncestor<T>(current, null);
        }
        private static T FindAncestor<T>(DependencyObject current, Func<DependencyObject, bool> filter)
            where T : DependencyObject
        {
            do
            {
                if (
                    ( current is T ) &&
                    (
                     ( filter == null )
                  || ( filter(current) )
                    )
                   )
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while ( current != null );
            return null;
        }



        private static void go(ItemsControl itemsControl)
        {
            //-----------------------------------------------------------------
            lock ( _collectionLock )
            {

                if ( !_rankGroups.ContainsKey(itemsControl) )
                {
                    itemsControl.AddLog("RANK", "NO ITEMSCONTROL");
                    return;
                }
                var rankGroup = _rankGroups[itemsControl];
                //itemsControl.Dispatcher.BeginInvoke((Action)(() => processUpdate(rankGroup, false)));
                var item = new Tuple<RankGroup, bool>(rankGroup, false);
                if ( !_updateQueue.Contains(item) )
                {
                    _updateQueue.Add(item);
                }
                itemsControl.Dispatcher.BeginInvoke((Action)( () => processUpdate() ));
            }
            //-----------------------------------------------------------------
        }



    }
}
