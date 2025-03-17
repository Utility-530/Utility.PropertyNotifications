using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utility.WPF.Helpers
{
    /// <summary>
    /// https://github.com/Rudnicky/VisualTreeHelper/blob/master/VisualTreeTraversionPoC/Utils/VisualTreeTraverseHelper.cs
    /// </summary>
    public static class VisualTreeExHelper
    {
        //https://stackoverflow.com/questions/974598/find-all-controls-in-wpf-window-by-type/978352#978352
        //answered Jun 10 '09 at 21:53
        //Bryce Kahle
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject? depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t)
                {
                    yield return t;
                }

                foreach (T childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }

        public static object? FindItemsPanel(Visual visual)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {

                if (VisualTreeHelper.GetChild(visual, i) is Visual child)
                {
                    if (child is { } temp && VisualTreeHelper.GetParent(child) is ItemsPresenter)
                    {
                        return temp;
                    }

                    if (FindItemsPanel(child) is { } panel)
                    {
                        return panel; // return the panel up the call stack
                    }
                }
            }

            return default;
        }

        /// <summary>
        /// Finds a Parent of given control
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns></returns>
        public static object? FindParent(this DependencyObject child, Type type) =>
            VisualTreeHelper.GetParent(child) switch
            {
                null => null,
                { } parent when parent.GetType() == type => parent,
                { } parent => parent.FindParent(type)
            };

        /// <summary>
        /// Finds a Parent of given control
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns></returns>
        public static T? FindParent<T>(this DependencyObject child) where T : DependencyObject =>
            VisualTreeHelper.GetParent(child) switch
            {
                null => null,
                T parent => parent,
                { } parent => FindParent<T>(parent)
            };

        /// <summary>
        /// Finds parent by given name
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DependencyObject? FindParent(this DependencyObject? dependencyObject, string name)
        {
            while (dependencyObject != null &&
                   VisualTreeHelper.GetParent(dependencyObject) is { } parentObj)
            {
                if ((string)parentObj.GetValue(FrameworkElement.NameProperty) == name)
                    return parentObj;

                dependencyObject = parentObj;
            }
            return null;
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static T? FindChild<T>(this DependencyObject? parent, string childName) where T : FrameworkElement
        {
            return FindChild<T>(parent, a => a.Name == childName);
        }



        public static T? FindChild<T>(this DependencyObject? parent, Predicate<T>? predicate = null) where T : DependencyObject
        {
            // Confirm parent and childName are valid.
            if (parent == null) 
                throw new ArgumentNullException(nameof(parent));

            foreach(var child in parent.ChildrenOfType<T>())
            {
                if (child is T t && predicate?.Invoke(t) != false)
                {
                    return t;
                }            
            }

            return null;

        }


        public static void ForEachChild<T>(this DependencyObject parent, Action<T> action) where T : DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                action(VisualTreeHelper.GetChild(parent, i) as T);

            }
        }

        /// <summary>
        /// Finds all controls of a specific type in visual tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static IEnumerable<T> ChildrenOfType<T>(this DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T childType)
                {
                    yield return childType;
                }

                foreach (var other in ChildrenOfType<T>(child))
                {
                    yield return other;
                }
            }
        }

        /// <summary>
        /// Finds all controls of a specific type in visual tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        [Obsolete($"Use {nameof(ChildrenOfType)}")]
        public static IEnumerable<T> FindChildren<T>(this DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T childType)
                {
                    yield return childType;
                }

                foreach (var other in FindChildren<T>(child))
                {
                    yield return other;
                }
            }
        }

        /// <summary>
        /// Finds all controls of a specific type in visual tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static IEnumerable<T> Children<T>(this DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T childType)
                {
                    yield return childType;
                }
            }
        }

        /// <summary>
        /// Finds all controls of a specific type in visual tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> Children(this DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                yield return child;
            }
        }

        /// <summary>
        /// Gets child of specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static T? ChildOfType<T>(this DependencyObject? depObj, bool includeSelf = false) where T : DependencyObject
        {
            if (depObj == null) return null;
            if (includeSelf && depObj is T t) return t;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = child as T ?? ChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        /// <summary>
        /// Gets child of specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static DependencyObject? ChildOfInterface<T>(this DependencyObject? depObj)
        {
            if (depObj == null) return default;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = child.GetType().GetInterfaces().Contains(typeof(T));
                if (result)
                    return child;
                else if (ChildOfInterface<T>(child) is { } obj)
                    return obj;
            }
            return default;
        }

        public static void DetachFromParent(this UIElement child, DependencyObject parent)
        {
            DetachChild(parent, child);
        }

        public static void DetachFromParent(this FrameworkElement child)
        {
            DetachChild(child.Parent, child);
        }
        public static void DetachChild(this DependencyObject parent, UIElement child)
        {
            switch (parent)
            {
                case Panel panel:
                    panel.Children.Remove(child);
                    break;
                case Decorator decorator when decorator.Child == child:
                    decorator.Child = null;
                    break;
                case ContentPresenter contentPresenter when contentPresenter.Content == child:
                    contentPresenter.Content = null;
                    break;
                case ContentControl contentControl when contentControl.Content == child:
                    contentControl.Content = null;
                    break;
            }
        }
    }
}