using System;
using Utility.Enums;

namespace Utility.Attributes
{
    public class ParamAttribute(CLREvent @event = CLREvent.None, int ratePerMinute = 60, string? customEventName = default) : Attribute
    {
        public CLREvent Event { get; } = @event;
        public int RatePerMinute { get; } = ratePerMinute;
        public string? CustomEventName { get; } = customEventName;
    }
}