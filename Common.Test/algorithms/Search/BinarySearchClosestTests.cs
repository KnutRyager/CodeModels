using Common.Algorithms;
using Common.Algorithms.Search;
using Xunit;

namespace Common.Test.algorithms.Search;

public class BinarySearchClosestTests
{
    [Fact]
    public void Find_Closest()
    {
        Assert.Equal(1, BinarySearchClosest.BinarySearch(new[] { 1, 2, 3, 6 }, 1));
        Assert.Equal(2, BinarySearchClosest.BinarySearch(new[] { 1, 2, 3, 6 }, 2));
        Assert.Equal(3, BinarySearchClosest.BinarySearch(new[] { 1, 2, 3, 6 }, 3));
        Assert.Equal(3, BinarySearchClosest.BinarySearch(new[] { 1, 2, 3, 6 }, 4));
        Assert.Equal(6, BinarySearchClosest.BinarySearch(new[] { 1, 2, 3, 6 }, 5));
        Assert.Equal(6, BinarySearchClosest.BinarySearch(new[] { 1, 2, 3, 6 }, 6));
        Assert.Equal(0, BinarySearchClosest.BinarySearch(new[] { 0, 100 }, 1));
        Assert.Equal(100, BinarySearchClosest.BinarySearch(new[] { 0, 100 }, 99));
    }

    [Fact]
    public void Find_Closest_CustomComparer()
    {
        Assert.Equal(1, BinarySearchClosest.BinarySearch(new[] { 1, 2, 3, 6 }, 1, (x, y) => x - y));
        Assert.Equal(2, BinarySearchClosest.BinarySearch(new[] { 1, 2, 3, 6 }, 2, (x, y) => x - y));
        Assert.Equal(3, BinarySearchClosest.BinarySearch(new[] { 1, 2, 3, 6 }, 3, (x, y) => x - y));
        Assert.Equal(3, BinarySearchClosest.BinarySearch(new[] { 1, 2, 3, 6 }, 4, (x, y) => x - y));
        Assert.Equal(6, BinarySearchClosest.BinarySearch(new[] { 1, 2, 3, 6 }, 5, (x, y) => x - y));
        Assert.Equal(6, BinarySearchClosest.BinarySearch(new[] { 1, 2, 3, 6 }, 6, (x, y) => x - y));
        Assert.Equal(0, BinarySearchClosest.BinarySearch(new[] { 0, 100 }, 1, (x, y) => x - y));
        Assert.Equal(100, BinarySearchClosest.BinarySearch(new[] { 0, 100 }, 99, (x, y) => x - y));
    }

    [Fact]
    public void Floor()
    {
        Assert.Equal(1, BinarySearchClosest.BinarySearchFloor(new[] { 1, 2, 3, 6 }, 1));
        Assert.Equal(2, BinarySearchClosest.BinarySearchFloor(new[] { 1, 2, 3, 6 }, 2));
        Assert.Equal(3, BinarySearchClosest.BinarySearchFloor(new[] { 1, 2, 3, 6 }, 3));
        Assert.Equal(3, BinarySearchClosest.BinarySearchFloor(new[] { 1, 2, 3, 6 }, 4));
        Assert.Equal(3, BinarySearchClosest.BinarySearchFloor(new[] { 1, 2, 3, 6 }, 5));
        Assert.Equal(6, BinarySearchClosest.BinarySearchFloor(new[] { 1, 2, 3, 6 }, 6));
        Assert.Equal(0, BinarySearchClosest.BinarySearchFloor(new[] { 0, 100 }, 1));
        Assert.Equal(0, BinarySearchClosest.BinarySearchFloor(new[] { 0, 100 }, 99));
    }

    [Fact]
    public void Ceil()
    {
        Assert.Equal(1, BinarySearchClosest.BinarySearchCeil(new[] { 1, 2, 3, 6 }, 1));
        Assert.Equal(2, BinarySearchClosest.BinarySearchCeil(new[] { 1, 2, 3, 6 }, 2));
        Assert.Equal(3, BinarySearchClosest.BinarySearchCeil(new[] { 1, 2, 3, 6 }, 3));
        Assert.Equal(6, BinarySearchClosest.BinarySearchCeil(new[] { 1, 2, 3, 6 }, 4));
        Assert.Equal(6, BinarySearchClosest.BinarySearchCeil(new[] { 1, 2, 3, 6 }, 5));
        Assert.Equal(6, BinarySearchClosest.BinarySearchCeil(new[] { 1, 2, 3, 6 }, 6));
        Assert.Equal(100, BinarySearchClosest.BinarySearchCeil(new[] { 0, 100 }, 1));
        Assert.Equal(100, BinarySearchClosest.BinarySearchCeil(new[] { 0, 100 }, 99));
    }
}
