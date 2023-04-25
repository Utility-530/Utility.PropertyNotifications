using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Utility.Commands;
using Utility.Enums;
using Utility.Infrastructure.Abstractions;
using Utility.PropertyTrees.Abstractions;
using Autofac;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyTrees.WPF.Demo
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window, IObserver
    {
        private IHistory history;

        public HistoryWindow(IContainer container)
        {
            InitializeComponent();

            history = container.Resolve<IHistory>(); 
            var playback = container.Resolve<IPlayback>();

            HistoryPanel.DataContext = history;

            StepButtons.Enabled = Step.Walk | Step.Backward | Step.Forward;

            StepButtons.Command = new Command<Step>(step =>
            {
                switch (step)
                {
                    case Step.None:
                        break;

                    case Step.Backward:
                        playback.Back();
                        break;

                    case Step.Forward:
                        playback.Forward();
                        break;

                    case Step.Walk:
                        playback.Play();
                        StepButtons.Enabled = Step.Wait;
                        break;

                    case Step.Wait:
                        playback.Pause();
                        OnNext(default);
                        break;

                    case Step.All:
                        break;
                }
            });    
        }

        public bool Equals(IEquatable? other)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(object value)
        {
            StepButtons.Enabled = Steps().Aggregate((x, y) => x |= y);

            IEnumerable<Step> Steps()
            {
                if (history.Future.GetEnumerator().MoveNext())
                {
                    yield return Step.Walk;
                    yield return Step.Forward;
                }
                if (history.Past.GetEnumerator().MoveNext())
                    yield return Step.Backward;
            }
        }
    }
}