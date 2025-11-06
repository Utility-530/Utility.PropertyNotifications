using System;
using Utility.Changes;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Keys;
using Utility.Interfaces;
using Utility.Interfaces.NonGeneric;
using Utility.ServiceLocation;

namespace Utility.Nodes.Meta
{
    internal class ChangeTracker
    {

        Lazy<ITreeRepository> repository;
        INodeSource engine;

        public ChangeTracker(INodeSource nodeSource)
        {
            var repo = Globals.Resolver.Resolve<ITreeRepository>();
            repository = new(() => repo);
            engine = nodeSource;
        }


        public void Track(Change a)
        {
            if (a is Change { Type: Changes.Type.Add, Value: { } value })
            {
                if (value is INodeViewModel _node)
                    engine.Add(_node);
                else
                {
                    //node.Add(await node.ToTree(value));
                }
            }
            else if (a is Change { Type: Changes.Type.Remove, Value: { } _value })
            {
                if (_value is not INodeViewModel node)
                {
                    throw new Exception("  333 sdsdf");
                }
                engine.RemoveBy(c =>

                {
                    if (c is IKey key)
                    {
                        if (_value is IKey _key)
                        {
                            return key.Key().Equals(_key.Key());
                        }
                        else if (_value is IGetGuid guid)
                        {
                            return key.Key().Equals(new GuidKey(guid.Guid));
                        }
                    }
                    throw new Exception("44c dd");

                });

                repository.Value.Remove((GuidKey)node.Key());
            }
            else if (a is Change { Type: Changes.Type.Update, Value: INodeViewModel newValue, OldValue: INodeViewModel oldValue })
            {
                engine.RemoveBy(c =>
                {
                    if (c is IKey key)
                    {
                        if (oldValue is IKey _key)
                        {
                            return key.Key().Equals(oldValue.Key());
                        }
                        //else if (_value is IGetGuid guid)
                        //{
                        //    return key.Key.Equals(new GuidKey(guid.Guid));
                        //}
                    }
                    throw new Exception("44c dd");
                });
            }
        }
    }
}
