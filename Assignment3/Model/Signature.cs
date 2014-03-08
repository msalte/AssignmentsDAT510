using System;
using System.Numerics;

namespace Assignment3.Model
{
    [Serializable]
    public class Signature
    {
        public BigInteger R { get; set; }
        public BigInteger S { get; set; }
    }
}
