using Common.Util;
using Xunit;

namespace Common.Tests
{
    public class MathUtilTests
    {
        [Fact]
        public void Max()
        {
            Assert.Equal(4, MathUtil.Max<int>(4, 3));
            Assert.Equal(4, MathUtil.Max<int>(3, 4));
            Assert.Equal(4, MathUtil.Max<int>(4, null));
            Assert.Equal(4, MathUtil.Max<int>(null, 4));
            Assert.Null(MathUtil.Max((double?)null, null));
        }
    }
}
