using System;
using System.Numerics;
using Assignment3.Utils;

namespace Assignment3.Model
{
    [Serializable]
    public class Message
    {
        public string Content { get; set; }
        public DateTime Stored { private get; set; }
        public GlobalKeys GlobalKeys { get; set; }
        public UserPublicKey UserPublicKey { get; set; }
        public UserPrivateKey UserPrivateKey { get; set; }
        public SecretNumber SecretNumber { get; set; }
        public Signature Signature { get; private set; }

        public string StoredText
        {
            get { return string.Format("{0: d. MMM yyyy HH:mm:ss}", Stored); }
        }

        public void Sign()
        {
            var rsha = new RSHA();

            var concatenatedMessage = RSHAUtils.StringToBits(Content + Stored);

            var r = BigInteger.ModPow(GlobalKeys.G, SecretNumber.K, GlobalKeys.P)%GlobalKeys.Q;

            var kInverse = DSAUtils.ModInverse(SecretNumber.K, GlobalKeys.Q);

            var hashValue = rsha.Hash(concatenatedMessage).AsBigInteger;

            var s = (kInverse*(hashValue + (UserPrivateKey.X*r)))%GlobalKeys.Q;

            Signature = new Signature
                {
                    R = r,
                    S = s
                };
        }

        public bool Verify()
        {
            var rsha = new RSHA();

            var concatenatedMessage = RSHAUtils.StringToBits(Content + Stored);

            var w = DSAUtils.ModInverse(Signature.S, GlobalKeys.Q)%GlobalKeys.Q;

            var hashValue = rsha.Hash(concatenatedMessage).AsBigInteger;

            var u1 = (hashValue*w)%GlobalKeys.Q;
            var u2 = (Signature.R*w)%GlobalKeys.Q;

            var v = ((BigInteger.ModPow(GlobalKeys.G, u1, GlobalKeys.P)*
                      BigInteger.ModPow(UserPublicKey.Y, u2, GlobalKeys.P))%GlobalKeys.P)%
                    GlobalKeys.Q;

            return (v == Signature.R);
        }
    }
}