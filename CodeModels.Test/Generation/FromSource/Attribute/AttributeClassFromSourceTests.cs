using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Interface;

public class AttributeClassFromSourceTests
{
    [Fact] public void AttributeSingle() => @"
[System.Serializable]
class A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeMultiple() => @"
[System.Serializable]
[System.Obsolete]
class A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorEmpty() => @"
[System.Obsolete()]
class A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithParameter() => @"
[System.Obsolete(""test"")]
class A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithNamedParameter() => @"
[System.Obsolete(message: ""test"")]
class A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithMultipleParameters() => @"
[System.Obsolete(""test"", false)]
class A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeMultipleWithConstructorWithMultipleParameters() => @"
[System.Obsolete(message: ""test"")]
[AttributeUsage(validOn: AttributeTargets.All), Serializable]
class A
{
}".AssertParsedAndGeneratedEqual();
}