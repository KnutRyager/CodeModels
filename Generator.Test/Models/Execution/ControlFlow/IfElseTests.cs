using FluentAssertions;
using Xunit;

namespace CodeAnalyzation.Models.Execution.ControlFlow;

public class IfElseTests
{
    [Fact] public void IfTrue() => @"
var a = true;
if(a) {
    ""yup""
} else {
    ""nope""
};".Eval().Should().Be("yup");

    [Fact] public void IfElseIfTrue() => @"
var a = true; if(!a) {
    ""yup1""
} else if(a) {
    ""yup2""
} else {
    ""nope""
};".Eval().Should().Be("yup2");

    [Fact] public void IfFalse() => @"
var a = false;
if(a) {
    ""yup""
} else {
    ""nope"";
}".Eval().Should().Be("nope");

    [Fact] public void IfElseIfFalse() => @"
var a = false; if(a) {
    ""yup1""
} else if(a) {
    ""yup2""
} else {
    ""nope"";
}".Eval().Should().Be("nope");

    [Fact] public void IfIllegalBreakBreaks() => @"
var s = 0; if(true) {
    s += 1;
    break;
    s += 2;
}
s;".Eval().Should().Be(1);
}
