using System;
using System.Reflection;

namespace Utility.Nodify.Demo
{
    public static class Sample
    {
        private static System.Type type = typeof(Sample);

        public static int _A()
        {
            return 0;
        }

        public static int _B()
        {
            return 2;
        }

        public static int _C()
        {
            return 2;
        }

        public static int Sum(int a, int b)
        {
            return a + b;
        }

        public static int Multiply(int a, int b)
        {
            return a * b;
        }

        public record ViewModel(int Sum, int Multiplication);

        public static (ParameterInfo, ParameterInfo)[] Connections => new[] {
            (ReturnParameter(nameof(_A)), Parameter(nameof(Sum), "a")),
            //(ReturnParameter(nameof(_B)), Parameter(nameof(Sum), "b")),
            //(ReturnParameter(nameof(Sum)), Parameter(nameof(Multiply), "a")),
            //(ReturnParameter(nameof(_C)), Parameter(nameof(Multiply), "b"))
        };

        static ParameterInfo Parameter(string methodName, string name)
        {
            if (name == null)
                return type.GetMethod(methodName).ReturnParameter;
            var parameters = type.GetMethod(methodName).GetParameters();

            foreach (var parameter in parameters)
            {
                if (parameter.Name == name)
                {
                    return parameter;
                }

            }
            throw new Exception("s39d sed333 dd");
        }

        static ParameterInfo ReturnParameter(string methodName)
        {
            return type.GetMethod(methodName).ReturnParameter;
        }
    }
}
