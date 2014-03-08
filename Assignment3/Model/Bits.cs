using System;
using System.Collections;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace Assignment3.Model
{
    /// <summary>
    /// This class is basically a clone of the .NET BitArray class. 
    /// However, some convenient methods exist to better serve the
    /// assignment's purpose and to significantly ease the debugging process.
    /// </summary>
    public class Bits
    {
        private BitArray _bits;

        public Bits(int size)
        {
            _bits = new BitArray(size);
        }

        public Bits(bool[] bits)
        {
            _bits = new BitArray(bits);
        }

        public Bits(Bits bits)
        {
            _bits = new BitArray(bits._bits);
        }

        public int Count
        {
            get { return _bits.Count; }
        }

        public string AsBinaryString
        {
            get
            {
                var sb = new StringBuilder();

                for (var i = 0; i < _bits.Count; i++)
                {
                    var bit = _bits.Get(i);

                    sb.Append(bit ? "1" : "0");
                }

                return sb.ToString();
            }
        }

        public BigInteger AsBigInteger
        {
            get
            {
                var bytes = new byte[_bits.Length/RSHA.ByteSize];
                _bits.CopyTo(bytes, 0);
                return BigInteger.Abs(new BigInteger(bytes));
            }
        }

        public string AsPlainText
        {
            get
            {
                var sb = new StringBuilder();

                var binary = AsBinaryString;

                for (var i = 0; i < binary.Length; i += RSHA.ByteSize)
                {
                    var byteString = binary.Substring(i, RSHA.ByteSize);

                    if (!byteString.Equals("00000000"))
                    {
                        var digit = Byte.Parse(Convert.ToInt32(byteString, 2).ToString(CultureInfo.InvariantCulture));

                        var letter = Encoding.UTF8.GetString(new[] {digit});

                        sb.Append(letter);
                    }
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Remove the X leftmost bits
        /// </summary>
        /// <param name="places">The number of bits to remove</param>
        public void ShrinkLeft(int places)
        {
            var temp = new BitArray(_bits);

            try
            {
                _bits = new BitArray(temp.Count - places);
            }
            catch (ArgumentOutOfRangeException)
            {
                _bits = new BitArray(0);
            }

            for (var i = 0; i < _bits.Count; i++)
            {
                _bits.Set(i, temp.Get(i + places));
            }
        }

        public void ShrinkRight(int places)
        {
            var temp = new BitArray(_bits);

            try
            {
                _bits = new BitArray(temp.Count - places);
            }
            catch (ArgumentOutOfRangeException)
            {
                _bits = new BitArray(0);
            }

            for (var i = 0; i < _bits.Count; i++)
            {
                _bits.Set(i, temp.Get(i));
            }
        }

        /// <summary>
        /// Pads the X number of left bit places with the parameter given value
        /// </summary>
        /// <param name="value">The value to pad with</param>
        /// <param name="places">How many bits to pad</param>
        public void PadLeft(bool value, int places)
        {
            var temp = new BitArray(_bits);

            var size = temp.Count + places;

            _bits = new BitArray(size);

            for (var i = 0; i < size; i++)
            {
                if (i < places)
                {
                    _bits.Set(i, value);
                    continue;
                }

                _bits.Set(i, temp.Get(i - places));
            }
        }

        public Bits Xor(Bits xorWith)
        {
            _bits.Xor(xorWith._bits);

            return this;
        }

        public Bits Or(Bits orWith)
        {
            _bits.Or(orWith._bits);

            return this;
        }

        public void Set(int index, bool value)
        {
            _bits.Set(index, value);
        }

        public bool Get(int index)
        {
            return _bits.Get(index);
        }
    }
}