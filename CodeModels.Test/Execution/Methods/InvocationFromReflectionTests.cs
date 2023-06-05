using System;
using CodeModels.Factory;
using TestCommon;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;
using static CodeModels.Factory.CodeModelsFromReflection;

namespace CodeModels.Test.Execution.Functions;

public class InvocationFromReflectionTests
{
    [Fact]
    public void ObjectToString()
    {
        var invocation = Invocation<object, string?>(x => x.ToString(), Identifier("x"));
        invocation.CodeModelEqual("x.ToString()");
    }

    [Fact]
    public void IdentifierToInvocation()
    {
        var intArg = CodeModelFactory.Literal(1337);
        var a = Identifier<A>("x");
        var invocation = a.GetInvocation(x => x.B);
        invocation.CodeModelEqual("x.B()");
    }

    [Fact]
    public void IdentifierToInvocation3()
    {
        var intArg = CodeModelFactory.Literal(1337);
        var a = Identifier<A>("x");
        var invocation = a.GetInvocation(x => x.C, new[] { intArg });
        invocation.CodeModelEqual("x.C(1337)");
    }

    [Fact(Skip = "Not implemented")]
    public void IdentifierToStaticInvocation()
    {
        var intArg = CodeModelFactory.Literal(1337);
        var a = Identifier<int>("x");
        var invocation = a.GetInvocation(x => Math.Sqrt);
        invocation.CodeModelEqual("Math.sqrt(x)");
    }
}

file class A
{
    public int B() => 0;
    public int C2(int x) => x;
    public int C(int x, int y) => x;
    public string D() => "";
}

