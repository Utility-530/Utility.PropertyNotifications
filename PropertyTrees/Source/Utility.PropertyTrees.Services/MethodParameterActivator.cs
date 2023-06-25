using System.Collections;
using Utility.Infrastructure;
using Utility.Models;
using Utility.Helpers;
using Utility.Nodes.Abstractions;
using Utility.Nodes;

namespace Utility.PropertyTrees.Services
{
    public class MethodParameterActivator : BaseObject
    {

        List<INode> selectedNodes = new();

        INode? hoveredNode;

        public override Key Key => new(Guids.MethodParameterActivator, nameof(MethodParameterActivator), typeof(MethodParameterActivator));

        public MethodParametersResponse OnNext(MethodParametersRequest Request)
        {
            List<object> objects = new();

            if (Request.MethodInfo.Name == "Remove" || Request.MethodInfo.Name == "MoveUp" || Request.MethodInfo.Name == "MoveDown")
            {
                if (Request.Data is IEnumerable enumerable)
                {
                    var type = Request.Data.GetType().GenericTypeArguments().Single();

                    if (hoveredNode is PropertyBase { PropertyType: Type propertyType } valueNode)
                    {
                        if (propertyType.Equals(type))
                        {
                            objects.Add(valueNode.Data);
                        }
                    }
                }

                return new MethodParametersResponse(objects.ToArray());
            }

            foreach (var param in Request.MethodInfo.GetParameters())
            {
                var instance = Activator.CreateInstance(param.ParameterType);
                objects.Add(instance);
            }

            return new MethodParametersResponse(objects.ToArray());
        }

        public void OnNext(SelectionChange change)
        {
            selectedNodes.Add(change.Node);
        }

        public void OnNext(OnHoverChange change)
        {
            if (change.IsMouseOver)
                hoveredNode = change.Node;
            else
                hoveredNode = null;
        }
    }
}
