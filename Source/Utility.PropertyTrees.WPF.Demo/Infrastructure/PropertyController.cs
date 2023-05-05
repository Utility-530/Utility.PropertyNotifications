using Utility.Enums;
using Utility.Infrastructure;
using Utility.Models;
using System;
using Utility.Infrastructure.Common;

namespace Utility.PropertyTrees.Infrastructure
{
    public class PropertyController : BaseObject
    {
        public static Guid Guid => Guid.Parse("e15c2e55-bf33-480c-a4e7-b4b7804d1735");

        public override Key Key => new(Guid, nameof(PropertyController), typeof(PropertyController));

        public PropertyController()
        {
        }

        public override void OnNext(object value)
        {
            if (value is PropertyOrder order)
                Process(order);
            else
                base.OnNext(value);

            void Process(PropertyOrder order)
            {
                order.Progress = 0;
                switch (order.Access)
                {
                    case Access.Set:
                        {
                            try
                            {
                                
                            }
                            catch (Exception ex)
                            {
                                order.Exception = ex;
                                throw;
                            }

                            break;
                        }
                }
                void Update(object newValue, PropertyOrder order)
                {
                    Broadcast(new PropertyChange(order.Key, newValue, order.Value));
                }
            }
        }

    }
}