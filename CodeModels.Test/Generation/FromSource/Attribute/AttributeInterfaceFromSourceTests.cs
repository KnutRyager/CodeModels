using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Interface;

public interface AttributeInterfaceFromSourceTests
{
    [Fact] public void AttributeSingle() => @"
[System.Serializable]
interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeMultiple() => @"
[System.Serializable]
[System.Obsolete]
interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorEmpty() => @"
[System.Obsolete()]
interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithParameter() => @"
[System.Obsolete(""test"")]
interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithNamedParameter() => @"
[System.Obsolete(message: ""test"")]
interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithMultipleParameters() => @"
[System.Obsolete(""test"", false)]
interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeMultipleWithConstructorWithMultipleParameters() => @"
[System.Obsolete(message: ""test"")]
[AttributeUsage(validOn: AttributeTargets.All), Serializable]
interface A
{
}".AssertParsedAndGeneratedEqual();
}