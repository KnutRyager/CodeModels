using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Class;

public class ClassInheritanceFromSourceTests
{
    [Fact] public void ClassInheritance() => @"
class A
{
}

class B : A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ClassInheritanceGeneric() => @"
class A<T>
{
}

class B : A<int>
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ClassImplementInterface() => @"
interface A
{
}

class B : A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ClassImplementInterfaceGeneric() => @"
interface A<T>
{
}

class B : A<int>
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ClassInheritanceAndImplementInterfaceGeneric() => @"
class A
{
}

interface B
{
}

class C : A, B
{
}".AssertParsedAndGeneratedEqual();
}