using Common.DataStructures;
using Xunit;

namespace Common.Tests.Algorithms;

public class TreeTests
{
    [Fact]
    public void ConstructionAndAccess()
    {
        var tree = new Tree<string>("root",
            "1a",
            new("1b",
                "2a"),
            "1c",
            new("1d",
                "2b", new("2c",
                    "3a", "3b", "3c", "3d")
                ),
            new("1e",
               "2d", "2e", "2f", "2g"
            )
        );
        Assert.Equal("root", tree);
        Assert.Equal("1a", tree[0]);
        Assert.Equal("1b", tree[1]);
        Assert.Equal("2a", tree[1][0]);
        Assert.Equal("1c", tree[2]);
        Assert.Equal("1d", tree[3]);
        Assert.Equal("2b", tree[3][0]);
        Assert.Equal("2c", tree[3][1]);
        Assert.Equal("3a", tree[3][1][0]);
        Assert.Equal("3b", tree[3][1][1]);
        Assert.Equal("3c", tree[3][1][2]);
        Assert.Equal("3d", tree[3][1][3]);
        Assert.Equal("1e", tree[4]);
        Assert.Equal("2d", tree[4][0]);
        Assert.Equal("2e", tree[4][1]);
        Assert.Equal("2f", tree[4][2]);
        Assert.Equal("2g", tree[4][3]);
    }

    [Fact]
    public void Transform()
    {
        var tree = new Tree<string>("root",
            "1a",
            new("1b",
                "2a", "2b", "2c"),
            "1c"
        );
        var transformed = tree.Transform((x, y) => $"{x}({string.Join(",", y)})");
        Assert.Equal("root(1a,1b,1c)", transformed);
        Assert.Equal("1a()", transformed[0]);
        Assert.Equal("1b(2a,2b,2c)", transformed[1]);
        Assert.Equal("2a()", transformed[1][0]);
        Assert.Equal("2b()", transformed[1][1]);
        Assert.Equal("2c()", transformed[1][2]);
        Assert.Equal("1c()", transformed[2]);
    }

    [Fact]
    public void To()
    {
        var tree = new Tree<int>(2,
            5,
            new(11,
                2, 3),
            17
        );
        var transformed = tree.To<A>((x, y) => new(x * Math.Max(1, y.Select(y => y.Value).Sum()), y.ToArray()));
        Assert.Equal(154, transformed.Value);
        Assert.Equal(5, transformed.Children[0].Value);
        Assert.Equal(55, transformed.Children[1].Value);
        Assert.Equal(2, transformed.Children[1].Children[0].Value);
        Assert.Equal(3, transformed.Children[1].Children[1].Value);
        Assert.Equal(17, transformed.Children[2].Value);
    }

    [Fact]
    public void TransformTo()
    {
        var tree = new Tree<int>(2,
            5,
            new(11,
                2, 3),
            17
        );
        var transformed = tree.Transform((x, y) => x * Math.Max(1, y.Sum())).To<A>((x, y) => new(x, y.ToArray()));
        Assert.Equal(66, transformed.Value);
        Assert.Equal(5, transformed.Children[0].Value);
        Assert.Equal(55, transformed.Children[1].Value);
        Assert.Equal(2, transformed.Children[1].Children[0].Value);
        Assert.Equal(3, transformed.Children[1].Children[1].Value);
        Assert.Equal(17, transformed.Children[2].Value);
    }

    [Fact]
    public void Traverse()
    {
        var tree = new Tree<int>(2,
            5,
            new(11,
                2, 3),
            17
        );
        var sum = 0;
        tree.Traverse(tree, x => sum += x);
        Assert.Equal(40, sum);
    }

    private record A(int Value, A[] Children);
}
