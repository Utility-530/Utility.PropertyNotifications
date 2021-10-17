using System;
using UtilityStruct;
using UtilityStruct.Helper;
using Xunit;

namespace UtilityStruct.Test
{
    public class UnitTest1
    {

        [Fact]
        public void Test5()
        {
            var odd = ProbabilityEx.GetPerfectProbability(19.0 / 100d, 18.4 / 100d, 60.20 / 100d);
            Assert.Equal(100, (int)(odd.away + odd.draw + odd.home));
        }


        [Fact]
        public void Test1()
        {
            var odd = SpanHelper.Replace("uiro_sdf_ee", "_", "..");
            Assert.True(odd == "uiro..sdf..ee");
        }
    }
}
