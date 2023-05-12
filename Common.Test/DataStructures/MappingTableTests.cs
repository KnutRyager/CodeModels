using Common.DataStructures;
using Common.Files;
using Common.Typing;
using System.ComponentModel;
using Xunit;

namespace Common.Tests.DataStructures;

public class MappingTableTests
{
    public class TableSpec
    {
        [DisplayName("Name")]
        public string TheName { get; set; }
        [DisplayName("Value")]
        public int TheValue { get; set; }
    }

    [Fact]
    public void ClassMappingTable()
    {
        var table = new ClassMappingTable<int, TableSpec>(new LazyLoadList<TableSpec>(() => FileUtil.ReadFileToType<TableSpec>("TestData/SampleForMappingTableParse.txt", ",")), x => x.TheValue);
        Assert.Equal(0, table.IndexOf(10));
        Assert.Equal(1, table.IndexOf(20));
        Assert.Equal(2, table.IndexOf(30));
        Assert.Equal("A", table[10].TheName);
        Assert.Equal("B", table[20].TheName);
        Assert.Equal("C", table[30].TheName);
    }

    [Fact]
    public void LazyMappingTable()
    {
        var table = new MappingTable<int, string>(new LazyLoadList<(int, string)>(() => FileUtil.ReadFileToTuple<int, string>("TestData/SampleForMappingTableParse.txt", ",")));
        Assert.Equal("A", table[10].Item2);
        Assert.Equal("B", table[20].Item2);
        Assert.Equal("C", table[30].Item2);
    }

    public interface IMyInterface1 { }
    public interface IMyInterface2 { }
    [Fact(Skip = "Not working anytime soon")]
    public void MappingTableWithTypes()
    {
        var table = new MappingTable<D_int<IMyInterface1>, D_string<IMyInterface2>>(new LazyLoadList<(D_int<IMyInterface1>, D_string<IMyInterface2>)>(() => FileUtil.ReadFileToTuple<D_int<IMyInterface1>, D_string<IMyInterface2>>("SampleForMappingTableParse.txt", ",")));
        Assert.Equal("A", table[10].Item2);
        Assert.Equal("B", table[20].Item2);
        Assert.Equal("C", table[30].Item2);
    }
}
