using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Record;

public class RecordInheritanceFromSourceTests
{
    [Fact] public void RecordInheritance() => @"
record A;
record B : A;".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordInheritanceGeneric() => @"
record A<T>;
record B : A<int>;".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordImplementInterface() => @"
interface A
{
}

record B : A;".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordImplementInterfaceGeneric() => @"
interface A<T>
{
}

record B : A<int>;".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordInheritanceAndImplementInterfaceGeneric() => @"
record A;
interface B
{
}

record C : A, B;".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordOverrideVirtualMethod() => @"
record A
{
    public virtual int P => 1;
}

record B : A
{
    public override int P => 1;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordOverrideAbstractMethod() => @"
abstract record A
{
    public abstract int P;
}

record B : A
{
    public override int P => 1;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordOverrideMethodWithNew() => @"
record A
{
    public int P;
}

record B : A
{
    public new int P => 1;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordOverrideRecordBaseMethod() => @"
record A
{
    public virtual int P => 1;
}

record B : A
{
    public override int P => base.P;
}".AssertParsedAndGeneratedEqual();
}
