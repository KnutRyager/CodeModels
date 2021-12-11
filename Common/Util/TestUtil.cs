using System.Collections.Generic;
using System.Linq;

namespace Common.Util
{
    public static class TestUtil
    {
        /// <summary>
        /// IEnumerable<object[]> is required by Theory tests.
        /// Use as member variable passed into theory.
        /// </summary>
        public static IEnumerable<object?> TestData<T>(params T[] data) => data.Select(x => new object?[] { x });
    }
}