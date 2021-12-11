using static CodeAnalysisTests.TestUtil;
using Xunit;
using CodeAnalyzation.CodeGeneration;
using TheEverythingAPI.Modelling;
using System;

namespace CodeAnalysisTests;

public class CodeGenerationTests
{
    [Fact]
    public void POJO() => CodeEqual(
@"public class A
{
    public int P1 { get; set; }

    public decimal? P2 { get; set; }

    public System.DateTime P3 { get; set; }
}",
    CodeGeneration.GeneratePOJO(new Clazz()
    {
        Name = "A",
        Fields = new[] { new Field() { Name = "P1", Type = typeof(int) } ,
            new Field() { Name = "P2", Type = typeof(decimal?) } ,
            new Field() { Name = "P3", Type = typeof(DateTime) } }
    }));


    [Fact]
    public void POJOWithForeignKey() => CodeEqual(
@"public class A
{
    public int? FK { get; set; }

    public B B_Prop { get; set; }
}",
    CodeGeneration.GeneratePOJO(new Clazz()
    {
        Name = "A",
        Fields = new[] {
            new Field() { Name = "FK", Type = typeof(int?),  ReferenceField = new Field(){ Name= "OtherField", Clazz = new Clazz(){ Name = "B"} }},
            new Field() { Name = "B_Prop",  ReferenceClazz = new Clazz(){ Name= "B" } } }
    }));
}
