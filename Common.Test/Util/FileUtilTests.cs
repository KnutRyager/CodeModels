using Common.Files;
using Xunit;

namespace Common.Tests
{
    public class FileUtilTests
    {

        [Fact]
        public void Pathify()
        {
            Assert.Equal("", FileUtil.Pathify("", ""));
            Assert.Equal("", FileUtil.Pathify(null, ""));
            Assert.Equal("", FileUtil.Pathify("", null));
            Assert.Equal(@"root", FileUtil.Pathify("root", null));
            Assert.Equal("path", FileUtil.Pathify(null, "path"));
            Assert.Equal(@"root\name", FileUtil.Pathify("root", "name"));
            Assert.Equal(@"root\name", FileUtil.Pathify(@"root\", "name"));
            Assert.Equal("c:name", FileUtil.Pathify("root", "c:name"));
        }
    }
}
