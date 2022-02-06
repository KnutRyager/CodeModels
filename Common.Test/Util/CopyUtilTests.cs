using Xunit;

namespace Common.Tests
{
    public class CopyUtilTests
    {
        [Fact] public void CopyInt() => Assert.Equal(4, 4.Copy());
        [Fact] public void CopyString() => Assert.Equal("test", "test".Copy());
        [Fact] public void CopyType() => Assert.Equal("Int32", typeof(int).Copy().Name);
        [Fact] public void CopyMethodInfo() => Assert.Equal("Parse", typeof(int).GetMethod(nameof(int.Parse), new[] { typeof(string) }).Copy().Name);
    }
}
