using System;
using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Interface;

public class AttributeClassFromSourceTests
{
    [Fact] public void AttributeSingle() => @"
using System;

[Serializable]
class A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeMultiple() => @"
using System;

[Serializable]
[Obsolete]
class A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorEmpty() => @"
using System;

[Obsolete()]
class A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithParameter() => @"
using System;

[Obsolete(""test"")]
class A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithNamedParameter() => @"
using System;

[Obsolete(message: ""test"")]
class A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithMultipleParameters() => @"
using System;

[Obsolete(""test"", false)]
class A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeMultipleWithConstructorWithMultipleParameters() => @"
using System;

[Obsolete(message: ""test"")]
[AttributeUsage(validOn: AttributeTargets.All), Serializable]
class A
{
}".AssertParsedAndGeneratedEqual();
}