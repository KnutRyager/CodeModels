using System.Linq;
using CodeAnalyzation;
using Common.DataStructures;
using Common.Util;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Xunit;
using static CodeAnalyzation.SemanticExtensions;
using static CodeAnalyzation.SyntaxNodeExtensions;

namespace Generator.Test;

public class MemberDependenciesTests
{
    [Fact]
    public void FindDirectPropertyDependencies()
    {
        var code = @"
using System;
using System.Collections.Generic;

public class ClassA {
    public int A => 1;
    public int B => A;
    public int C => A + B;
    public int D => A * A + B + B + C + C;
    public ClassB E => new ClassB();
    public int F => E.B_A;
    public ClassA G => E.C.E.C;
    private ClassA H { get => G; set { } }
    private List<ClassA> I { get; set; }
    internal List<ClassA> J => I;
    public int K => E.D.C_A + E.D.C_B + A + E.B_A;
    internal int L => I.Select2(x => x.E.B_A);
    internal int M() => A;
    internal int N() => A + M();
    internal int O = 5;
    internal int P = O;
}

public class ClassB
{
    public int B_A => 1;
    public int B_B => B_A;
    public ClassA C => new();
    public ClassC D => new();
}

public class ClassC
{
    public int C_A => 1;
    public int C_B => C_A;
    public ClassA C_C => new();
    public ClassB C_D => new();
}

public static class Extensions
{
    public static int Select2(this List<ClassA> a, Func<ClassA, int> selector) => 1;
}".ParseKeepSemanticModel();
        var dependencies = code.Compilation.GetClasses().GetDependencies(code.Model).SelectMany(x => x.Value.Select(x => x.Dependencies.SelectMany(y => y.Transform(z => z.Name)
            .TransformByTransformedParent<string>((node, parent) => $"{StringUtil.FilterJoin(parent, node)}").ToList()).Distinct())).ToList();
        dependencies[00].Should().BeEquivalentTo();
        dependencies[01].Should().BeEquivalentTo("A");
        dependencies[02].Should().BeEquivalentTo("A", "B");
        dependencies[03].Should().BeEquivalentTo("A", "B", "C");
        dependencies[04].Should().BeEquivalentTo();
        dependencies[05].Should().BeEquivalentTo("E", "E.B_A");
        dependencies[06].Should().BeEquivalentTo("E", "E.C");
        dependencies[07].Should().BeEquivalentTo("G");
        dependencies[08].Should().BeEquivalentTo();
        dependencies[09].Should().BeEquivalentTo("I");
        dependencies[10].Should().BeEquivalentTo("A", "E", "E.D", "E.D.C_A", "E.D.C_B", "E.B_A");
        dependencies[11].Should().BeEquivalentTo("I", "E", "E.B_A");
        dependencies[12].Should().BeEquivalentTo("A");
        dependencies[13].Should().BeEquivalentTo("A", "M");
        dependencies[14].Should().BeEquivalentTo();
        dependencies[15].Should().BeEquivalentTo("O");
    }

    [Fact]
    public void CollectPropertiesTest() => new Tree<string>("0", "A", new("B", new Tree<string>("C", "1", "2")))
            .TransformByTransformedParent<string>((node, parent) => $"{StringUtil.FilterJoin(parent, node)}")
            .ToList().Should().BeEquivalentTo("0", "0.A", "0.B", "0.B.C", "0.B.C.1", "0.B.C.2");
}