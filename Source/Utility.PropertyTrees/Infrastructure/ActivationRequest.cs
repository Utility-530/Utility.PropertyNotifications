using System.ComponentModel;

namespace Utility.PropertyTrees.Infrastructure
{
    public record ActivationRequest(Guid Guid, PropertyDescriptor Descriptor, object Data, PropertyType PropertyType);
}


