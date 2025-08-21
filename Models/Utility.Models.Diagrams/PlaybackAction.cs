using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;

namespace Utility.Models.Diagrams
{
    public class PlaybackAction : Model, IAction, IGetReference
    {
        private Func<object> reference;
        private Action action;
        private Action undoaction;
        private readonly Action<bool> activeAction;
        private object arguments;

        public PlaybackAction(object reference, Action action, Action undoaction, Action<bool> activeAction = null, object? arguments = null)
        {
            this.reference = () => reference;
            this.action = action;
            this.undoaction = undoaction;
            this.activeAction = activeAction;
            this.arguments = arguments;
            Name = $"{action.Method.Name} on {reference.GetType().Name}";
        }

        public PlaybackAction(Func<object> reference, Action action, Action undoaction, Action<bool> activeAction = null, object? arguments = null)
        {
            this.reference = reference;
            this.action = action;
            this.undoaction = undoaction;
            this.activeAction = activeAction;
            this.arguments = arguments;
            Name = $"{action.Method.Name} on {reference.GetType().Name}";
        }

        public override void SetNode(INode node)
        {
            base.SetNode(node);
            node.WithChangesTo(a => (a as IGetIsSelected).IsSelected)
                .Subscribe(a =>
                {
                    activeAction?.Invoke(a);
                });
        }

        public object Reference => reference();

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