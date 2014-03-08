using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assignment2.Model;

namespace Assignment2.Utils
{
    /// <summary>
    /// Utility class for the BigNumber class. Containing several
    /// convenient methods. 
    /// </summary>
    public static class BigNumberUtils
    {
        private const int CharPadding = 100;

        private static readonly BigNumber RandomMultiplier = new BigNumber("16807");
        private static readonly BigNumber RandomModulus = new BigNumber("2147483647");

        /// <summary>
        /// This method converts the parameter given plain text into a BigNumber representation
        /// </summary>
        /// <param name="plainText">The plain text to convert</param>
        /// <returns>The BigNumber after conversion</returns>
        public static BigNumber FromPlainText(string plainText)
        {
            var result = new BigNumber();

            foreach (var c in plainText)
            {
                int pos = c;

                // add 100 to guarantee 3 digits for all characters
                var seq = (pos + CharPadding).ToString(CultureInfo.InvariantCulture);

                var digit1 = string.Empty;
                var digit2 = string.Empty;
                var digit3 = string.Empty;

                try
                {
                    digit1 = seq.Substring(0, 1);
                    digit2 = seq.Substring(1, 1);
                    digit3 = seq.Substring(2, 1);
                }
                catch (ArgumentOutOfRangeException)
                {
                }

                result.Append(Int32.Parse(digit1));
                result.Append(Int32.Parse(digit2));
                result.Append(Int32.Parse(digit3));
            }

            return result;
        }

        /// <summary>
        /// This method converts the parameter given BigNumber to plain text.
        /// </summary>
        /// <param name="bigNumber">The BigNumber to convert</param>
        /// <returns>The plain text after conversion</returns>
        public static string AsPlainText(BigNumber bigNumber)
        {
            var text = string.Empty;
            var failed = false;

            for (var i = 0; i < bigNumber.Count; i += 3)
            {
                try
                {
                    var digit1 = bigNumber.GetDigit(i);
                    var digit2 = bigNumber.GetDigit(i + 1);
                    var digit3 = bigNumber.GetDigit(i + 2);

                    var pos = string.Format("{0}{1}{2}", digit1, digit2, digit3);

                    // subtract the 100 that's added in FromPlainText()
                    text += Convert.ToChar(Int32.Parse(pos) - CharPadding);

                }
                catch (Exception)
                {
                    failed = true;
                    break;
                }
            }

            return !failed ? text : bigNumber.AsString;
        }

        /// <summary>
        /// Converts the parameter given BigNumber object to
        /// binary representation.
        /// </summary>
        /// <param name="number">The BigNumber to convert</param>
        /// <returns>The resulting binary sequence</returns>
        public static string ToBinaryString(BigNumber number)
        {
            var result = string.Empty;

            while (!number.IsEqual(BigNumber.Zero))
            {
                var bit = BigNumber.Modulus(number, BigNumber.Two);

                result = bit.AsString + result;

                number = BigNumber.Divide(number, BigNumber.Two);
            }

            return result;
        }

        /// <summary>
        /// Converts the parameter given binary string to a BigNumber
        /// object. 
        /// </summary>
        /// <param name="binaryString">The binary string to convert</param>
        /// <returns>The resulting BigNumber object</returns>
        public static BigNumber FromBinaryString(string binaryString)
        {
            var result = BigNumber.Zero.Duplicate();

            var pos = binaryString.Length;

            foreach (var bit in binaryString)
            {
                if (bit == '1')
                {
                    var mostSignificantBit = new BigNumber(string.Format("{0}", pos - 1));

                    var value = BigNumber.Pow(BigNumber.Two, mostSignificantBit);

                    result = BigNumber.Add(result, value);
                }

                pos--;
            }

            return result;
        }

        /// <summary>
        /// Generates a random sequence of BigNumber objects
        /// </summary>
        /// <param name="minValue">The minimum value of the sequence</param>
        /// <param name="maxValue">The maximum value of the sequence</param>
        /// <param name="size">The size of the sequence, or the number of BigNumber objects to generate</param>
        /// <returns>A IEnumerable object containing the generated BigNumber objects</returns>
        public static IEnumerable<BigNumber> RandomSequence(BigNumber minValue, BigNumber maxValue, BigNumber size)
        {
            var list = new List<BigNumber>();

            // Add 1 to maxValue to ensure that the resulting values can include max
            maxValue = BigNumber.Add(maxValue, BigNumber.One);

            var diff = BigNumber.Subtract(maxValue, minValue);

            for (var i = BigNumber.Zero.Duplicate(); i.LessThan(size); i = BigNumber.Add(i, BigNumber.One))
            {
                var rand = RandomNumber(GenerateSeed());

                while (rand.LessThan(minValue))
                {
                    // square value while less than min
                    rand = BigNumber.Pow(rand, BigNumber.Two);
                }

                // make sure minValue <= rand <= maxValue
                var result = BigNumber.Modulus(rand, diff);

                result = BigNumber.Add(result, minValue);

                list.Add(result);
            }

            return list;
        }

        /// <summary>
        /// Generates a single random number from the parameter given seed
        /// </summary>
        /// <param name="seed">The seed from which to generate a random BigNumber</param>
        /// <returns>The pseudo random generated BigNumber</returns>
        private static BigNumber RandomNumber(BigNumber seed)
        {
            var rnd = seed.Duplicate();

            rnd = BigNumber.Multiply(RandomMultiplier, rnd);
            rnd = BigNumber.Modulus(rnd, RandomModulus);

            return rnd;
        }

        /// <summary>
        /// Generates a seed by using the system's clock. 
        /// </summary>
        /// <returns>The seed, represented by a BigNumber object</returns>
        private static BigNumber GenerateSeed()
        {
            var millis = 0;

            while (millis == 0)
            {
                millis = DateTime.Now.Millisecond;
            }

            return new BigNumber(string.Format("{0}", millis));
        }

        /// <summary>
        /// Generates a prime of a parameter given length
        /// </summary>
        /// <param name="digits">The number of digits the generated prime number should contain</param>
        /// <returns>The resulting generated prime number, as a BigNumber object</returns>
        public static BigNumber GeneratePrime(int digits)
        {
            var minVal = string.Empty;
            var maxVal = string.Empty;

            for (var i = 0; i < digits; i++)
            {
                maxVal += "9";
                minVal += i == 0 ? "1" : "0";
            }

            var min = new BigNumber(minVal);
            var max = new BigNumber(maxVal);

            var rnd = RandomSequence(min, max, BigNumber.One).First(); // 1 random no.

            while (rnd.Count > digits)
            {
                rnd.Pop();
            }

            var toAdd = rnd.GetDigit(rnd.Count - 1)%2 == 0 ? BigNumber.One : BigNumber.Two;

            while (!IsProbablyPrime(rnd))
            {
                rnd = BigNumber.Add(rnd, toAdd);

                if (rnd.Count > digits)
                {
                    rnd.Pop();
                }
            }

            return rnd;
        }

        /// <summary>
        /// Checks whether the given number is probably a prime number. 
        /// This check is performed by using the simple Fermat probability test.
        /// </summary>
        /// <param name="candidate">The number to check for primality</param>
        /// <returns>true if the number is probably a prime number (not guaranteed), false otherwise</returns>
        private static bool IsProbablyPrime(BigNumber candidate)
        {
            if (candidate.IsEqual(BigNumber.Zero) || candidate.IsEqual(BigNumber.One))
            {
                return false;
            }

            var coprime =
                RandomSequence(BigNumber.One, BigNumber.Subtract(candidate, BigNumber.One), BigNumber.One).First();

            while (!IsCoprimeTo(candidate, coprime))
            {
                coprime = BigNumber.Add(coprime, BigNumber.One);
            }

            var exponent = BigNumber.Subtract(candidate, BigNumber.One);

            var result = BigNumber.ModPow(coprime, exponent, candidate);

            return result.IsEqual(BigNumber.One);
        }

        /// <summary>
        /// Generates a random BigNumber that is coprime to the parameter given BigNumber
        /// </summary>
        /// <param name="bigNumber">The BigNumber from which to generate a coprime</param>
        /// <returns>The generated coprime</returns>
        public static BigNumber GenerateRandomCoprimeTo(BigNumber bigNumber)
        {
            var rnd = RandomSequence(BigNumber.One, bigNumber, BigNumber.One).First();

            var sizeExceeded = false;

            while (!IsCoprimeTo(rnd, bigNumber))
            {
                rnd = BigNumber.Add(rnd, BigNumber.One);

                if (!rnd.GreaterThan(bigNumber)) continue;

                sizeExceeded = true;
                break;
            }

            if (sizeExceeded)
            {
                while (!IsCoprimeTo(rnd, bigNumber))
                {
                    rnd = BigNumber.Subtract(rnd, BigNumber.One);
                }
            }

            return rnd;
        }

        /// <summary>
        /// Checks whether two parameter given BigNumber objects are coprime
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>true if the numbers are coprime, false otherwise</returns>
        private static bool IsCoprimeTo(BigNumber a, BigNumber b)
        {
            return BigNumber.Gcd(a, b).IsEqual(BigNumber.One);
        }
    }
}