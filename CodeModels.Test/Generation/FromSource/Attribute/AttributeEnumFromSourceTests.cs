using System;
using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Interface;

public class AttributeEnumFromSourceTests
{
    [Fact] public void AttributeSingle() => @"
using System;

[Serializable]
enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeMultiple() => @"
using System;

[Serializable]
[Obsolete]
enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorEmpty() => @"
[Obsolete()]
enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithParameter() => @"
using System;

[Obsolete(""test"")]
enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithNamedParameter() => @"
using System;

[Obsolete(message: ""test"")]
enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AttributeConstructorWithMultipleParameters() => @"
using System;

[Obsolete(""test"", false)]
enum A
{
}".AssertParsedAndGeneratedEqual();
}