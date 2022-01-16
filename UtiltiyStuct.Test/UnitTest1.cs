using System.IO;
using UtilityStruct.Common;
using Xunit;

namespace UtilityStruct.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var odd = SpanHelper.Replace("uiro_sdf_ee", "_", "..");
            Assert.True(odd == "uiro..sdf..ee");
        }

        [Fact]
        public void Test2()
        {
            var rec = typeof(FileInfo).AsRecord();
            var type = rec.ToType();
            Assert.True(rec.Name == type.Name);
            Assert.True(rec.Namespace == type.Namespace);
            Assert.True(rec.Assembly == type.Assembly.FullName);
        }

        [Fact]
        public void Test3()
        {
            var odd = ProbabilityEx.GetPerfectProbability(19.0 / 100d, 18.4 / 100d, 60.20 / 100d);
            Assert.Equal(100, (int)(odd.away + odd.draw + odd.home));
        }
    }
}