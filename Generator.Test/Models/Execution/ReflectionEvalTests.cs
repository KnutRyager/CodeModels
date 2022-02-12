using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using System.Linq;

namespace CodeAnalyzation.Models.Execution.Test;

public class ReflectionEvalTests
{
    [Fact] public void StaticField() => "System.Math.PI".Eval().Should().Be(Math.PI);
    //[Fact] public void StaticProperty() => "System.Math.Sqrt(25)".Eval().Should().Be(5);
    [Fact] public void StaticMethodCall() => "System.Math.Sqrt(25)".Eval().Should().Be(5);
    [Fact] public void EnumValue() => "System.AttributeTargets.Method".Eval().Should().Be(AttributeTargets.Method);
    [Fact] public void TypeOf() => "typeof(string)".Eval().Should().Be(typeof(string));
    [Fact] public void SizeOf() => "sizeof(int)".Eval().Should().Be(sizeof(int));
    [Fact] public void InstanceProperty() => "\"test\".Length".Eval().Should().Be("test".Length);
    [Fact] public void InstanceMethod() => "\"a_a\".Replace('a','b')".Eval().Should().Be("b_b");
    [Fact] public void InstantiateStruct() => "new System.DateTime(2022, 2, 12)".Eval().Should().BeEquivalentTo(new DateTime(2022, 2, 12));
    [Fact] public void InstantiateObject() => "new System.Text.StringBuilder(\"1337\").ToString()".Eval().Should().Be("1337");
    [Fact] public void InstantiateObjectWithInitializer() => "new System.Text.StringBuilder() { Capacity = 1337 }.Capacity".Eval().Should().Be(1337);
    [Fact] public void InstantiateArray() => "new int[] { 1, 2, 3 }".Eval().Should().BeEquivalentTo(new int[] { 1, 2, 3 });
    [Fact] public void InstantiateArrayImplicit() => "new [] { 1, 2, 3 }".Eval().Should().BeEquivalentTo(new[] { 1, 2, 3 });
    [Fact] public void InstantiateList() => "new System.Collections.Generic.List<int>() { 1, 2, 3 }".Eval().Should().BeEquivalentTo(new List<int>() { 1, 2, 3 });
    [Fact] public void InstantiateListImplicitInit() => "new System.Collections.Generic.List<int>(new[] { 0, 1337, 0 }) { [2] = 5, [0] = 2 }".Eval().Should().BeEquivalentTo(new List<int>(new[] { 0, 1337, 0 }) { [2] = 5, [0] = 2 });
    [Fact] public void InstantiateSet() => "new System.Collections.Generic.HashSet<int>() { 1, 2, 3 }".Eval().Should().BeEquivalentTo(new HashSet<int>() { 1, 2, 3 });
    [Fact] public void InstantiateDictionary() => "new System.Collections.Generic.Dictionary<int, string>() { { 1, \"a\" }, { 2, \"b\" } }".Eval().Should().BeEquivalentTo(new Dictionary<int, string>() { { 1, "a" }, { 2, "b" } });
    [Fact] public void InstantiateDictionaryImplicitInit() => "new System.Collections.Generic.Dictionary<int, string> { [1] = \"a\", [2] = \"b\" }".Eval().Should().BeEquivalentTo(new Dictionary<int, string> { [1] = "a", [2] = "b" });
    // ConcurrentDictionary fails due to wrong assembly in semantic symbol...
    //[Fact] public void InstantiateConcurrentDictionary() => "new System.Collections.Concurrent.ConcurrentDictionary<int, string> { [1] = \"a\", [2] = \"b\" }".Eval().Should().BeEquivalentTo(new ConcurrentDictionary<int, string> { [1] = "a", [2] = "b" });
}
