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

    [Fact] public void ClassOverrideVirtualMethod() => @"
class A
{
    public virtual int P => 1;
}

class B : A
{
    public override int P => 1;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ClassOverrideAbstractMethod() => @"
abstract class A
{
    public abstract int P;
}

class B : A
{
    public override int P => 1;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ClassOverrideMethodWithNew() => @"
class A
{
    public int P;
}

class B : A
{
    public new int P => 1;
}".AssertParsedAndGeneratedEqual();
}