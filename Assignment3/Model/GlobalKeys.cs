using System;
using System.Numerics;

namespace Assignment3.Model
{
    [Serializable]
    public class GlobalKeys
    {
        public GlobalKeys()
        {
        }

        public GlobalKeys(GlobalKeys copy)
        {
            Q = BigInteger.Parse(copy.Q.ToString());
            P = BigInteger.Parse(copy.P.ToString());
            G = BigInteger.Parse(copy.G.ToString());
        }

        public BigInteger Q { get; set; }
        public BigInteger P { get; set; }
        public BigInteger G { get; set; }
    }
}
