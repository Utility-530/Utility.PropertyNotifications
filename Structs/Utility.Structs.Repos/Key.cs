using System;

namespace Utility.Structs.Repos
{
    public readonly record struct Key(Guid Guid, Guid ParentGuid, object Instance, string Name, int? Index, DateTime? Removed);



}

