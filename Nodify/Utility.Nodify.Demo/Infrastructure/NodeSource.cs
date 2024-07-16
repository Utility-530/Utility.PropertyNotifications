using Utility.Nodify.Core;
using System;
using Utility.Nodify.Operations.Infrastructure;
using System.Reactive.Linq;
using System.Reflection;
using NetFabric.Hyperlinq;
using System.Linq;
using System.Collections.Generic;
using Utility.Helpers;
using Newtonsoft.Json;
using Utility.Nodify.Base;

namespace Utility.Nodify.Demo
{
    public class NodeSource : INodeSource
    {
        Lazy<ICollection<Node>> items = new(() => ToNodes(typeof(Sample)).ToList());
        Random random = new();
        static Dictionary<Guid, Node> cache = new();

        public IEnumerable<MenuItem> Filter(bool? isInput, Type? type)
        {
            List<Node> nodes = new();

            if (type == null)
            {
                nodes.AddRange(items.Value);
            }
            else if (isInput.HasValue && isInput.Value)
            {
                var sType = type.AsString();

                var @default = items.Value.Where(a => a.Outputs.Any(a => a.Content == sType));
                nodes.AddRange(@default);

                var output = ToOutputNode(new ObjectInfo(Guid.NewGuid(), type, RandomHelper.NextWord(random)));
                nodes.Add(output);
            }
            else
            {
                var sType = type.AsString();

                var @default = items.Value.Where(a => a.Inputs.Any(a => a.Content == sType));
                nodes.AddRange(@default);

                var input = ToInputNode(new ObjectInfo(Guid.NewGuid(), type, RandomHelper.NextWord(random)));
                nodes.Add(input);
            }

            List<MenuItem> list = new();

            foreach (var node in nodes)
            {
                cache[node.Guid] = node;
                list.Add(new MenuItem(node.Name, node.Guid));
            }

            return list;
        }

        public Node Find(MenuItem guid)
        {
            return cache[guid.Guid];
        }

        static IEnumerable<Node> ToNodes(Type type)
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(a => a.IsSpecialName == false))
            {
                yield return ToNode(method);
            }
        }

        public static Node ToNode(MethodInfo methodInfo)
        {
            var nodeViewModel = new Node
            {
                Guid = Guid.NewGuid(),
                Name = methodInfo.Name,             //nodeViewModel.Action = new Action(() => { nodeViewModel.Output.Single().Value= methodInfo.Invoke(null, nodeViewModel.Input.Select(a=>a.Value).ToArray());  });
                Content = methodInfo.Serialise(),
                Inputs = methodInfo.GetParameters().Select(a => new Connector() { Key = KeySource.Key(methodInfo, a), Content = a.ParameterType.AsString() }).ToArray(),
                Outputs = new[] { new Connector() { Key = KeySource.Key(methodInfo, methodInfo.ReturnParameter), Content = methodInfo.ReturnType.AsString() } }
            };

            return nodeViewModel;
        }

        public static Node ToInputNode(ObjectInfo proto)
        {
            var nodeViewModel = new Node
            {
                Guid = Guid.NewGuid(),
                Name = proto.Name,             //nodeViewModel.Action = new Action(() => { nodeViewModel.Output.Single().Value= methodInfo.Invoke(null, nodeViewModel.Input.Select(a=>a.Value).ToArray());  });
                Content = JsonConvert.SerializeObject(proto),
                Inputs = new[] { new Connector() { Key = proto.Name, Content = proto.Type.AsString() } },
                Outputs = Array.Empty<Connector>(),
            };

            return nodeViewModel;
        }

        public static Node ToOutputNode(ObjectInfo proto)
        {
            var nodeViewModel = new Node
            {
                Guid = Guid.NewGuid(),
                Name = proto.Name,             //nodeViewModel.Action = new Action(() => { nodeViewModel.Output.Single().Value= methodInfo.Invoke(null, nodeViewModel.Input.Select(a=>a.Value).ToArray());  });
                Content = JsonConvert.SerializeObject(proto),
                Inputs = Array.Empty<Connector>(),
                Outputs = new[] { new Connector() { Key = proto.Name, Content = proto.Type.AsString() } }
            };

            return nodeViewModel;
        }


        //public static MenuItem ToMenuItem(MethodInfo methodInfo)
        //{
        //    var nodeViewModel = new MenuItem(methodInfo.Name, methodInfo.Serialise(), methodInfo.Parameters().Select(A => A.ParameterType).ToArray(), new Type[] { methodInfo.ReturnType });
        //    //{
        //    //    Key =            //nodeViewModel.Action = new Action(() => { nodeViewModel.Output.Single().Value= methodInfo.Invoke(null, nodeViewModel.Input.Select(a=>a.Value).ToArray());  });
        //    //    Content = methodInfo.Serialise(),
        //    //    //Inputs = methodInfo.GetParameters().Select(a => new Connector() { Key = methodInfo.Name + "." + a.Name, Content = a.ParameterType.AsString() }).ToArray(),
        //    //    //Outputs = new[] { new Connector() { Key = methodInfo.Name + "." + methodInfo.ReturnParameter.Name, Content = methodInfo.ReturnParameter.ParameterType.AsString() } }
        //    //};

        //    return nodeViewModel;
        //}


        //public static void Load(System.Type type, string name, Guid guid)
        //{
        //    // associate guid with new proto
        //    var proto = TreeRepository.Instance.CreateProto(guid, name, type).Result;
        //    var instance = Activator.CreateInstance(type);
        //    var propertyData = new RootDescriptor(instance) { Guid = guid };
        //    propertyData.VisitDescendants(a => { });
        //}
    }
}
