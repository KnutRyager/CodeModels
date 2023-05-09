using Xunit;
using static CodeModels.Factory.CodeModelFactory;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeModels.Models.Execution.Test;

public class ProgramModelExecutionContextTests
{
    [Fact]
    public void SetAndGetVariable()
    {
        var context = new ProgramModelExecutionContext();
        context.EnterScope();
        context.SetValue("test", Literal("a"), true);
        context.GetValue("test").Should().Be(Literal("a"));
    }

    [Fact]
    public void DefineAndAssignVariable()
    {
        var context = new ProgramModelExecutionContext();
        context.EnterScope();
        context.DefineVariable("test");
        var assignment = Assignment(Identifier("test"), Literal("a"));
        assignment.Evaluate(context);
        context.GetValue("test").Should().Be(Literal("a"));
    }

    [Fact]
    public void AddAssignment()
    {
        var context = new ProgramModelExecutionContext();
        context.EnterScope();
        context.SetValue("test", Literal(2), true);
        var assignment = Assignment(Identifier("test"), SyntaxKind.AddAssignmentExpression, Literal(3));
        assignment.Evaluate(context);
        context.GetValue("test").Should().Be(Literal(5));
    }

    [Fact]
    public void AddTwoVariables()
    {
        var context = new ProgramModelExecutionContext();
        context.EnterScope();
        context.SetValue("a", Literal(2), true);
        context.SetValue("b", Literal(3), true);
        context.SetValue("c", BinaryExpression(Identifier("a"), OperationType.Plus, Identifier("b")).Evaluate(context), true);
        context.GetValue("c").EvaluatePlain(context).Should().Be(5);
    }

    [Fact]
    public void SimpleIf()
    {
        var context = new ProgramModelExecutionContext();
        context.EnterScope();
        context.SetValue("a", Literal(0), true);
        If(Literal(true), Assignment(Identifier("a"), Literal(1337)).AsStatement()).Evaluate(context);
        context.GetValue("a").EvaluatePlain(context).Should().Be(1337);
        If(Literal(false), Assignment(Identifier("a"), Literal("nope")).AsStatement()).Evaluate(context);
        context.GetValue("a").EvaluatePlain(context).Should().Be(1337);
    }

    [Fact]
    public void MultiIfs()
    {
        var context = new ProgramModelExecutionContext();
        context.EnterScope();
        context.DefineVariable("a");
        var ifs = MultiIf(new[] {
            If(BinaryExpression(Identifier("a"), OperationType.Equals, Literal(0)), Assignment(Identifier("a"), Literal("equal")).AsStatement()),
            If(BinaryExpression(Identifier("a"), OperationType.GreaterThan, Literal(1)), Assignment(Identifier("a"), Literal("great")).AsStatement()),
            If(BinaryExpression(Identifier("a"), OperationType.LessThan, Literal(0)), Assignment(Identifier("a"), Literal("less")).AsStatement()),
             },
            Assignment(Identifier("a"), Literal("one")).AsStatement());
        context.SetValue("a", Literal(0));
        ifs.Evaluate(context);
        context.GetValue("a").EvaluatePlain(context).Should().Be("equal");
        context.SetValue("a", Literal(2));
        ifs.Evaluate(context);
        context.GetValue("a").EvaluatePlain(context).Should().Be("great");
        context.SetValue("a", Literal(-2));
        ifs.Evaluate(context);
        context.GetValue("a").EvaluatePlain(context).Should().Be("less");
        context.SetValue("a", Literal(1));
        ifs.Evaluate(context);
        context.GetValue("a").EvaluatePlain(context).Should().Be("one");
    }

    [Fact]
    public void SwitchCase()
    {
        var context = new ProgramModelExecutionContext();
        context.EnterScope();
        context.DefineVariable("a");
        var switchCase = Switch(Identifier("a"), new[] {
            Case(Literal(1),Assignment(Identifier("a"), Literal("one")).AsStatement()),
            Case(Literal(2),Assignment(Identifier("a"), Literal("two")).AsStatement()),
            Case(Literal(3),Assignment(Identifier("a"), Literal("three")).AsStatement()),
            },
            Assignment(Identifier("a"), Literal("four")).AsStatement());
        context.SetValue("a", Literal(1));
        switchCase.Evaluate(context);
        context.GetValue("a").EvaluatePlain(context).Should().Be("one");
        context.SetValue("a", Literal(2));
        switchCase.Evaluate(context);
        context.GetValue("a").EvaluatePlain(context).Should().Be("two");
        context.SetValue("a", Literal(3));
        switchCase.Evaluate(context);
        context.GetValue("a").EvaluatePlain(context).Should().Be("three");
        context.SetValue("a", Literal(4));
        switchCase.Evaluate(context);
        context.GetValue("a").EvaluatePlain(context).Should().Be("four");
    }

    [Fact]
    public void SimpleForLoop()
    {
        var context = new ProgramModelExecutionContext();
        context.EnterScope();
        context.SetValue("a", Literal(0), true);
        var forLoop = For("i", Literal(10), Assignment(Identifier("a"), SyntaxKind.AddAssignmentExpression, Identifier("i")).AsStatement());
        forLoop.Evaluate(context);
        context.GetValue("a").EvaluatePlain(context).Should().Be(45);
    }

    [Fact]
    public void WhileLoop()
    {
        var context = new ProgramModelExecutionContext();
        context.EnterScope();
        context.SetValue("a", Literal(0), true);
        var whileLoop = While(BinaryExpression(Identifier("a"), OperationType.LessThan, Literal(10)),
            Assignment(Identifier("a"), SyntaxKind.AddAssignmentExpression, Literal(1)).AsStatement());
        whileLoop.Evaluate(context);
        context.GetValue("a").EvaluatePlain(context).Should().Be(10);
    }

    [Fact]
    public void DoWhileLoop()
    {
        var context = new ProgramModelExecutionContext();
        context.EnterScope();
        context.SetValue("a", Literal(0), true);
        var doWhileLoop = Do(Assignment(Identifier("a"), SyntaxKind.AddAssignmentExpression, Literal(1)).AsStatement(),
            BinaryExpression(Identifier("a"), OperationType.LessThan, Literal(10)));
        doWhileLoop.Evaluate(context);
        context.GetValue("a").EvaluatePlain(context).Should().Be(10);
    }

    [Fact]
    public void ForEachLoop()
    {
        var context = new ProgramModelExecutionContext();
        context.EnterScope();
        context.SetValue("a", Literal(0), true);
        var forEachLoop = ForEach("b",
            Literal(new object[] { 1, 2, 3, 4, 5 }),
            Assignment(Identifier("a"), SyntaxKind.AddAssignmentExpression, Identifier("b")).AsStatement());
        forEachLoop.Evaluate(context);
        context.GetValue("a").EvaluatePlain(context).Should().Be(15);
    }

    [Fact]
    public void AssignToVariableInLowerScope()
    {
        var context = new ProgramModelExecutionContext();
        context.EnterScope();
        context.DefineVariable("a");
        context.EnterScope();
        context.SetValue("a", Literal(1337));
        context.ExitScope();
        context.GetValue("a").EvaluatePlain(context).Should().Be(1337);
    }

    [Fact]
    public void SetUndefinedVariable() => Assert.Throws<ProgramModelExecutionException>(() => new ProgramModelExecutionContext().SetValue("test", Literal("a")));
    [Fact]
    public void SetVariableWhenNoScope() => Assert.Throws<ProgramModelExecutionException>(() => new ProgramModelExecutionContext().SetValue("test", Literal("a"), true));
    [Fact]
    public void PopWhenNoScopes() => Assert.Throws<ProgramModelExecutionException>(() => new ProgramModelExecutionContext().ExitScope());
    [Fact]
    public void StackOverflow() => Assert.Throws<ProgramModelExecutionException>(() =>
    {
        var context = new ProgramModelExecutionContext();
        for (var i = 0; i <= 10001; i++) context.EnterScope();
    });


}
