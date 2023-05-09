using System.IO;
using CodeModels.Models;
using CodeModels.ProgramModels;
using FluentAssertions;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;
using static CodeModels.Factory.ProgramModelFactory;

namespace CodeModels.Test.ProgramModels;

public class ProgramModuleTests
{
    [Fact]
    public void SaveFiles()
    {
        var model = ProgramModule.Create(
            new("TestProgramModule"),
            null,
            ProgramLibrary.Create(new("Library1"),
            null,
            new NamedValueCollection(new AbstractProperty[] {
                new(type: Type(typeof(string)),
                name: "Property1")
            }, "ClassA")));
        var folder = model.ToFolder();
        folder.Save("BasicProgramModelTest");

        var fileRead = File.ReadAllText("BasicProgramModelTest/TestProgramModule/file0.cs");
        fileRead.Should().Be(@"public class ClassA
{
    public string Property1 { get; set; }
}");
    }

}
