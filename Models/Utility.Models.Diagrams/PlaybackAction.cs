using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes;
using Utility.PropertyNotifications;

namespace Utility.Models.Diagrams
{
    public class PlaybackAction : NodeViewModel, IAction, IGetReference
    {
        private object reference;
        private Action action;
        private Action undoaction;
        private object arguments;

        public PlaybackAction(object reference, Action action, Action undoaction, Action<bool> activeAction = null, object? arguments = null)
        {
            this.reference = reference;
            this.action = action;
            this.undoaction = undoaction;
            this.arguments = arguments;
            Name = $"{action.Method.Name} on {reference.GetType().Name}";

            this.WithChangesTo(a => (a as IGetIsSelected).IsSelected)
                .Subscribe(a =>
                {
                    activeAction?.Invoke(a);
                });
        }

        public object Reference => reference;

        public object? Arguments => arguments;

        public void Do()
        {
            action();
        }

        public void Undo()
        {
            undoaction();
        }
    }
}