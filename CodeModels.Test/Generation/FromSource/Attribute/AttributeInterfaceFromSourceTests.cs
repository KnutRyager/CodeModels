using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Interface;

public interface AttributeInterfaceFromSourceTests
{
    [Fact] public void AttributeSingle() => @"
using System;

[Serializable]
interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeMultiple() => @"
using System;

[Serializable]
[Obsolete]
interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorEmpty() => @"
using System;

[Obsolete()]
interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithParameter() => @"
using System;

[Obsolete(""test"")]
interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithNamedParameter() => @"
using System;

[Obsolete(message: ""test"")]
interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithMultipleParameters() => @"
using System;

[Obsolete(""test"", false)]
interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeMultipleWithConstructorWithMultipleParameters() => @"
using System;

[Obsolete(message: ""test"")]
[AttributeUsage(validOn: AttributeTargets.All), Serializable]
interface A
{
}".AssertParsedAndGeneratedEqual();
}