using System.ComponentModel;
using System.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Models.Diagrams;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.ServiceLocation;

namespace Utility.Nodify.Transitions.Demo.Infrastructure
{
    internal class Tracker
    {
        public void Track(IDiagramViewModel diagramViewModel)
        {
            diagramViewModel
                .Connections
                .AndAdditions<IConnectionViewModel>()
                .Subscribe(connection =>
                {
                    if (connection.Output is IValueConnectorViewModel { } valueConnector and IGetData { Data: PropertyInfo propertyInfo })
                    {
                        valueConnector
                        .WithChangesTo(b => (b as IGetValue).Value)
                        .Subscribe(value =>
                        {
                            Globals.Resolver
                            .Resolve<IPlaybackEngine>()
                            .OnNext(
                                new PlaybackAction(connection.Output.Node,
                                () => (connection.Input as ISetValue).Value = value,
                                () => { },
                                a => (connection as ISetIsActive).IsActive = a,
                                new Dictionary<string, object> {
                                            { "Value", value },
                                    //{ "Name", value.Name },
                                    //{ "PreviousValue", previousResult }
                                }
                                )
                                { Name = "connection" });
                        });
                    }
                });

            diagramViewModel
                .Nodes
                .AndAdditions<INodeViewModel>()
                .Subscribe(nodeViewModel =>
                {
                    if (nodeViewModel is IGetData { Data : MethodInfo methodInfo })
                    {
                        Dictionary<string, object> previousValues = methodInfo.GetParameters().ToDictionary(a => a.Name, a => (object)null);
                        nodeViewModel.Inputs.AndAdditions<IConnectorViewModel>().Subscribe(input =>
                        {
                            if (input is IGetData { Data: ParameterInfo parameterInfo })
                            {
                                input.WithChangesTo(a => (a as IGetValue).Value).Subscribe(async value =>
                                {
                                    previousValues[parameterInfo.Name] = value;
                                    if (previousValues.All(previousValues => previousValues.Value != null))
                                    {
                                        try
                                        {
                                            var result = await Task.Run(() => methodInfo.Invoke(null, methodInfo.GetParameters().Select(a => previousValues[a.Name]).ToArray()));
                                            if (nodeViewModel.Outputs.FirstOrDefault() is ISetValue setValue)
                                            {
                                                setValue.Value = result;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Globals.Exceptions.OnNext(ex);
                                        }
                                    }
                                });
                            }
                        });
                    }
                    else if (nodeViewModel is IGetData { Data: PropertyDescriptor propertyDescriptor })
                    {
                        nodeViewModel.Inputs.AndAdditions<IConnectorViewModel>().Subscribe(input =>
                        {
                            if (input is IValueConnectorViewModel { } valueConnector and IGetData { Data: PropertyInfo propertyInfo })
                            {
                                valueConnector.WithChangesTo(a => (a as IGetValue).Value).Subscribe(value =>
                                {
                                    propertyInfo.SetValue(nodeViewModel, value);
                                });
                            }
                        });

                        nodeViewModel.Outputs.AndAdditions<IConnectorViewModel>().Subscribe(output =>
                        {
                            if (output is IValueConnectorViewModel { } connector and IGetData { Data: PropertyInfo propertyInfo })
                            {
                                nodeViewModel.WithChangesTo(propertyInfo).Subscribe(value =>
                                {
                                    (connector as ISetValue).Value = value;
                                });
                            }
                        });
                    }
                });
        }
    }
}
