using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Assignment2.Interface;
using Assignment2.Utils;

namespace Assignment2.Model
{
    public class RSA
    {
        private readonly ICryptographyProgressCallback _cryptographyProgressCallback;
        private readonly IGenerationProgressCallback _generationProgressCallback;

        public RSA(ICryptographyProgressCallback cryptographyProgressCallback,
            IGenerationProgressCallback generationProgressCallback)
        {
            _cryptographyProgressCallback = cryptographyProgressCallback;
            _generationProgressCallback = generationProgressCallback;
        }

        public void ExecuteCryptography(RSAParameters rsaParameters, bool encrypt, string input, bool isInputBinary)
        {
            try
            {
                _cryptographyProgressCallback.OnCryptographyBegin();

                BigNumber message;

                if (encrypt)
                {
                    message = !isInputBinary
                        ? BigNumberUtils.FromPlainText(input)
                        : BigNumberUtils.FromBinaryString(input);
                }
                else
                {
                    message = BigNumberUtils.FromBinaryString(input);
                }

                var output = BigNumber.ModPow(message, encrypt ? rsaParameters.E : rsaParameters.D, rsaParameters.N);

                _cryptographyProgressCallback.OnCryptographyComplete(encrypt
                    ? BigNumberUtils.ToBinaryString(output)
                    : BigNumberUtils.AsPlainText(output));
            }
            catch (Exception)
            {
                _cryptographyProgressCallback.OnCryptographyError(string.Format("An error occurred during {0}.",
                    encrypt ? "encryption" : "decryption"));
            }
        }

        public async void GeneratePrimes(int primeLength)
        {
            try
            {
                var timeElapsed = new Stopwatch();
                var generatedParameters = new RSAParameters();

                timeElapsed.Start();

                _generationProgressCallback.OnGenerationBegin();

                generatedParameters.P = await Task.Run(() => BigNumberUtils.GeneratePrime(primeLength));
                generatedParameters.Q = await Task.Run(() => BigNumberUtils.GeneratePrime(primeLength));

                while (generatedParameters.P.IsEqual(generatedParameters.Q))
                {
                    generatedParameters.Q = await Task.Run(() => BigNumberUtils.GeneratePrime(primeLength));
                }

                generatedParameters.N = BigNumber.Multiply(generatedParameters.P, generatedParameters.Q);

                generatedParameters.Phi = BigNumber.Multiply(BigNumber.Subtract(generatedParameters.P, BigNumber.One),
                    BigNumber.Subtract(generatedParameters.Q, BigNumber.One));

                generatedParameters.E = BigNumberUtils.GenerateRandomCoprimeTo(generatedParameters.Phi);

                generatedParameters.D = BigNumber.ModInverse(generatedParameters.E, generatedParameters.Phi);

                timeElapsed.Stop();

                _generationProgressCallback.OnGenerationComplete(timeElapsed, generatedParameters);
            }
            catch (Exception)
            {
                _generationProgressCallback.OnGenerationError("An error occurred during parameter generation.");
            }
        }
    }
}