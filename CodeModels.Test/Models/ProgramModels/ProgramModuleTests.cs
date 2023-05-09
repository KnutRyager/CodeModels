using System.IO;
using CodeModels.Models;
using CodeModels.Models.ProgramModels;
using FluentAssertions;
using Xunit;
using static CodeModels.Models.CodeModelFactory;
using static CodeModels.Models.ProgramModelFactory;

namespace CodeModels.Generation.Test;

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
            new PropertyCollection(new Property[] {
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
