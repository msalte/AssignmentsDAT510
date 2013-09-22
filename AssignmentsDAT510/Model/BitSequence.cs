using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using AssignmentsDAT510.Util;

namespace AssignmentsDAT510.Model
{
    /// <summary>
    /// This class i basically a BitArray but it has some additional 
    /// properties that is very helpful while making sure everything
    /// is working as intended during debugging. Such properties 
    /// include a string representation of the bit
    /// sequence.
    /// </summary>
    public class BitSequence
    {
        private BitArray _bits;

        public BitSequence(int size)
        {
            _bits = new BitArray(size);
        }

        public BitSequence(bool[] values)
        {
            _bits = new BitArray(values);
        }

        public BitSequence(string input)
        {
            var isInputBinary = !input.Any(bit => bit != '1' && bit != '0');

            if (isInputBinary)
            {
                CreateFromBinaryInput(input);
            }
            else
            {
                CreateFromTextInput(input);
            }

        }

        private void CreateFromBinaryInput(string binaryInput)
        {
            _bits = new BitArray(binaryInput.Length);

            for (var i = 0; i < binaryInput.Length; i++)
            {
                _bits.Set(i, binaryInput[i] == '1');
            }
        }

        private void CreateFromTextInput(string textInput)
        {
            var sb = new StringBuilder();

            foreach (var c in textInput)
            {
                var bytes = Encoding.UTF8.GetBytes(c.ToString(CultureInfo.InvariantCulture));

                foreach (var b in bytes)
                {
                    sb.Append(Convert.ToString(b, 2).PadLeft(BitUtils.ByteSize, '0'));
                }
            }

            var binaryString = sb.ToString();

            _bits = new BitArray(binaryString.Length);

            for (var i = 0; i < binaryString.Length; i++)
            {
                _bits.Set(i, binaryString[i] == '1');
            }
        }

        public int Count
        {
            get { return _bits != null ? _bits.Count : 0; }
        }

        public BitSequence XOR(BitSequence toXorWith)
        {
            _bits = _bits.Xor(toXorWith._bits);

            return this;
        }

        public string AsBinaryString
        {
            get
            {
                var bs = new StringBuilder();

                if (_bits != null)
                {
                    for (var i = 0; i < Count; i++)
                    {
                        bs.Append(Get(i) ? "1" : "0");
                    }
                }

                return bs.ToString();
            }
        }

        public string AsTextString
        {
            get
            {
                var sb = new StringBuilder();

                var binary = AsBinaryString;

                for (var i = 0; i < binary.Length; i += BitUtils.ByteSize)
                {
                    var byteString = binary.Substring(i, BitUtils.ByteSize);

                    if (!byteString.Equals("00000000"))
                    {
                        var digit = Byte.Parse(Convert.ToInt32(byteString, 2).ToString(CultureInfo.InvariantCulture));

                        var letter = Encoding.UTF8.GetString(new [] {digit});

                        sb.Append(letter);

                    }
                }

                return sb.ToString();
            }
        }

        public int AsInteger
        {
            get { return Convert.ToInt32(AsBinaryString, 2); }
        }

        public bool Get(int index)
        {
            return _bits.Get(index);
        }

        public void Set(int index, bool value)
        {
            _bits.Set(index, value);
        }
    }
}