using System;
using System.Numerics;

namespace Assignment3.Model
{
    [Serializable]
    public class UserPublicKey
    {
        public UserPublicKey()
        {
        }

        public UserPublicKey(UserPublicKey copy)
        {
            Y = BigInteger.Parse(copy.Y.ToString());
        }

        public BigInteger Y { get; set; }
    }
}
