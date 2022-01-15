using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Algorithms;

/// <summary>
/// KnapSack implementation, using GCD (Greatest Common Divisor) optimization.
/// 
/// Returns the selected elements, thus the caller must calculate the "value" themselves.
/// This avoids any confusion with scaled numbers...
/// </summary>
public static class KnapSackAlgorithm
{
    public static List<T> KnapSack<T>(long capacity, IEnumerable<T> items, Func<T, int> valueGetter, Func<T, int> weightGetter)
    => KnapSack(capacity, items, x => valueGetter(x), x => (long)weightGetter(x));

    public static List<T> KnapSack<T>(decimal capacity, IEnumerable<T> items, Func<T, decimal> valueGetter, Func<T, decimal> weightGetter)
    => KnapSack((long)(capacity * 100), items, x => (long)(valueGetter(x) * 100), x => (long)(weightGetter(x) * 100));

    public static List<T> KnapSack<T>(long capacity, IEnumerable<T> items, Func<T, long> valueGetter, Func<T, long> weightGetter)
    {
        // Optimize with Greatest Common Divisor
        var value = GreatestCommonDivisor.DivideByGCD(items.Select(valueGetter));
        var weightUnadjusted = items.Select(weightGetter);
        var weightGcd = GreatestCommonDivisor.GCD(weightUnadjusted);
        var weight = weightUnadjusted.Select(x => x / weightGcd).ToArray();
        capacity /= weightGcd;

        var itemsArray = items.ToArray();
        int itemsCount = itemsArray.Length;
        var solution = new long[itemsCount + 1, capacity + 1];
        var itemTaken = new int[itemsCount + 1, capacity + 1];
        var path = new (int x, int y)[itemsCount + 1, capacity + 1];
        for (var i = 0; i <= itemsCount; ++i)
        {
            for (var w = 0; w <= capacity; ++w)
            {
                if (i == 0 || w == 0)
                {
                    solution[i, w] = 0;
                }
                else if (weight[i - 1] <= w)
                {
                    var withPrevious = solution[i - 1, w];
                    var withCurrent = value[i - 1] + solution[i - 1, w - weight[i - 1]];
                    if (withPrevious > withCurrent)
                    {
                        path[i, w] = (i - 1, w);
                        solution[i, w] = withPrevious;
                    }
                    else
                    {
                        path[i, w] = (i - 1, (int)(w - weight[i - 1]));
                        solution[i, w] = withCurrent;
                        itemTaken[i, w] = i;
                    }
                }
                else
                {
                    solution[i, w] = solution[i - 1, w];
                    path[i, w] = (i - 1, w);
                }
            }
        }
        var itemIndexes = new List<int>();
        var coordinates = (x: itemsCount, y: capacity);
        while (coordinates.x > 0 || coordinates.y > 0)
        {
            var item = itemTaken[coordinates.x, coordinates.y];
            if (item > 0) itemIndexes.Add(item);
            coordinates = path[coordinates.x, coordinates.y];
        }

        return itemIndexes.Select(x => itemsArray[x - 1]).ToList();
    }
}
