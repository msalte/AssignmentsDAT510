﻿using System;
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
    /// include string and hexadecimal representation of the bit
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

        public BitSequence(byte[] values)
        {
            _bits = new BitArray(values);
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
                var bytes = new byte[Count/BitUtils.ByteSize];

                if (_bits != null) _bits.CopyTo(bytes, 0);

                return Encoding.UTF8.GetString(bytes);
            }
        }

        public string AsHexString
        {
            get
            {
                var bytes = new byte[Count/BitUtils.ByteSize];

                if (_bits != null) _bits.CopyTo(bytes, 0);

                return BitConverter.ToString(bytes);
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