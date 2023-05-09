using System.IO;
using CodeAnalyzation.Models;
using CodeAnalyzation.Models.ProgramModels;
using FluentAssertions;
using Xunit;
using static CodeAnalyzation.Models.CodeModelFactory;
using static CodeAnalyzation.Models.ProgramModelFactory;

namespace CodeAnalyzation.Generation.Test;

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
