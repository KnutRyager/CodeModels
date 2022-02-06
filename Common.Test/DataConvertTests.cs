using Common.Util;
using Xunit;

namespace Common.Tests;

public class DataConvertTests
{

    [Fact]
    public void Stream2Text()
    {
        var stream = DataConvert.Text2Stream("Test");
        var str = DataConvert.Stream2Text(stream);
        Assert.Equal("Test", str);
    }
}
