using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Assignment3.Utils
{
    public static class DSAUtils
    {
        private static readonly BigInteger RandomMultiplier = BigInteger.Parse("16807");
        private static readonly BigInteger RandomModulus = BigInteger.Parse("2147483647");

        public static BigInteger ModInverse(BigInteger a, BigInteger b)
        {
            var numerator = a % b;
            var divideBy = b;

            var thisX = BigInteger.Zero;
            var prevX = BigInteger.One;

            while (divideBy > BigInteger.Zero && divideBy.Sign == 1)
            {
                // divisor > 0

                var result = BigInteger.Divide(numerator, divideBy);
                var remaining = numerator % divideBy;

                if (remaining <= BigInteger.Zero)
                {
                    // remainder <= 0

                    break;
                }

                var next = BigInteger.Subtract(prevX, BigInteger.Multiply(thisX, result));

                prevX = thisX;
                thisX = next;

                numerator = divideBy;
                divideBy = remaining;
            }

            if (!divideBy.Equals(BigInteger.One))
            {
                throw new ArgumentException("Arguments are not relatively prime");
            }

            return thisX.Sign < 0 ? BigInteger.Add(thisX, b) : thisX;
        }

        public static IEnumerable<BigInteger> RandomSequence(BigInteger minValue, BigInteger maxValue, BigInteger size)
        {
            var list = new List<BigInteger>();

            // Add 1 to maxValue to ensure that the resulting values can include max
            maxValue = BigInteger.Add(maxValue, BigInteger.One);

            var diff = BigInteger.Subtract(maxValue, minValue);

            for (var i = BigInteger.Zero; IsLessThan(i, size); i = BigInteger.Add(i, BigInteger.One))
            {
                var rand = RandomNumber(GenerateSeed());

                while (IsLessThan(rand, minValue))
                {
                    // square value while less than min
                    rand = BigInteger.Pow(rand, 2);
                }

                // make sure minValue <= rand <= maxValue
                var result = rand % diff;

                result = BigInteger.Add(result, minValue);

                list.Add(result);
            }

            return list;
        }

        public static BigInteger RandomInteger(BigInteger minValue, BigInteger maxValue)
        {
            return RandomSequence(minValue, maxValue, BigInteger.One).First();
        }

        private static BigInteger GenerateSeed()
        {
            var millis = 0;

            while (millis == 0)
            {
                millis = DateTime.Now.Millisecond;
            }

            return BigInteger.Parse(string.Format("{0}", millis));
        }

        private static BigInteger RandomNumber(BigInteger seed)
        {
            var rnd = seed;

            rnd = BigInteger.Multiply(RandomMultiplier, rnd);
            rnd = rnd % RandomModulus;

            return rnd;
        }

        private static bool IsLessThan(BigInteger a, BigInteger b)
        {
            return !IsGreaterThan(a, b) && !a.Equals(b);
        }

        private static bool IsGreaterThan(BigInteger a, BigInteger b)
        {
            return a > b;
        }
        
        public static bool IsProbablyPrime(BigInteger candidate)
        {
            if (candidate.Equals(BigInteger.Zero) || candidate.Equals(BigInteger.One))
            {
                return false;
            }

            var coprime = RandomInteger(BigInteger.One, BigInteger.Subtract(candidate, BigInteger.One));

            while (!IsCoprimeTo(candidate, coprime))
            {
                coprime = BigInteger.Add(coprime, BigInteger.One);
            }

            var exponent = BigInteger.Subtract(candidate, BigInteger.One);

            var result = BigInteger.ModPow(coprime, exponent, candidate);

            return result.Equals(BigInteger.One);
        }

        private static bool IsCoprimeTo(BigInteger a, BigInteger b)
        {
            return BigInteger.GreatestCommonDivisor(a, b).Equals(BigInteger.One);
        }
    }
}