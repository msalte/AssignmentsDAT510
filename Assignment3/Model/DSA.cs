using System;
using System.Numerics;
using Assignment3.Interface;
using Assignment3.Utils;

namespace Assignment3.Model
{
    public class DSA
    {
        private static readonly BigInteger Two = BigInteger.Parse("2");

        public GlobalKeys GlobalKeys { get; private set; }
        public UserPrivateKey UserPrivateKey { get; private set; }
        public UserPublicKey UserPublicKey { get; private set; }
        public SecretNumber SecretNumber { get; private set; }

        private readonly INotifyParametersChanged _paramsChangedCallback;
        private readonly INotifyDSAErrorOccurred _dsaErrorOccurredCallback;

        public DSA(INotifyParametersChanged paramsChangedCallback, INotifyDSAErrorOccurred dsaErrorOccurredCallback)
        {
            _paramsChangedCallback = paramsChangedCallback;
            _dsaErrorOccurredCallback = dsaErrorOccurredCallback;

            GlobalKeys = new GlobalKeys();
            UserPrivateKey = new UserPrivateKey();
            UserPublicKey = new UserPublicKey();
            SecretNumber = new SecretNumber();
        }

        public Message SignMessage(string content)
        {
            try
            {
                var msg = new Message
                    {
                        Stored = DateTime.Now,
                        Content = content,
                        GlobalKeys = new GlobalKeys(GlobalKeys),
                        UserPublicKey = new UserPublicKey(UserPublicKey),
                        UserPrivateKey = new UserPrivateKey(UserPrivateKey),
                        SecretNumber = new SecretNumber(SecretNumber)
                    };

                msg.Sign();

                GenerateSecretNumber();

                return msg;
            }
            catch (Exception)
            {
                _dsaErrorOccurredCallback.DSAErrorOccurred(
                    "An error occurred!\n\nThe reason is possibly related to the relationships between manually input parameters being invalid.");
            }

            return null;
        }

        public void GenerateParameters()
        {
            GlobalKeys.Q = GenerateQ();
            GlobalKeys.P = GenerateP(GlobalKeys.Q);
            GlobalKeys.G = GenerateG(GlobalKeys.Q, GlobalKeys.P);

            UserPrivateKey.X = DSAUtils.RandomInteger(BigInteger.Zero, GlobalKeys.Q);

            UserPublicKey.Y = BigInteger.ModPow(GlobalKeys.G, UserPrivateKey.X, GlobalKeys.P);

            GenerateSecretNumber();
        }

        private void GenerateSecretNumber()
        {
            SecretNumber.K = DSAUtils.RandomInteger(BigInteger.Zero, GlobalKeys.Q);

            _paramsChangedCallback.OnParametersChanged();
        }

        private BigInteger GenerateG(BigInteger q, BigInteger p)
        {
            var h = BigInteger.Zero;

            while (BigInteger.ModPow(h, (p - 1)/q, p) <= BigInteger.One)
            {
                h = DSAUtils.RandomInteger(Two, p - Two);
            }

            return BigInteger.ModPow(h, (p - 1)/q, p);
        }

        private BigInteger GenerateP(BigInteger q)
        { 
            const int bitLength = 1024;

            var kMin = BigInteger.Pow(Two, bitLength - 1)/q;
            var kMax = BigInteger.Pow(Two, bitLength)/q;

            var k = DSAUtils.RandomInteger(kMin, kMax);

            var p = BigInteger.One;
            
            while (!DSAUtils.IsProbablyPrime(p))
            {
                p = BigInteger.Multiply(q, k) + 1;

                k++;
            }

            return p;
        }

        private BigInteger GenerateQ()
        {
            const int bitLength = 160;

            var minValue = BigInteger.Pow(Two, bitLength - 1);
            var maxValue = BigInteger.Pow(Two, bitLength);

            var q = DSAUtils.RandomInteger(minValue, maxValue);

            while (!DSAUtils.IsProbablyPrime(q))
            {
                q += q.IsEven ? 1 : 2;
            }

            return q;
        }
    }
}