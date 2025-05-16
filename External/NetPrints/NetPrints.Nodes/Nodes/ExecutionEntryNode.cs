using NetPrints.Core;
using NetPrints.Interfaces;
using System.Runtime.Serialization;

namespace NetPrints.Graph
{
    [DataContract]
    public abstract class ExecutionEntryNode : Node, IExecutionEntryNode
    {
        /// <summary>
        /// Output execution pin that initially executes when a method gets called.
        /// </summary>
        public INodeOutputExecPin InitialExecutionPin
        {
            get { return OutputExecPins[0]; }
        }

        public ExecutionEntryNode(IExecutionGraph graph)
            : base(graph)
        {

        }
    }
}
