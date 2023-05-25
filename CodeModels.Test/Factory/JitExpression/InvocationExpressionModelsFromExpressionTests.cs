using CodeModels.Factory;
using FluentAssertions;
using Xunit;

namespace CodeModels.Test.Factory.JitExpression;

public class InvocationExpressionModelsFromExpressionTests
{
    private A a = new A();

    [Fact] public void MethodInstance() => CodeModelsFromExpression.GetCode(x => a.InstanceMethod()).Should().Be("a.InstanceMethod()");
    [Fact] public void MethodInstanceChained() => CodeModelsFromExpression.GetCode(x => a.InstanceMethodChain().InstanceMethod()).Should().Be("a.InstanceMethodChain().InstanceMethod()");
    [Fact] public void MethodStatic() => CodeModelsFromExpression.GetCode(x => A.StaticMethod()).Should().Be("A.StaticMethod()");
    [Fact] public void MethodStaticChained() => CodeModelsFromExpression.GetCode(x => A.StaticMethodChain().InstanceMethod()).Should().Be("A.StaticMethodChain().InstanceMethod()");
    [Fact] public void Extension() => CodeModelsFromExpression.GetCode(x => a.AExtension()).Should().Be("a.AExtension()");
    [Fact] public void ExtensionChained() => CodeModelsFromExpression.GetCode(x => a.AExtensionChain().InstanceMethod()).Should().Be("a.AExtensionChain().InstanceMethod()");
    [Fact] public void ExtensionMultipleArgs() => CodeModelsFromExpression.GetCode(x => a.AExtensionMultipleArgs(1)).Should().Be("a.AExtensionMultipleArgs(1)");
}

public class A
{
    public int InstanceMethod() => 1;
    public A InstanceMethodChain() => this;
    public static int StaticMethod() => 1;
    public static A StaticMethodChain() => new A();
}

public static class AExtensions
{
    public static int AExtension(this A a) => a.InstanceMethod();
    public static A AExtensionChain(this A a) => a;
    public static int AExtensionMultipleArgs(this A a, int i) => i;
}