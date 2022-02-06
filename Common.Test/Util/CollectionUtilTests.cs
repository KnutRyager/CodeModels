using Common.Util;
using Xunit;

namespace Common.Tests
{
    public class CollectionUtilTests
    {
        [Fact]
        public void Bucketify()
        {
            var buckets = new[] { 10, 20, 30 };
            var items = new[] { 9, 10, 11, 19, 20, 21, 29, 30, 31 };
            var bucketed = CollectionUtil.Bucketify(buckets, items);
            Assert.Equal(new[] { -1, 0, 0, 0, 1, 1, 1, 2, 2 }, bucketed);
        }

        [Fact]
        public void CombineFilters()
        {
            var list1 = new[] { 1, 2, 3 };
            var list2 = new[] { 2, 3, 4, 5, 6 };
            var combined = CollectionUtil.CombineFilters(list1, list2, null);
            Assert.Equal(new[] { 2, 3 }, combined);
        }
    }
}
