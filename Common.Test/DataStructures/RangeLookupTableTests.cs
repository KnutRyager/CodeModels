using Common.DataStructures;
using Xunit;

namespace Common.Tests.DataStructures;

public class RangeLookupTableTests
{
    [Fact]
    public void Floor()
    {
        var table = new RangeLookupTable<int, int, int>("test",
            new[] { 10, 20, 30 }, new[] { 40, 50, 60 }, new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 } },
            rowMode: LookupRangeTableMode.Floor,
            columnMode: LookupRangeTableMode.Floor
            );
        Assert.Equal(1, table[11, 41]);
        Assert.Equal(1, table[19, 49]);
        Assert.Equal(2, table[19, 50]);
        Assert.Equal(3, table[19, 60]);
        Assert.Equal(4, table[20, 49]);
        Assert.Equal(5, table[20, 59]);
        Assert.Equal(6, table[20, 69]);
        Assert.Equal(7, table[30, 49]);
        Assert.Equal(8, table[30, 59]);
        Assert.Equal(9, table[30, 69]);
    }

    [Fact]
    public void Ceiling()
    {
        var table = new RangeLookupTable<int, int, int>("test",
            new[] { 10, 20, 30 }, new[] { 40, 50, 60 }, new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 } },
            rowMode: LookupRangeTableMode.Ceiling,
            columnMode: LookupRangeTableMode.Ceiling
           );
        Assert.Equal(1, table[1, 1]);
        Assert.Equal(1, table[9, 39]);
        Assert.Equal(1, table[9, 40]);
        Assert.Equal(2, table[9, 41]);
        Assert.Equal(2, table[9, 50]);
        Assert.Equal(4, table[11, 39]);
        Assert.Equal(5, table[11, 49]);
        Assert.Equal(6, table[11, 59]);
        Assert.Equal(7, table[21, 39]);
        Assert.Equal(8, table[21, 49]);
        Assert.Equal(9, table[21, 59]);
    }

    [Fact]
    public void Exact()
    {
        var table = new RangeLookupTable<int, string, int>("test",
            new[] { 10, 20, 30 }, new[] { "aaa", "bbb", "ccc" }, new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 } },
            rowMode: LookupRangeTableMode.Exact,
            columnMode: LookupRangeTableMode.Exact
           );
        Assert.Equal(1, table[10, "aaa"]);
        Assert.Equal(9, table[30, "ccc"]);
        Assert.Throws<ArgumentException>(() => table[1, "ab"]);
        Assert.Throws<ArgumentException>(() => table[30, "cca"]);
    }

    [Fact]
    public void Table3D()
    {
        var table = new RangeLookupTable3D<int, string, int, int>("test",
            new[] { 10, 20, 30 }, new[] { "aaa", "bbb", "ccc" }, new[] { 111, 222, 333 }, new[] {
                    new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 } },
                    new[] { new[] { 11, 12, 13 }, new[] { 14, 15, 16 }, new[] { 17, 18, 19 } },
                    new[] { new[] { 21, 22, 23 }, new[] { 24, 25, 26 }, new[] { 27, 28, 29 } },
            },
            rowMode: LookupRangeTableMode.Exact,
            columnMode: LookupRangeTableMode.Exact
           );
        Assert.Equal(1, table[10, "aaa", 111]);
        Assert.Equal(2, table[10, "aaa", 222]);
        Assert.Equal(3, table[10, "aaa", 333]);
        Assert.Equal(27, table[30, "ccc", 111]);
        Assert.Equal(28, table[30, "ccc", 222]);
        Assert.Equal(29, table[30, "ccc", 333]);
        Assert.Throws<ArgumentException>(() => table[1, "ab", 111]);
        Assert.Throws<ArgumentException>(() => table[30, "cca", 111]);
        Assert.Throws<ArgumentException>(() => table[30, "ab", 1337]);
    }
}
