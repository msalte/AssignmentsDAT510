using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assignment2.Utils;

namespace Assignment2.Model
{
    /// <summary>
    /// Custom implementation of a BigInteger. Implements the IEnumerable interface
    /// to enable some convenient LINQ queries.
    /// </summary>
    public class BigNumber : IEnumerable<int>, IComparable<BigNumber>
    {
        private readonly List<int> _digits;

        private const int Ten = 10;
        private const int Nine = 9;

        public static readonly BigNumber Two = new BigNumber("2");
        public static readonly BigNumber One = new BigNumber("1");
        public static readonly BigNumber Zero = new BigNumber("0");

        public BigNumber() : this("")
        {
        }

        public BigNumber(string number)
        {
            _digits = new List<int>();

            foreach (var digit in number)
            {
                _digits.Add(Int32.Parse(digit.ToString(CultureInfo.InvariantCulture)));
            }
        }

        public BigNumber(List<int> digits)
        {
            _digits = digits;
        }

        public int Count
        {
            get { return _digits.Count; }
        }

        public bool IsNegative { get; set; }

        public string AsString
        {
            get { return _digits.Aggregate(string.Empty, (current, i) => current + i); }
        }

        public BigNumber Duplicate()
        {
            return new BigNumber(new List<int>(_digits))
            {
                IsNegative = IsNegative
            };
        }

        /// <summary>
        /// Adds two BigNumber objects together
        /// </summary>
        /// <param name="n1">The first number</param>
        /// <param name="n2">The second number</param>
        /// <returns>The result from the addition operation</returns>
        public static BigNumber Add(BigNumber n1, BigNumber n2)
        {
            var number1 = n1.Duplicate();
            var number2 = n2.Duplicate();

            if (number1.IsNegative && number2.IsNegative)
            {
                // -18+(-14) = -18-14 = -32
                return Subtract(number1, number2);
            }

            if (number1.IsNegative && !number2.IsNegative)
            {
                // -18+14 = 14-18 = -4
                number1.IsNegative = false;

                return Subtract(number2, number1);
            }

            if (!number1.IsNegative && number2.IsNegative)
            {
                // 18+(-14) = 18-14 = 4
                number2.IsNegative = false;

                return Subtract(number1, number2);
            }

            var capacity = number1.Count > number2.Count ? number1.Count : number2.Count;

            var resultingDigits = InitializeList(capacity);

            var carry = 0;

            for (var i = capacity - 1; i >= 0; i--)
            {
                var otherDigit = number2.Pop();
                var thisDigit = number1.Pop();

                var addedValue = otherDigit + thisDigit;

                if (carry != 0)
                {
                    addedValue += carry;
                    carry = 0;
                }

                if (addedValue >= Ten)
                {
                    addedValue = addedValue - Ten;
                    carry = 1;
                }

                resultingDigits[i] = addedValue;
            }

            if (carry != 0)
            {
                resultingDigits.Insert(0, carry);
            }

            return new BigNumber(resultingDigits);
        }

        /// <summary>
        /// Subtracts a BigNumber's value from another BigNumber
        /// </summary>
        /// <param name="n1">The BigNumber to subtract from</param>
        /// <param name="n2">The BigNumber to subtract</param>
        /// <returns>A new BigNumber instance representing the result of the subtraction operation</returns>
        public static BigNumber Subtract(BigNumber n1, BigNumber n2)
        {
            n1.RemovePrecedingZeros();
            n2.RemovePrecedingZeros();

            var number1 = n1.Duplicate();
            var number2 = n2.Duplicate();

            if (number1.IsNegative && number2.IsNegative)
            {
                // -18-(-14) = -18+14 = -4
                number2.IsNegative = false;

                return Add(number1, number2);
            }

            if (number1.IsNegative && !number2.IsNegative)
            {
                // -18-14 = -(18+14) = -38
                number1.IsNegative = false;

                var added = Add(number1, number2);

                added.IsNegative = true; // always negative

                return added;
            }

            if (!number1.IsNegative && number2.IsNegative)
            {
                // 18-(-14) = 18+14 = 38
                number2.IsNegative = false;

                return Add(number1, number2);
            }

            var thisIsGreater = number1.GreaterThan(number2);

            var resultingDigits = InitializeList(thisIsGreater ? number1.Count : number2.Count);

            if (thisIsGreater || number1.IsEqual(number2))
            {
                for (var i = number1.Count - 1; i >= 0; i--)
                {
                    var thisDigit = number1.Peek();
                    var otherDigit = number2.Peek();

                    var mustBorrow = thisDigit < otherDigit;

                    if (mustBorrow)
                    {
                        var temp = number1.Duplicate();

                        var borrowLeftOffset = 0; // no. of places to the left to borrow from

                        while (temp.Peek() == 0)
                        {
                            borrowLeftOffset++;

                            temp.Pop();
                        }

                        if (borrowLeftOffset > 0)
                        {
                            temp = number1.Duplicate();

                            var borrowIndex = (temp.Count - 1) - borrowLeftOffset;

                            temp.DecreaseDigitBy(borrowIndex, 1);

                            for (var k = borrowIndex; k < temp.Count; k++)
                            {
                                var isLastDigit = k == temp.Count - 1;

                                if (isLastDigit)
                                {
                                    temp.IncreaseDigitBy(k, Ten);
                                }
                                else if (k != borrowIndex)
                                {
                                    temp.IncreaseDigitBy(k, Nine);
                                }
                            }
                        }
                        else
                        {
                            var borrowIndex = i > 0 ? i - 1 : 0;

                            temp.DecreaseDigitBy(borrowIndex, 1);
                            temp.IncreaseDigitBy(i, Ten);
                        }

                        number1 = temp;

                        thisDigit = number1.Peek();
                    }

                    resultingDigits[i] = thisDigit - otherDigit;

                    number1.Pop();
                    number2.Pop();
                }

                return new BigNumber(resultingDigits).RemovePrecedingZeros();
            }

            // The other number is greater, the result is consequently a negative number
            for (var i = number2.Count - 1; i >= 0; i--)
            {
                var thisDigit = number1.Peek();
                var otherDigit = number2.Peek();

                var mustBorrow = otherDigit < thisDigit;

                if (mustBorrow)
                {
                    var temp = number2.Duplicate();

                    var borrowLeftOffset = 0; // no. of places to the left to borrow from

                    while (temp.Peek() == 0)
                    {
                        borrowLeftOffset++;

                        temp.Pop();
                    }

                    if (borrowLeftOffset > 0)
                    {
                        temp = number2.Duplicate();

                        var borrowIndex = (temp.Count - 1) - borrowLeftOffset;

                        temp.DecreaseDigitBy(borrowIndex, 1);

                        for (var k = borrowIndex; k < temp.Count; k++)
                        {
                            if (k == temp.Count - 1)
                            {
                                temp.IncreaseDigitBy(k, Ten);
                            }
                            else if (k != borrowIndex)
                            {
                                temp.IncreaseDigitBy(k, Nine);
                            }
                        }
                    }
                    else
                    {
                        var borrowIndex = i > 0 ? i - 1 : 0;

                        temp.DecreaseDigitBy(borrowIndex, 1);
                        temp.IncreaseDigitBy(i, Ten);
                    }

                    number2 = temp;

                    otherDigit = number2.Peek();
                }

                resultingDigits[i] = otherDigit - thisDigit;

                number1.Pop();
                number2.Pop();
            }

            return new BigNumber(resultingDigits)
            {
                IsNegative = true // the result is a negative number
            }.RemovePrecedingZeros();
        }

        /// <summary>
        /// Returns the greatest common denominator of two parameter
        /// given BigNumber instances.
        /// </summary>
        /// <param name="n1">The first number</param>
        /// <param name="n2">The second number</param>
        /// <returns>A new BigNumber instance representing the resulting GCD</returns>
        public static BigNumber Gcd(BigNumber n1, BigNumber n2)
        {
            var a = n1.Duplicate();
            var b = n2.Duplicate();

            if (a.IsNegative || b.IsNegative || b.GetDigit(0) == 0)
            {
                throw new ArgumentException();
            }

            while (!b.IsEqual(Zero))
            {
                var rmnd = Modulus(a, b);

                a = b.Duplicate();
                b = rmnd.Duplicate();
            }

            return a;
        }

        private static List<int> InitializeList(int capacity)
        {
            var resultingDigits = new List<int>();

            for (var i = 0; i < capacity; i++)
            {
                resultingDigits.Add(0);
            }

            return resultingDigits;
        }

        private BigNumber RemovePrecedingZeros()
        {
            while (Count > 1 && _digits[0] == 0)
            {
                _digits.RemoveAt(0);
            }

            return this;
        }

        /// <summary>
        /// Multiplies two BigNumber instances
        /// </summary>
        /// <param name="n1">The first number</param>
        /// <param name="n2">The second number</param>
        /// <returns>A new BigNumber instance representing the result of the multiplication operation</returns>
        public static BigNumber Multiply(BigNumber n1, BigNumber n2)
        {
            var number1 = n1.Duplicate();
            var number2 = n2.Duplicate();

            var toSummarize = new List<BigNumber>();

            var carry = 0;
            var placesToPad = 0;

            for (var i = number2.Count - 1; i >= 0; i--)
            {
                var otherDigit = number2.Pop();

                var result = string.Empty;

                for (var j = number1.Count - 1; j >= 0; j--)
                {
                    var isLast = j == 0;

                    var thisDigit = number1.GetDigit(j);

                    var res = otherDigit*thisDigit + carry;

                    if (!isLast)
                    {
                        carry = res/Ten;
                        res = res%Ten;
                    }

                    result = res + result;
                }

                toSummarize.Add(new BigNumber(result.PadRight(result.Length + placesToPad, '0')));

                placesToPad++;
                carry = 0;
            }

            var sum = Sum(toSummarize);

            sum.IsNegative = number1.IsNegative ^ number2.IsNegative;

            return sum;
        }

        private static BigNumber Sum(IEnumerable<BigNumber> list)
        {
            var result = Zero.Duplicate();

            return list.Aggregate(result, Add);
        }

        /// <summary>
        /// Executes the Modular exponentiation operation on a given BigNumber instnace
        /// </summary>
        /// <param name="baseNumber">The number to which the modular exponentiation should be performed</param>
        /// <param name="exponent">The exponent value</param>
        /// <param name="modulus">The modulus value</param>
        /// <returns>A new BigNumber instance representing the result of the modular exponentiation operation</returns>
        public static BigNumber ModPow(BigNumber baseNumber, BigNumber exponent, BigNumber modulus)
        {
            var result = One.Duplicate();

            var binaryExponent = BigNumberUtils.ToBinaryString(exponent);

            var length = binaryExponent.Length;
            
            for (var i = length - 1; i >= 0; i--)
            {
                var isBitOne = binaryExponent[i] == '1';

                if (isBitOne)
                {
                    result = Multiply(result, baseNumber);
                    result = Modulus(result, modulus);
                }

                baseNumber = Pow(baseNumber, Two);
                baseNumber = Modulus(baseNumber, modulus);
            }

            return result;
        }

        /// <summary>
        /// Executes the modular inverse operation on two numbers
        /// </summary>
        /// <param name="a">The first number, the initial numerator</param>
        /// <param name="b">The second number, the initial denominator</param>
        /// <returns>A new BigNumber instance representing the result of the modular inverse operation</returns>
        public static BigNumber ModInverse(BigNumber a, BigNumber b)
        {
            var numerator = Modulus(a, b);
            var divideBy = b.Duplicate();

            var thisX = Zero;
            var prevX = One;

            while (divideBy.GreaterThan(Zero) && !divideBy.IsNegative)
            {
                // divisor > 0

                var result = Divide(numerator, divideBy);
                var remaining = Modulus(numerator, divideBy);

                if (remaining.IsNegative || remaining.IsEqual(Zero))
                {
                    // remainder <= 0

                    break;
                }

                var next = Subtract(prevX, Multiply(thisX, result));

                prevX = thisX.Duplicate();
                thisX = next.Duplicate();

                numerator = divideBy.Duplicate();
                divideBy = remaining.Duplicate();
            }

            if (!divideBy.IsEqual(One))
            {
                throw new ArgumentException("Arguments are not relatively prime");
            }

            return thisX.IsNegative ? Add(thisX, b) : thisX;
        }

        /// <summary>
        /// Executes the "power of" operation on a parameter given BigNumber instance
        /// </summary>
        /// <param name="bigNumber">The number to which the exponentiation should be performed</param>
        /// <param name="exponent">The exponent value</param>
        /// <returns>A new BigNumber instance representing the result of the exponentiation operation</returns>
        public static BigNumber Pow(BigNumber bigNumber, BigNumber exponent)
        {
            if (exponent.IsEqual(Zero)) return One;

            var result = bigNumber.Duplicate();
            var e = exponent.Duplicate();

            var tens = 0;

            var isFirstIteration = true;

            for (var i = e.Count - 1; i >= 0; i--)
            {
                var digit = e.Pop();

                for (var k = 0; k < tens; k++)
                {
                    digit *= 10;
                }

                for (var n = 0; n < digit; n++)
                {
                    if (isFirstIteration)
                    {
                        // skip first iteration because x^1 = x

                        isFirstIteration = false;

                        continue;
                    }

                    result = Multiply(result, bigNumber);
                }

                tens++;
            }

            return result;
        }

        /// <summary>
        ///  Divides two BigNumber instances
        /// </summary>
        /// <param name="number">The number to divide</param>
        /// <param name="divideBy">The number to divide by</param>
        /// <returns>A new BigNumber instance representing the result of the division operation</returns>
        public static BigNumber Divide(BigNumber number, BigNumber divideBy)
        {
            var ten = new BigNumber(Ten.ToString(CultureInfo.InvariantCulture));

            if (number.IsEqual(divideBy)) return One;

            var temp = number.Duplicate();

            var times = Zero.Duplicate();

            while (temp.GreaterThan(divideBy) || temp.IsEqual(divideBy))
            {
                var div = divideBy.Duplicate();

                var diff = temp.Count - div.Count;

                for (var i = 0; i < diff; i++)
                {
                    div.AppendZero();

                    if (div.GreaterThan(temp))
                    {
                        diff--;
                        div.Pop();
                    }
                }

                var exponent = new BigNumber(diff.ToString(CultureInfo.InvariantCulture));

                var toAdd = diff > 0 ? Pow(ten, exponent) : One;

                times = Add(times, toAdd);

                temp = Subtract(temp, div);
            }

            times.IsNegative = number.IsNegative ^ divideBy.IsNegative;

            return times;
        }

        /// <summary>
        /// Executes the modulus operation on two BigNumber instance
        /// </summary>
        /// <param name="number">The number to which the modulus operation should be performed</param>
        /// <param name="modulus">The modulus operator value</param>
        /// <returns>A new BigNumber instance representing the result from the modulus operation</returns>
        public static BigNumber Modulus(BigNumber number, BigNumber modulus)
        {
            if (number.IsNegative || modulus.IsNegative || modulus.GetDigit(0) == 0)
            {
                throw new ArgumentException();
            }

            if (modulus.IsEqual(One)) return Zero;

            var result = number.Duplicate();

            while (result.GreaterThan(modulus) || result.IsEqual(modulus))
            {
                var mod = modulus.Duplicate();

                var diff = result.Count - mod.Count;

                for (var i = 0; i < diff; i++)
                {
                    mod.AppendZero();

                    if (mod.GreaterThan(result))
                    {
                        mod.Pop();
                    }
                }

                result = Subtract(result, mod);
            }

            return result;
        }

        public bool GreaterThan(BigNumber bigNumber)
        {
            if (IsNegative != bigNumber.IsNegative)
            {
                return !IsNegative;
            }

            if (Count != bigNumber.Count) return Count > bigNumber.Count;

            var isGreaterThan = false;

            for (var i = 0; i < Count; i++)
            {
                if (_digits[i] == bigNumber.GetDigit(i)) continue;

                isGreaterThan = _digits[i] > bigNumber.GetDigit(i);

                break;
            }

            return isGreaterThan;
        }

        public bool LessThan(BigNumber bigNumber)
        {
            return !GreaterThan(bigNumber) && !IsEqual(bigNumber);
        }

        public bool IsEqual(BigNumber bigNumber)
        {
            if (Count != bigNumber.Count) return false;

            return !bigNumber.Where((t, i) => _digits[i] != bigNumber.GetDigit(i)).Any();
        }

        public IEnumerator<int> GetEnumerator()
        {
            return _digits.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Pop()
        {
            if (Count == 0) return 0;

            var lastIndex = Count - 1;

            var last = _digits[lastIndex];

            _digits.RemoveAt(lastIndex);

            return last;
        }

        private int Peek()
        {
            return Count == 0 ? 0 : _digits[Count - 1];
        }

        private void SetDigit(int index, int value)
        {
            _digits[index] = value;
        }

        public int GetDigit(int index)
        {
            return _digits[index];
        }

        private void IncreaseDigitBy(int index, int value)
        {
            SetDigit(index, GetDigit(index) + value);
        }

        private void DecreaseDigitBy(int index, int value)
        {
            SetDigit(index, GetDigit(index) - value);
        }

        public void Append(int digit)
        {
            _digits.Add(digit);
        }

        private void AppendZero()
        {
            Append(0);
        }

        public int CompareTo(BigNumber other)
        {
            if (GreaterThan(other))
            {
                return 1;
            }

            if (LessThan(other))
            {
                return -1;
            }

            return 0;
        }
    }
}