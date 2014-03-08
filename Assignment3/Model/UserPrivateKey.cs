using System;
using System.Numerics;

namespace Assignment3.Model
{
    [Serializable]
    public class UserPrivateKey
    {
        public UserPrivateKey()
        {
        }

        public UserPrivateKey(UserPrivateKey copy)
        {
            X = BigInteger.Parse(copy.X.ToString());
        }

        public BigInteger X { get; set; }
    }
}
