using Utility.Nodify.Demo;
using Utility.Nodify.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Nodify.Demo.Infrastructure
{
    public static class Methods
    {
        [Operation(MinInput = 2, MaxInput = 10, GenerateInputNames = false)]
        public static double Add(params double[] operands) => operands.Sum();

        [Operation(MinInput = 2, MaxInput = 10, GenerateInputNames = false)]
        public static double Multiply(params double[] operands) => operands.Aggregate((x, y) => x * y);

        public static double Divide(double a, double b) => a / b;

        public static double Subtract(double a, double b) => a - b;

        public static double Pow(double value, double exp) => (double)Math.Pow((double)value, (double)exp);

        [Operation(GenerateInputNames = false)]
        public static double Abs(double value) => Math.Abs(value);

        public static double PI(double pi = Math.PI) => pi;    
        
        public static double Cosine1() => (double)Math.Cos(1);


        public static double Value(double value) => value;
    }
}
