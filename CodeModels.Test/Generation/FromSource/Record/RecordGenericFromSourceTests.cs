using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Record;

public class RecordGenericFromSourceTests
{
    [Fact] public void RecordGeneric() => @"
record A<T>;".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordGenericWithRecordConstraint() => @"
record A<T>
    where T : record;".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordGenericWithNewConstraint() => @"
record A<T>
    where T : new();".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordGenericWithIntConstraint() => @"
record A<T>
    where T : int;".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordGenericWithAnotherRecordConstraint() => @"
record A;
record B<T>
    where T : A;".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordGenericTwoParameters() => @"
record A<T1, T2>;".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordGenericWithProperty() => @"
record A<T>
{
    T B { get; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordGenericWithMethod() => @"
record A<T>
{
    T B();
}".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordWithGenericMethod() => @"
record A
{
    T B<T>();
}".AssertParsedAndGeneratedEqual();
}
