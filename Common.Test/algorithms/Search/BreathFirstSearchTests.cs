using Common.Algorithms.Search;
using Common.DataStructures;
using FluentAssertions;
using Xunit;

namespace Common.Test.algorithms.Search;

public class BreathFirstSearchTests
{
    [Fact]
    public void Search_Root() => BreathFirstSearch.Search(
        NodeFactory.Create(1), 1).Value
        .Should().Be(1);

    [Fact]
    public void Search_Depth1() => BreathFirstSearch.Search(
        NodeFactory.Create(1, NodeFactory.Create(2)), 2).Value
        .Should().Be(2);
    [Fact]
    public void Search_NotFound() => BreathFirstSearch.Search(
        NodeFactory.Create(1), 2)
        .Should().BeNull();

    [Fact]
    public void Search_CustomCriteria_Root() => BreathFirstSearch.Search(
        NodeFactory.Create(1), x => x.Value == 1, x => x.Children).Value
        .Should().Be(1);

    [Fact]
    public void Search_CustomCriteria_Depth1() => BreathFirstSearch.Search(
        NodeFactory.Create(1, NodeFactory.Create(2)), x => x.Value == 2, x => x.Children).Value
        .Should().Be(2);

    [Fact]
    public void FindPath_CustomCriteria_Root() => BreathFirstSearch.FindPath(
        NodeFactory.Create(1, NodeFactory.Create(2)), x => x.Value == 1, x => x.Children).Select(x => x.Value)
        .Should().BeEquivalentTo(new List<int>() { 1 });

    [Fact]
    public void FindPath_CustomCriteria_Depth1() => BreathFirstSearch.FindPath(
        NodeFactory.Create(1, NodeFactory.Create(2)), x => x.Value == 2, x => x.Children).Select(x => x.Value)
        .Should().BeEquivalentTo(new List<int>() { 1, 2 });

    [Fact]
    public void FindPath_CustomCriteria_Depth2() => BreathFirstSearch.FindPath(
        NodeFactory.Create(1,
            NodeFactory.Create(2,
                NodeFactory.Create(3)),
            NodeFactory.Create(4,
                NodeFactory.Create(5),
                NodeFactory.Create(6),
                NodeFactory.Create(7))), x => x.Value == 6, x => x.Children).Select(x => x.Value)
        .Should().BeEquivalentTo(new List<int>() { 1, 4, 6 });

    [Fact]
    public void Search_NullWhenNoSolution() => BreathFirstSearch.Search(
        NodeFactory.Create(1, NodeFactory.Create(2)), 3)
        .Should().BeNull();

    [Fact]
    public void Search_CyclicalTerminates() => BreathFirstSearch.Search(
        NodeFactory.Create(1), x => x.Value == 2, x => new[] { x })
        .Should().BeNull(null);

    [Fact]
    public void Search_CustomCriteria_NullWhenNoSolution() => BreathFirstSearch.Search(
        NodeFactory.Create(1, NodeFactory.Create(2)), x => x.Value == 3, x => x.Children)
        .Should().BeNull();

    [Fact]
    public void Search_CustomCriteria_CyclicalTerminates() => BreathFirstSearch.Search(
        NodeFactory.Create(1), x => x.Value == 2, x => new[] { x })
        .Should().BeNull(null);

    [Fact]
    public void FindPath_CustomCriteria_NullWhenNoSolution() => BreathFirstSearch.FindPath(
        NodeFactory.Create(1, NodeFactory.Create(2)), x => x.Value == 3, x => x.Children)
        .Should().BeNull();

    [Fact]
    public void FindPath_CustomCriteria_CyclicalTerminates() => BreathFirstSearch.FindPath(
        NodeFactory.Create(1), x => x.Value == 2, x => new[] { x })
        .Should().BeNull(null);

}
