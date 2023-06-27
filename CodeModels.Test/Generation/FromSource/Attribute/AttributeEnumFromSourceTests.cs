using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Interface;

public class AttributeEnumFromSourceTests
{
    [Fact] public void AttributeSingle() => @"
[System.Serializable]
enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeMultiple() => @"
[System.Serializable]
[System.Obsolete]
enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorEmpty() => @"
[System.Obsolete()]
enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithParameter() => @"
[System.Obsolete(""test"")]
enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithNamedParameter() => @"
[System.Obsolete(message: ""test"")]
enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithMultipleParameters() => @"
[System.Obsolete(""test"", false)]
enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeMultipleWithConstructorWithMultipleParameters() => @"
[System.Obsolete(message: ""test"")]
[AttributeUsage(validOn: AttributeTargets.All), Serializable]
enum A
{
}".AssertParsedAndGeneratedEqual();
}