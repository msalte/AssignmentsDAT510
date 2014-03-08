using System;
using System.Numerics;

namespace Assignment3.Model
{
    [Serializable]
    public class SecretNumber
    {
        public SecretNumber()
        {
        }

        public SecretNumber(SecretNumber copy)
        {
            K = BigInteger.Parse(copy.K.ToString());
        }

        public BigInteger K { get; set; }
    }
}