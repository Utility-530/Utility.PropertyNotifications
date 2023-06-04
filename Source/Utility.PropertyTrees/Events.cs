using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Models;
using Utility.Nodes;
using Utility.PropertyTrees.Infrastructure;

namespace Utility.PropertyTrees
{
    public class Events
    {
        public record ActivationRequest(Guid? Key, PropertyDescriptor Descriptor, object Data, PropertyType PropertyType) : Request;
        public record ActivationResponse(ValueNode PropertyNode) : Response(PropertyNode);
    }
}
