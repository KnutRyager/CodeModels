using CodeModels.Execution;
using FluentAssertions;
using Xunit;

namespace CodeModels.Test.Execution.Expression;

public class LiteralEvalTests
{
    [Fact] public void EmptyExpressionIsNull() => "".Eval().Should().Be(null);
    [Fact] public void EmptyStringExpressionIsEmptyString() => "\"\"".Eval().Should().Be("");
    [Fact] public void String() => "\"Hello, world!\"".Eval().Should().Be("Hello, world!");
    [Fact] public void True() => "true".Eval().Should().Be(true);
    [Fact] public void False() => "false".Eval().Should().Be(false);
    [Fact] public void Byte() => "0".Eval().Should().Be((byte)0);
    [Fact] public void Int() => "-1337".Eval().Should().Be(-1337);
    [Fact] public void IntPlainNegative() => "-2147483648".Eval().Should().Be(int.MinValue);
    [Fact] public void UInt() => "4294967295u".Eval().Should().Be(uint.MaxValue);
    [Fact] public void UIntPlain() => "4294967295".Eval().Should().Be(uint.MaxValue);
    [Fact] public void Long() => "9223372036854775807L".Eval().Should().Be(long.MaxValue);
    [Fact] public void LongPlainPositive() => "9223372036854775807".Eval().Should().Be(long.MaxValue);
    [Fact] public void LongPlainNegative() => "-9223372036854775807".Eval().Should().Be(long.MinValue + 1);
    [Fact] public void LongPlainNegativeMinimum() => "-9223372036854775808".Eval().Should().Be(long.MinValue);
    [Fact] public void ULong() => "18446744073709551615uL".Eval().Should().Be(ulong.MaxValue);
    [Fact] public void ULongPlain() => "18446744073709551615".Eval().Should().Be(ulong.MaxValue);
    [Fact] public void ULongHex() => "0xFFFFUL".Eval().Should().Be(0xFFFFUL);
    [Fact] public void Float() => "3.40282347E+38f".Eval().Should().Be(float.MaxValue);
    [Fact] public void Double() => "1.7976931348623157E+308d".Eval().Should().Be(double.MaxValue);
    [Fact] public void DecimalExact() => "0.5m".Eval().Should().Be(0.5m);
    [Fact] public void DecimalBig() => "18446744073709551616m".Eval().Should().Be(18446744073709551616m);
}
