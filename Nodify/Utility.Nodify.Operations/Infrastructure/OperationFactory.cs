//using Nodify;
//using Utility.Nodify.Core;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Windows;
//using static System.Runtime.InteropServices.JavaScript.JSType;

//namespace Utility.Nodify.Operations
//{
//    public static class OperationFactory
//    {
//        public static List<OperationInfo> GetOperationsInfo(Type container)
//        {
//            List<OperationInfo> result = new List<OperationInfo>();

//            foreach (var method in container.GetMethods())
//            {
//                if (method.IsStatic)
//                {
//                    OperationInfo op = new OperationInfo
//                    {
//                        Title = method.Name
//                    };

//                    var attr = method.GetCustomAttribute<OperationAttribute>();
//                    var para = method.GetParameters();

//                    bool generateInputNames = true;

//                    op.Type = OperationType.Normal;

//                    if (para.Length == 2)
//                    {
//                        var delType = typeof(Func<double, double, double>);
//                        var del = (Func<double, double, double>)Delegate.CreateDelegate(delType, method);

//                        op.Operation = new BinaryOperation(del);
//                    }
//                    else if (para.Length == 1)
//                    {
//                        if (para[0].ParameterType.IsArray)
//                        {
//                            op.Type = OperationType.Expando;

//                            var delType = typeof(Func<double[], double>);
//                            var del = (Func<double[], double>)Delegate.CreateDelegate(delType, method);

//                            op.Operation = new ParamsOperation(del);
//                            op.MaxInput = int.MaxValue;
//                        }
//                        else
//                        {
//                            var delType = typeof(Func<double, double>);
//                            var del = (Func<double, double>)Delegate.CreateDelegate(delType, method);

//                            op.Operation = new UnaryOperation(del);
//                        }
//                    }
//                    else if (para.Length == 0)
//                    {
//                        var delType = typeof(Func<double>);
//                        var del = (Func<double>)Delegate.CreateDelegate(delType, method);

//                        op.Operation = new ValueOperation(del);
//                    }

//                    if (attr != null)
//                    {
//                        op.MinInput = attr.MinInput;
//                        op.MaxInput = attr.MaxInput;
//                        generateInputNames = attr.GenerateInputNames;
//                    }
//                    else
//                    {
//                        op.MinInput = (uint)para.Length;
//                        op.MaxInput = (uint)para.Length;
//                    }


//                    foreach (var param in para)
//                    {

//                        if (IsParams(param))
//                        {
//                            for (int i = 0; i < op.MinInput; i++)
//                            {
//                                op.Inputs.Add(new(i.ToString(), GetDefaultValue(param.ParameterType.GetElementType())));
//                            }
//                        }
//                        else
//                        {
//                            op.Inputs.Add(new(param.Name , param.HasDefaultValue? param.DefaultValue: GetDefaultValue(param.ParameterType)));
//                        }
//                    }

                  

//                    result.Add(op);
//                }
//            }

//            return result;
//        }

//        static bool IsParams(ParameterInfo param)
//        {
//            return param.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0;
//        }

//        static object GetDefaultValue(Type t)
//        {
//            if (t.IsValueType)
//                return Activator.CreateInstance(t);

//            return null;

//        }

//        public static NodeViewModel CreateNode(OperationInfo info)
//        {
//            var inputs = info.Inputs.Select(input => new ConnectorViewModel
//            {
//                Title = input.Name,
//                Value = input.DefaultValue
//            });

//            switch (info.Type)
//            {
//                //case OperationType.Expression:
//                //    return new ExpressionOperationViewModel
//                //    {
//                //        Title = info.Title,
//                //        Output = new ConnectorViewModel(),
//                //        Operation = info.Operation,
//                //        Expression = "1 + sin {a} + cos {b}"
//                //    };

//                //case OperationType.Calculator:
//                //    return new CalculatorOperationViewModel
//                //    {
//                //        Title = info.Title,
//                //        Operation = info.Operation,
//                //    };

//                //case OperationType.Expando:
//                //    var o = new ExpandoOperationViewModel
//                //    {
//                //        MaxInput = info.MaxInput,
//                //        MinInput = info.MinInput,
//                //        Title = info.Title,
//                //        Output = new ConnectorViewModel(),
//                //        Operation = info.Operation
//                //    };

//                //    o.Input.AddRange(input);
//                //    return o;

//                ////case OperationType.Group:
//                ////    return new OperationGroupViewModel
//                ////    {
//                ////        Title = info.Title,
//                ////    };

//                //case OperationType.Graph:
//                //    return new OperationGraphViewModel
//                //    {
//                //        Title = info.Title,
//                //        DesiredSize = new Size(420, 250)
//                //    };

//                default:
//                    {
//                        var op = new OperationNodeViewModel
//                        {
//                            Title = info.Title,
//                        };
//                        var connector = new ConnectorViewModel
//                        {
//                            Node = op
//                        };
//                        op.Output.AddRange(new[] { connector });
//                        op.Input.AddRange(inputs);
//                        //op.OnInputValueChanged();
//                        return op;
//                    }

//            }
//        }
//    }
//}
