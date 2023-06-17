using Netly.Core;
using Netly;
using System;
using Byter;
using Newtonsoft.Json;
using Utility.Models.UDP;
using DryIoc;
using System.Threading;
using Utility.Nodify.Operations;
using Utility.Nodify.Core;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NetFabric.Hyperlinq;
using Utility.Nodify.Engine.ViewModels;
using Utility.Graph.Shapes;


namespace Utility.Simulation.WPF.UDP.Demo;

class Constants
{
    public const string IPAddress = "127.0.0.1";
    public const int Port = 8000;
}

public class ResetOperation : IOperation
{
    private readonly IContainer container;

    private IList<Message> past => container.Resolve<RangeObservableCollection<Message>>(Keys.Past);
    private IList<Message> current => container.Resolve<RangeObservableCollection<Message>>(Keys.Current);
    private IList<Message> future => container.Resolve<RangeObservableCollection<Message>>(Keys.Future);

    public ResetOperation(IContainer container)
    {
        this.container = container;
    }
    public IOValue[] Execute(params IOValue[] operands)
    {
        if (operands.Select(a => a.Value).OfType<Reset>().SingleOrDefault() is not null)
        {
            past.Clear();
            current.Clear();
            future.Clear();
        }
        return Array.Empty<IOValue>();
    }
}

public class ResetController : OperationNodeViewModel
{
    //public const string Input0 = nameof;
    //public const string Input1 = "1";
    //public const string Output0 = "0";

    public const string Reset = nameof(Reset);

    public ResetController()
    {
        Title = Reset;
        Location = new System.Windows.Point(600, 200);

        var input0 = new ConnectorViewModel() { Title = nameof(Input0) };

        Input.Add(input0);

    }

    public ConnectorViewModel Input0 => Input[0];
}


public class GraphOperation : IOperation, IGraphController, IContentViewModel
{
    private readonly IContainer container;
    private GraphModel model = new GraphModel();

    public const string Input0 = "0";
    public const string Input1 = "1";
    public const string Input2 = "2";
    public const string Output0 = "0";

    public GraphModel Model => model;

    public GraphOperation(IContainer container)
    {
        this.container = container;
    }

    public IOValue[] Execute(params IOValue[] operands)
    {
        if (operands.Select(a => a.Value).OfType<TypeGuidDto>().SingleOrDefault() is TypeGuidDto { Guid: var guid, TypeDto: var typeDto, GuidDto: var guidDto } typeGuidDto)
        {
            bool flag =false;
            foreach (var baseObject in model.Graph.Vertices)
            {
                if (baseObject.Guid == guidDto.Source)
                {
                    baseObject.Events.Add(new Utility.Infrastructure.InitialisedEvent(typeDto));
                    flag = true;
                }
                //if(baseObject.Name == bases.InName)
            }
            if(flag==false)
            {

            }
            return new IOValue[] { new IOValue(Output0, guid) };
        }
        else if (operands.Select(a => a.Value).OfType<KeyDto[]>().SingleOrDefault() is KeyDto[] keyDto)
        {
            foreach (var baseObject in keyDto)
            {
                model.Graph.AddVertex(new PocVertex(baseObject.Name, baseObject.Guid));
            }
        }
        else if (operands.Select(a => a.Value).OfType<Reset>().SingleOrDefault() is not null)
        {
            model.Graph.Clear();
        }
        else
        {

        }

        return Array.Empty<IOValue>();
    }
}


public class GraphController : OperationNodeViewModel
{
    //public const string Input0 = nameof;
    //public const string Input1 = "1";
    //public const string Output0 = "0";

    public const string Graph = nameof(Graph);

    public GraphController()
    {
        Title = Graph;
        Location = new System.Windows.Point(400, 400);

        var input0 = new ConnectorViewModel() { Title = GraphOperation.Input0 };
        var input1 = new ConnectorViewModel() { Title = GraphOperation.Input1 };
        var input2 = new ConnectorViewModel() { Title = GraphOperation.Input2 };
        //var input2 = new ConnectorViewModel() { Title = GraphController.Input2, };
        var output0 = new ConnectorViewModel() { Title = GraphOperation.Output0 };
        //var output1 = new ConnectorViewModel() { Title = OperationInterfaceNodeViewModel.Output1, };

        Input.Add(input0);
        Input.Add(input1);
        Input.Add(input2);
        Output.Add(output0);
    }

    public ConnectorViewModel Input0 => Input[0];
    public ConnectorViewModel Input1 => Input[1];
    public ConnectorViewModel Input2 => Input[2];
    public ConnectorViewModel Output0 => Output[0];
}


public class UDPOperation : IOperation
{
    private readonly IContainer container;
    private GraphModel model => container.Resolve<IGraphController>().Model;

    public UDPOperation(IContainer container)
    {
        this.container = container;
    }

    public IOValue[] Execute(params IOValue[] operands)
    {
        if (operands.OfType<TypeGuidDto>().SingleOrDefault() is TypeGuidDto { TypeDto: var typeDto, GuidDto: var guidDto } typeGuidDto)
        {
            //foreach (var baseObject in model.Graph.Vertices)
            //{
            //    //if (baseObject.Guid == typeGuidDto.TypeDto.)
            //    //{
            //    //    baseObject.Events.Add(new InitialisedEvent(bases2));
            //    //}
            //    //if(baseObject.Name == bases.InName)
            //}
            return new IOValue[] { new IOValue(nameof(UDPController.Output0), guidDto.Source) };
        }

        return Array.Empty<IOValue>();
    }
}

public record Reset();

public class UDPController : OperationNodeViewModel
{

    public const string UDP = nameof(UDP);

    private readonly IContainer container;

    //private Queue<Guid> guids = new();
    readonly UdpServer server = new();

    public UDPController(IContainer container)
    {
        Title = UDP;
        Location = new System.Windows.Point(100, 300);
        this.container = container;
        var host = new Host(Constants.IPAddress, Constants.Port);

        var input0 = new ConnectorViewModel() { Title = nameof(Input0) };
        var output0 = new ConnectorViewModel() { Title = nameof(Output0) };
        var output1 = new ConnectorViewModel() { Title = nameof(Output1) };
        var output2 = new ConnectorViewModel() { Title = nameof(Output2) };

        Input.Add(input0);
        Output.Add(output0);
        Output.Add(output1);
        Output.Add(output2);

        server.OnOpen(() =>
        {
            Console.WriteLine($"[OPEN]: {host}");
            server.ToData("hello client");
        });

        server.OnClose(() =>
        {
            Console.WriteLine($"[CLOSE]: {host}");
        });

        server.OnError((e) =>
        {
            Console.WriteLine($"[ERROR]: {e}");
        });

        server.OnEnter((client) =>
        {
            Console.WriteLine($"[ENTER]: {client.Host}");
        });

        server.OnExit((client) =>
        {
            Console.WriteLine($"[EXIT]: {client.Host}");
        });

        server.OnData((client, data) =>
        {
            Console.WriteLine($"[DATA] {client.Host}: {NE.GetString(data)}");
        });

        server.OnEvent((client, name, data) =>
        {
            container.Resolve<SynchronizationContext>().Post(a =>
            {
                Reader reader = new(data);
                if (name == nameof(GuidDto))
                {

                    var guid = Guid.Parse(reader.Read<string>());
                    var read2 = reader.Read<string>();
                    var bases = JsonConvert.DeserializeObject<GuidDto>(read2);
                    var read3 = reader.Read<string>();
                    var bases2 = JsonConvert.DeserializeObject<TypeDto>(read3);
                    Output0.Value = new TypeGuidDto(guid, bases, bases2);
                }
                else if (name == nameof(KeyDto))
                {
                    var nodes = reader.Read<string>();
                    var bases = JsonConvert.DeserializeObject<KeyDto[]>(nodes);
                    Output1.Value = bases;

                    //foreach (var baseObject in bases)
                    //{
                    //    model.Graph.AddVertex(new PocVertex(baseObject.Name, baseObject.Guid));
                    //}
                    //var edge = new PocEdge("test", model.Graph.Vertices.ElementAt(0), model.Graph.Vertices.ElementAt(1));
                    //model.Graph.AddEdge(edge);
                }
      
                else if (name == "ping")
                {
                    Output2.Value = new Reset();
                    //throw new Exception("dss3222 2");
                }
            }, default);
        });



        server.OnModify((socket) =>
        {
            //socket.NoDelay = true;
        });

        server.Open(host);
    }

    public override void OnInputValueChanged(ConnectorViewModel connectorViewModel)
    {
        if (connectorViewModel.Title == nameof(Input0))
        {
            if (connectorViewModel.Value is Guid value)
            {
                var writer = new Writer();
                writer.Write(value.ToString());
                var bytes = writer.GetBytes();
                server.ToEvent("event", bytes);
            }
        }
        base.OnInputValueChanged(connectorViewModel);
    }

    public ConnectorViewModel Input0 => Input[0];
    public ConnectorViewModel Output0 => Output[0];
    public ConnectorViewModel Output1 => Output[1];
    public ConnectorViewModel Output2 => Output[2];

}
