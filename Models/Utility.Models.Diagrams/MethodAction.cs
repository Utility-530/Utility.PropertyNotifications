using Utility.Interfaces.Exs;

namespace Utility.Models.Diagrams
{
    public class MethodAction : IAction
    {
        private Action action;
        private Action undoaction;

        public MethodAction(MethodNode methodNode, Action action, Action undoaction)
        {
            MethodNode = methodNode;
            this.action = action;
            this.undoaction = undoaction;
        }

        public MethodNode MethodNode { get; }

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
