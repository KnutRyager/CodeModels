using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Common.Algorithms
{
    public static class GreatestCommonDivisor
    {
        public static long GCD(long a, long b) => (long)BigInteger.GreatestCommonDivisor(new BigInteger(a), new BigInteger(b));

        public static long GCD(IEnumerable<int> numbers) => GCD(numbers.Select(x => (long)x).ToList());

        public static long GCD(IEnumerable<long> numbers)
        {
            var numberList = numbers.ToList();
            if (numberList.Count == 0) return 0;
            var gcd = numberList[0];
            for (var i = 1; i < numberList.Count; i++)
            {
                gcd = GCD(gcd, numberList[i]);
            }
            return gcd;
        }

        public static List<long> DivideByGCD(IEnumerable<long> numbers)
        {
            var gcd = GCD(numbers);
            return numbers.Select(x => x / gcd).ToList();
        }

        public static List<int> DivideByGCD(IEnumerable<int> numbers)
        {
            var gcd = GCD(numbers);
            return numbers.Select(x => x / (int)gcd).ToList();
        }
    }
}