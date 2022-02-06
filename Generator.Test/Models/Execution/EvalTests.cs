using FluentAssertions;
using Xunit;

namespace CodeAnalyzation.Models.Execution.Test;

public class EvalTests
{
    [Fact] public void AddVariables() => "var a = 2; var b = 3; a + b;".Eval().Should().Be(5);
    [Fact] public void IfTrue() => "var a = true; if(a) { \"yup\" } else { \"nope\" };".Eval().Should().Be("yup");
    [Fact] public void IfFalse() => "var a = false; if(a) { \"yup\" } else { \"nope\"; }".Eval().Should().Be("nope");
    [Fact] public void ForLoop() => "var a = 0; for(var i = 0; i < 10; i++){ a += i; } a;".Eval().Should().Be(45);
    [Fact] public void WhileLoop() => "var a = 0; var i = 0; while(i < 10){ a += i; i++; } a;".Eval().Should().Be(45);
    [Fact] public void DoWhileLoop() => "var a = 0; var i = 0; do { a += i; i++; } while(i < 10); a;".Eval().Should().Be(45);
    [Fact] public void ForEachLoop() => "var a = 0; foreach(var i in new int[]{ 1, 2, 3, 4, 5 }) { a += i; } a;".Eval().Should().Be(15);
}
