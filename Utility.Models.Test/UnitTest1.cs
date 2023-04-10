using System;
using Xunit;

namespace UtilityModel.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var x = new Probability(19);

            var english = x.EnglishOdd;
        }

        [Fact]
        public void Test2()
        {
            var x = new DateTime(2000, 1, 2).AddMilliseconds(11111);
            var x2 = new DateTime(2000, 1, 3).AddMilliseconds(-11111);
            var y = (Day)x;

            Assert.True(((DateTime)y).Equals(x.Date));
            Assert.True(((DateTime)y).Equals(x2.Date));
        }


    }
}
