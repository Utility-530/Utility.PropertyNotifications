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
using Utility.Reactive;
using Utility.WPF.Abstract;

namespace Utility.WPF.Controls.Base
{
    public class DoubleContentControl : HeaderedContentControl, IOrientation
    {
        public static readonly DependencyProperty PositionProperty = DependencyHelper.Register<Dock>(new PropertyMetadata(Dock.Top));

        protected ContentPresenter? headerPresenter;
        protected ContentPresenter? contentPresenter;
        protected readonly ReplaySubject<DockPanelSplitter> DockPanelSplitterSubject = new(1);
        public static readonly DependencyProperty OrientationProperty = WrapPanel.OrientationProperty.AddOwner(typeof(DoubleContentControl));

        static DoubleContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DoubleContentControl), new FrameworkPropertyMetadata(typeof(DoubleContentControl)));
        }

        public DoubleContentControl()
        {
            Docks
                .CombineLatest(DockPanelSplitterSubject)
                .Subscribe(c =>
                {
                    var (position, splitter) = c;

                    if (Content == null && contentPresenter?.Content == null || Header == null && headerPresenter?.Content == null)
                    {
                        splitter.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        //DockPanel.SetDock(splitter, position);
                        splitter.Visibility = Visibility.Visible;
                    }

                    Change(position, splitter);
                });

            Orientations.CombineLatest(
                    headerPresenter
                .WhenAnyValue(a => a.Content)
                .Merge(this.WhenAnyValue(a => a.Header)))
                .Subscribe(a =>
                {
                    var (content, orientation) = a;
                    ChangeOrientation(orientation, content);
                });

            Orientations.CombineLatest(
                    contentPresenter
                        .WhenAnyValue(a => a.Content)
                        .Merge(this.WhenAnyValue(a => a.Content)))
                .Subscribe(a =>
                {
                    var (orientation, content) = a;
                    ChangeOrientation(content, orientation);
                });

            void Change(Dock position, DockPanelSplitter splitter)
            {
                DockPanel.SetDock(headerPresenter, position);
                DockPanel.SetDock(splitter, position);
                DockPanel.SetDock(contentPresenter, position);
            }
        }

        #region properties

        public Dock Position
        {
            get => (Dock)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        #endregion properties

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var dockPanel = GetTemplateChild("PART_DockPanel") as DockPanel ??
                            throw new Exception("sfsdd ff333..");
            //var children = dockPanel.Children.OfType<ContentPresenter>().ToArray();
            //contentPresenter = children.SingleOrDefault(a => a.ContentSource == "Content") ?? throw new Exception("sfsd cp1d ff333..");
            //headerPresenter = children.SingleOrDefault(a => a.ContentSource == "Header") ?? throw new Exception("sfsd cp1d ff333..");
            contentPresenter = GetTemplateChild("PART_ContentPresenter") as ContentPresenter ??
                               throw new Exception("sfsd cp1d ff333..");
            headerPresenter = GetTemplateChild("PART_HeaderPresenter") as ContentPresenter ??
                              throw new Exception("sfsdd fcp2f333..");
            Header ??= headerPresenter.Content;

            var dockPanelSplitter = GetTemplateChild("PART_DockPanelSplitter") as DockPanelSplitter ??
                                    throw new Exception($"{"PART_DockPanelSplitter"} fff333.. {GetTemplateChild("PART_DockPanelSplitter")?.GetType()}");

            if (dockPanel == null)
                throw new ApplicationException();

            DockPanelSplitterSubject.OnNext(dockPanelSplitter);
        }

        private static void ChangeOrientation(object content, Orientation orientation)
        {
            switch (content)
            {
                case IOrientation ori:
                    ori.Orientation = orientation switch
                    {
                        Orientation.Horizontal => Orientation.Vertical,
                        Orientation.Vertical => Orientation.Horizontal,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;

                case DependencyObject dp:
                    dp.SetValue(StackPanel.OrientationProperty, orientation switch
                    {
                        Orientation.Horizontal => Orientation.Vertical,
                        Orientation.Vertical => Orientation.Horizontal,
                        _ => throw new ArgumentOutOfRangeException()
                    });
                    //throw new Exception("Dsf4 444");
                    break;
            }
        }

        private IObservable<Dock> Docks =>
            Changes
                .Select(a =>
                {
                    var (dock, ori) = a;
                    if (dock.HasValue)
                        return dock.Value;
                    if (ori.HasValue)
                    {
                        switch (ori)
                        {
                            case Orientation.Horizontal:
                                return Dock.Left;

                            case Orientation.Vertical:
                                return Dock.Top;

                            default:
                                throw new ArgumentOutOfRangeException("sdfsd dssd 77");
                        }
                    }

                    throw new Exception("FSD£FFF");
                });

        private IObservable<Orientation> Orientations =>
            Changes
                .Select(a =>
                {
                    var (dock, ori) = a;
                    if (ori.HasValue)
                        return ori.Value;
                    if (dock.HasValue)
                    {
                        switch (dock.Value)
                        {
                            case Dock.Left:
                            case Dock.Right:
                                return Orientation.Horizontal;

                            case Dock.Top:
                            case Dock.Bottom:
                                return Orientation.Vertical;

                            default:
                                throw new ArgumentOutOfRangeException("sdfsd dssd 77");
                        }
                    }

                    throw new Exception("FSD£FFF");
                });

        private IObservable<(Dock?, Orientation?)> Changes
        {
            get
            {
                ReplaySubject<(Dock?, Orientation?)> replay = new(1);
                this.WhenAnyValue(a => a.Position)
                    .StartWith(Position)
                    .Combine(this.WhenAnyValue(a => a.Orientation).StartWith(Orientation))
                    .Subscribe(replay);
                return replay;
            }
        }
    }
}